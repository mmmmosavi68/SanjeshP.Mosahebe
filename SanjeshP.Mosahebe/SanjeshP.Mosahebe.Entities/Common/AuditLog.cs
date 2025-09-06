using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SanjeshP.Mosahebe.Entities.Common
{
    public class AuditLog : IEntity
    {
        [Key]
        public Guid AuditLogId { get; set; }

        [Required(ErrorMessage = "شناسه اصلی رکورد الزامی است")]
        [Display(Name = "شناسه اصلی رکورد")]
        public Guid EntityId { get; set; }

        [Required(ErrorMessage = "نام جدول الزامی است")]
        [MaxLength(100, ErrorMessage = "نام جدول نباید بیش از 100 کاراکتر باشد")]
        [Display(Name = "نام جدول")]
        public string TableName { get; set; }

        [Required(ErrorMessage = "نوع عملیات الزامی است")]
        [MaxLength(50, ErrorMessage = "نوع عملیات نباید بیش از 50 کاراکتر باشد")]
        [Display(Name = "نوع عملیات")]
        public string OperationType { get; set; } // Create, Update, Delete

        [Required(ErrorMessage = "زمان عملیات الزامی است")]
        [Display(Name = "زمان عملیات")]
        public DateTime OperationTime { get; set; }

        [Required(ErrorMessage = "شناسه کاربر الزامی است")]
        [Display(Name = "شناسه کاربر")]
        public Guid UserId { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        [Display(Name = "مقدار قبلی")]
        public string OldValue { get; set; }

        [Display(Name = "مقدار جدید")]
        public string NewValue { get; set; }
    }
}
