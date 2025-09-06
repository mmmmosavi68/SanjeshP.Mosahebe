using System;
using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.Menu.ViewModels.Menu
{
    public class MenuListViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "عنوان")]
        public string MenuTitle { get; set; }

        [Display(Name = "شناسه والد")]
        public Guid? ParentId { get; set; }

        [Display(Name = "کد صفحه")]
        public int? PageCode { get; set; }

        [Display(Name = "مسیر")]
        public string Path { get; set; }

        [Display(Name = "منطقه")]
        public string Area { get; set; }

        [Display(Name = "نام کنترلر")]
        public string ControllerName { get; set; }

        [Display(Name = "نام اکشن")]
        public string ActionName { get; set; }

        [Display(Name = "نمایش منو")]
        public bool? ShowMenu { get; set; }

        [Display(Name = "کلاس CSS")]
        public string CssClass { get; set; }

        [Display(Name = "آیکن")]
        public string Icon { get; set; }

        [Display(Name = "لینک")]
        public string Link { get; set; }
    }
}