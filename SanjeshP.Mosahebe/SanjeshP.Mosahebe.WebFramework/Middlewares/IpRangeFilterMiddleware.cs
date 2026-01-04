using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.WebFramework.Middlewares
{

    public class IpRangeFilterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _redirectUrl; // اضافه کردن این فیلد
        private readonly List<(byte[] networkBytes, byte[] maskBytes)> _filters = new List<(byte[], byte[])>();

        public IpRangeFilterMiddleware(RequestDelegate next, string[] allowedCidrs,string redirectURL)
        {
            _next = next;
            _redirectUrl = redirectURL; // مقداردهی فیلد

            foreach (var cidr in allowedCidrs)
            {
                var parts = cidr.Split('/');
                var ip = IPAddress.Parse(parts[0]);
                int prefixLength = int.Parse(parts[1]);

                var networkBytes = ip.GetAddressBytes();
                var maskBytes = CreateMask(networkBytes.Length, prefixLength);

                _filters.Add((networkBytes, maskBytes));
            }
        }

        private byte[] CreateMask(int length, int prefix)
        {
            var mask = new byte[length];
            int fullBytes = prefix / 8;
            for (int i = 0; i < fullBytes; i++) mask[i] = 0xFF;
            if (fullBytes < length)
            {
                mask[fullBytes] = (byte)(0xFF << (8 - (prefix % 8)));
            }
            return mask;
        }

        public async Task Invoke(HttpContext context)
        {
            var remoteIp = context.Connection.RemoteIpAddress;

            // اگر پشت پروکسی هستید، RemoteIpAddress توسط ForwardedHeadersMiddleware قبلاً اصلاح شده است
            if (remoteIp != null)
            {
                byte[] remoteBytes = remoteIp.GetAddressBytes();
                bool isAllowed = false;

                foreach (var filter in _filters)
                {
                    if (filter.networkBytes.Length != remoteBytes.Length) continue;

                    bool match = true;
                    for (int i = 0; i < remoteBytes.Length; i++)
                    {
                        if ((remoteBytes[i] & filter.maskBytes[i]) != (filter.networkBytes[i] & filter.maskBytes[i]))
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match) { isAllowed = true; break; }
                }

                if (!isAllowed)
                {
                    // به جای کد 403 و WriteAsync از ریدایرکت استفاده کنید
                    context.Response.Redirect(_redirectUrl);
                    return; // بسیار مهم: ادامه خط لوله را قطع می‌کند
                    //context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    //await context.Response.WriteAsync("Access Denied.");
                    //return;
                }
            }

            await _next(context);
        }
    }
}
