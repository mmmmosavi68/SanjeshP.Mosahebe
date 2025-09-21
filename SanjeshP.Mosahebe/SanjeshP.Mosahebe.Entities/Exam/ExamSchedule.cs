using SanjeshP.Mosahebe.Entities.Common;
using SanjeshP.Mosahebe.Common.Enums.ExamSchedule;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Exam
{
    public class ExamSchedule : BaseEntity<int>
    {
        // شناسه آزمون
        [Required]
        public int ExamId { get; set; }

        // شیء آزمون مرتبط
        [ForeignKey(nameof(ExamId))]
        public Exams Exam { get; set; }

        // نوع زمان‌بندی (از نوع enum بالا)
        [Required]
        public ExamEventType EventType { get; set; }

        // ✅ نام انگلیسی enum (رشته‌ای)
        public string EventTypeName { get; set; }

        // ✅ نام فارسی enum (از DisplayAttribute)
        public string EventTypeDisplayName { get; set; }

        [Required]
        [Display(Name = "زمان شروع رویداد")]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }

        [Required]
        [Display(Name = "زمان پایان رویداد")]
        [DataType(DataType.DateTime)]
        public DateTime EndTime { get; set; }
    }

}
