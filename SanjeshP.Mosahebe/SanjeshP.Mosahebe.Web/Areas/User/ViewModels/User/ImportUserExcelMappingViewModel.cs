using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SanjeshP.Mosahebe.Web.Areas.User.ViewModels.User
{
    public class ImportUserExcelMappingViewModel
    {
        public string TempKey { get; set; }

        [Display(Name = "نقش")]
        public int RoleId { get; set; }

        public List<SelectListItem> Roles { get; set; } = new();

        public List<string> ExcelColumns { get; set; } = new();

        [Display(Name = "نام")]
        public string FirstNameColumn { get; set; }

        [Display(Name = "نام خانوادگی")]
        public string LastNameColumn { get; set; }

        [Display(Name = "شماره همراه")]
        public string PhoneNumberColumn { get; set; }

        [Display(Name = "کد ملی")]
        public string NationalCodeColumn { get; set; }

        [Display(Name = "ایمیل")]
        public string EmailAddressColumn { get; set; }

        [Display(Name = "نام کاربری")]
        public string UserNameColumn { get; set; }

        [Display(Name = "رمز عبور")]
        public string PasswordColumn { get; set; }

        [Display(Name = "فعال")]
        public string IsActiveColumn { get; set; }

        // 💡 فیلد جدید برای ستون رشته‌ها
        [Display(Name = "کد رشته‌ها")]
        public string FieldCodesColumn { get; set; } // یا FieldCodeColumn
    }
}
