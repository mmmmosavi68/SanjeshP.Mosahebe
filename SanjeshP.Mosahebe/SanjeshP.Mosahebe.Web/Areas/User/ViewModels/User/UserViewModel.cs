using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.WebFramework.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.User.ViewModels.User
{
    public class UserViewModel : BaseDto<UserViewModel, Entities.User.User, Guid>
    {
        public Guid UserId { get; set; }

        [Display(Name = "نام")]
        public string FirstName { get; set; }
        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; }
        [Display(Name = "کد ملی")]
        public string NationalCode { get; set; }
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }
        [Display(Name = "ایمیل")]
        public string EmailAddress { get; set; }
        [Display(Name = "شماره همراه")]
        public string PhoneNumber { get; set; }
        [Display(Name = "نوع کاربری")]
        public string UserTypeTitle { get; set; }
        [Display(Name = "نوع کاربری")]
        public int? RoleId { get; set; }
        [Display(Name = "فعال")]
        public bool IsActive { get; set; }
        [Display(Name = "وضعیت ( فعال/غیرفعال)")]
        public IsActiveNameType IsActiveName { get; set; }
    }
}
