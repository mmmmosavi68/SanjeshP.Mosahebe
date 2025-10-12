using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.File
{
    public class UploadedFile : BaseEntity<int>
    {
        // شناسه کاربر (دانشجو)
        [Required]
        public int StudentId { get; set; }

        // شناسه Submission مرتبط
        public int? StudentFormSubmissionScoreId { get; set; }
        // شناسه Submission مرتبط
        public int? StudentFormSubmissionDataId { get; set; }

        // نام ایمن ذخیره‌شده در دیسک
        [Required]
        [MaxLength(200)]
        public string StoredFileName { get; set; }

        // نام اصلی فایل آپلود شده
        [MaxLength(500)]
        public string OriginalFileName { get; set; }

        // نوع محتوا (MIME type)
        [MaxLength(100)]
        public string ContentType { get; set; }

        // اندازه به بایت
        public long Size { get; set; }

        // زمان آپلود
        public DateTime UploadedAt { get; set; }

        // در صورت نیاز می‌توانید مسیر مجازی یا تگ‌های دیگر اضافه کنید
        [MaxLength(1000)]
        public string? RelativePath { get; set; }
    }
}
