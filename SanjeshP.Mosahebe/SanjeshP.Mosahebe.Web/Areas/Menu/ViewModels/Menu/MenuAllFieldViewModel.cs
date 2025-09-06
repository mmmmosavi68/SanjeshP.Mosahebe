using System;
using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.Menu.ViewModels.Menu
{
    public class MenuAllFieldViewModel
    {
        // از BaseEntity ارث می‌بریم
        public Guid Id { get; set; }

        // اطلاعات منوی اصلی
        [Required(ErrorMessage = "عنوان الزامی است")]
        [MaxLength(100, ErrorMessage = "عنوان نباید بیش از 100 کاراکتر باشد")]
        public string MenuTitle { get; set; }

        public Guid? ParentId { get; set; }

        public int? PageCode { get; set; }

        [MaxLength(200, ErrorMessage = "مسیر نباید بیش از 200 کاراکتر باشد")]
        public string Path { get; set; }

        [MaxLength(100, ErrorMessage = "منطقه نباید بیش از 100 کاراکتر باشد")]
        public string Area { get; set; }

        [MaxLength(100, ErrorMessage = "نام کنترلر نباید بیش از 100 کاراکتر باشد")]
        public string ControllerName { get; set; }

        [MaxLength(100, ErrorMessage = "نام اکشن نباید بیش از 100 کاراکتر باشد")]
        public string ActionName { get; set; }

        public bool? ShowMenu { get; set; }

        [MaxLength(50, ErrorMessage = "کلاس CSS نباید بیش از 50 کاراکتر باشد")]
        public string CssClass { get; set; }

        [MaxLength(100, ErrorMessage = "آیکن نباید بیش از 100 کاراکتر باشد")]
        public string Icon { get; set; }

        [MaxLength(200, ErrorMessage = "لینک نباید بیش از 200 کاراکتر باشد")]
        public string Link { get; set; }

        // فیلدهای مربوط به وضعیت
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        // تاریخ‌ها برای نمایش
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // شناسه سازنده و آی‌پی میزبان
        public Guid CreatedBy { get; set; }
        public string HostIp { get; set; }
    }
}