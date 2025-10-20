using System;

namespace SanjeshP.Mosahebe.Web.Areas.User.ViewModels.UserField
{
    public class UserFieldViewModel
    {
        public int Id { get; set; }             // Id جدول UserField
        public Guid UserId { get; set; }        // Guid کاربر
        public int ExamFieldId { get; set; }    // شناسه رشته آزمون
        public string ExamFieldTitle { get; set; } // عنوان رشته (برای نمایش)
        public int? StandardCode { get; set; }  // در صورت وجود کد استاندارد
    }
}
