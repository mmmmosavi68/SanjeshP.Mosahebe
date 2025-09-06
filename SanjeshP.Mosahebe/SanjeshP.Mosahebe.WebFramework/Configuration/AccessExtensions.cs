using Microsoft.Extensions.Caching.Memory;
using SanjeshP.Mosahebe.Common;
using System.Collections.Generic;
using System.Linq;

namespace SanjeshP.Mosahebe.WebFramework.Configuration
{
    public static class AccessExtensions
    {
        public static bool HasAccess(this IMemoryCache memoryCache, string token, string actionName)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(actionName))
                return false;

            var cacheKey = $"UserPermissions_{token}";
            if (memoryCache.TryGetValue(cacheKey, out List<UserAccessViewModel> accessData))
            {
                return accessData.Any(x => x.ActionName == actionName && x.SowMenu && (x.Person_Checkecd == true || x.Group_Checkecd == true));
            }

            return false;
        }
    }
}
