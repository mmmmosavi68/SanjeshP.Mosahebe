using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.StudentForm
{
    public class StudentFormSubmissionScore : BaseEntity<int>
    {
        public int StudentFormSubmissionId { get; set; }
        public Guid UserId { get; set; }
        /// شناسه دانشجو یا داوطلبی که این فرم را پر کرده
        public int StudentId { get; set; }
        /// وضعیت تایید/رد فرم توسط مدیر یا کارشناس
        public bool IsApproved { get; set; }
        /// دلیل رد شدن فرم (اگر IsApproved = false)
        public string RejectReason { get; set; }

        [Column(TypeName = "decimal(6,3)")]
        public double? UserScore { get; set; }
        /// نمره محاسبه‌شده برای این فرم (در صورت وجود فرایند محاسبه امتیاز)
        [Column(TypeName = "decimal(6,3)")]
        public double CalculatedScore { get; set; }

        public StudentFormSubmission StudentFormSubmission { get; set; }
    }
}
