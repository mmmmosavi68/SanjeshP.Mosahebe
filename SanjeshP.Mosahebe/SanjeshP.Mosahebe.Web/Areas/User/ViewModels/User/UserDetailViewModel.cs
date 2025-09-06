using AutoMapper;
using SanjeshP.Mosahebe.WebFramework.Api;
using System;
using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.User.ViewModels.User
{
    public class UserDetailViewModel : BaseDto<UserDetailViewModel, SanjeshP.Mosahebe.Entities.User.User, Guid>
    {
        public Guid UserId { get; set; }

        [Display(Name = "نام")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; }
        [Display(Name = "کد ملی")]
        public string NationalCode { get; set; }

        [Display(Name = "شماره همراه")]
        public string PhoneNumber { get; set; }

        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [Display(Name = "ایمیل")]
        public string EmailAddress { get; set; }

        [Display(Name = "نوع کاربری")]
        public int? RoleId { get; set; }

        [Display(Name = "فعال؟")]
        public bool IsActive { get; set; }

        [Display(Name = "تاریخ ایجاد")]
        public string CreateDate { get; set; }
        [Display(Name = "تاریخ ویرایش")]
        public string EditDate { get; set; }
        [Display(Name = "نقش")]
        public string RoleName { get; set; }

        [Display(Name = "تاریخ آخرین ورود")]
        public string LastLoginDate { get; set; }

        [Display(Name = "نوع کاربری")]
        public string UserTypeTitle { get; set; }

        [Display(Name = "ایجاد شده توسط")]
        public string CreatedByUserName { get; set; }

        [Display(Name = "ویرایش شده توسط")]
        public string EditedByUserName { get; set; }

        [Display(Name = "وضعیت ( فعال/غیرفعال)")]
        public IsActiveNameType IsActiveName { get; set; }
        public override void CustomMappings(IMappingExpression<SanjeshP.Mosahebe.Entities.User.User, UserDetailViewModel> mappingExpression)
        {
            mappingExpression.ForMember(
                dest => dest.CreatedByUserName,
                config => config.MapFrom(src => src.CreatorUser != null ? src.CreatorUser.UserName : string.Empty));

            mappingExpression.ForMember(
               dest => dest.EditedByUserName,
               config => config.MapFrom(src => src.EditorUser != null ? src.EditorUser.UserName : "- - - -"));
        }
    }
    public enum IsActiveNameType
    {
        [Display(Name = "غیر فعال")]
        Active = 0,
        [Display(Name = "فعال")]
        Inactive = 1,
    }

}
