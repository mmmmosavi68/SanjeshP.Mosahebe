using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Services.SmsSender.PayamSMS
{
    public interface IPayamSMS
    {
        Task<int> Send(string _number, string _message);
    }
}
