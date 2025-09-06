using System;
using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.Menu.ViewModels.Menu
{
    public class MenuEditViewModel
    {
        [Required(ErrorMessage = "شناسه منو الزامی است")]
        [Display(Name = "شناسه منو")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "عنوان الزامی است")]
        [MaxLength(100, ErrorMessage = "عنوان نباید بیش از 100 کاراکتر باشد")]
        [Display(Name = "عنوان")]
        public string MenuTitle { get; set; }

        [Display(Name = "عنوان والد")]
        public string Parent_title { get; set; }

        [Display(Name = "کد صفحه")]
        public int? PageCode { get; set; }

        [MaxLength(200, ErrorMessage = "مسیر نباید بیش از 200 کاراکتر باشد")]
        [Display(Name = "مسیر")]
        public string Path { get; set; }

        [MaxLength(100, ErrorMessage = "منطقه نباید بیش از 100 کاراکتر باشد")]
        [Display(Name = "منطقه")]
        public string Area { get; set; }

        [MaxLength(100, ErrorMessage = "نام کنترلر نباید بیش از 100 کاراکتر باشد")]
        [Display(Name = "نام کنترلر")]
        public string ControllerName { get; set; }

        [MaxLength(100, ErrorMessage = "نام اکشن نباید بیش از 100 کاراکتر باشد")]
        [Display(Name = "نام اکشن")]
        public string ActionName { get; set; }

        [Display(Name = "نمایش منو")]
        public bool ShowMenu { get; set; }

        [MaxLength(50, ErrorMessage = "کلاس CSS نباید بیش از 50 کاراکتر باشد")]
        [Display(Name = "کلاس CSS")]
        public string CssClass { get; set; }

        [MaxLength(100, ErrorMessage = "آیکن نباید بیش از 100 کاراکتر باشد")]
        [Display(Name = "آیکن")]
        public string Icon { get; set; }

        [MaxLength(200, ErrorMessage = "لینک نباید بیش از 200 کاراکتر باشد")]
        [Display(Name = "لینک")]
        public string Link { get; set; }
    }
}
