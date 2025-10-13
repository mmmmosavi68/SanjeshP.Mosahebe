using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.StudentForm
{
    // ذخیره کل پاسخ‌های یک فرم توسط یک دانشجو
    public class StudentFormSubmissionScore : BaseEntity<int>
    {
        /// شناسه فرم اصلی (مثلاً فرم اطلاعات زبان، فرم سوابق پژوهشی)
        public int FormItemId { get; set; }
        public FormItem FormItem { get; set; }

        /// شناسه دانشجو یا داوطلبی که این فرم را پر کرده
        public int? StudentId { get; set; }

        /// مجموعه مقادیر پر شده برای این ارسال (هر فیلد یک مقدار)
        public ICollection<StudentFormSubmissionData> StudentFormSubmissionDatas { get; set; }

        /// نمره محاسبه‌شده برای این فرم (در صورت وجود فرایند محاسبه امتیاز)
        public double? Score { get; set; }

        /// وضعیت تایید/رد فرم توسط مدیر یا کارشناس
        public bool? IsApproved { get; set; }

        /// دلیل رد شدن فرم (اگر IsApproved = false)
        public string RejectReason { get; set; }
    }
}
