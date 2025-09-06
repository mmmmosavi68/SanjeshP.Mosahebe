using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.User
{
    public class UserOtp : BaseEntity
    {
        public Guid UserId { get; set; } // ارتباط با کاربر

        public string Code { get; set; } // کد OTP

        public DateTime ExpireAt { get; set; } // تاریخ انقضای OTP

        public bool IsUsed { get; set; } = false; // اگر مصرف شد

        public string SentTo { get; set; } // شماره موبایل یا ایمیل

        public string Purpose { get; set; } // مثلاً "Login" یا "ForgotPassword"
    }
}
