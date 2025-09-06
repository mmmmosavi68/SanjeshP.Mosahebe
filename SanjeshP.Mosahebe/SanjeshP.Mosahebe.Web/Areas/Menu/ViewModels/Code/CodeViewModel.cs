using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.Menu.ViewModels.Code
{
    public class CodeViewModel
    {
        public int Id { get; set; }

        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [Display(Name = "شناسه والد")]
        public int? ParentId { get; set; }

        [Display(Name = "کد استاندارد")]
        public int? StandardCode { get; set; }

        [Display(Name = "کد اصلی")]
        public int? PrimaryCode { get; set; }

        [Display(Name = "کد فرعی")]
        public int? SubCode { get; set; }

        [Display(Name = "اطلاعات اضافی")]
        public string AdditionalInformation { get; set; }

        [Display(Name = "تاریخ ایجاد")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "تاریخ به‌روزرسانی")]
        public DateTime? UpdatedDate { get; set; }

        [Display(Name = "وضعیت فعال بودن")]
        public bool IsActive { get; set; }

        [Display(Name = "وضعیت حذف")]
        public bool IsDeleted { get; set; }

        [Display(Name = "شناسه سازنده")]
        public Guid CreatedBy { get; set; }

        [Display(Name = "آی‌پی میزبان")]
        public string HostIp { get; set; }

        // برای نمایش Parent (در صورتی که نیاز باشد)
        [Display(Name = "والد")]
        public string ParentTitle { get; set; }

        // برای نمایش فرزندان (در صورتی که نیاز باشد)
        [Display(Name = "فرزندان")]
        public List<string> ChildTitles { get; set; } = new List<string>();
    }
}
