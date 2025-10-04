using SanjeshP.Mosahebe.Entities.Common;
using SanjeshP.Mosahebe.Entities.Student;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.StudentForm
{
    // مقدار هر فیلد در یک ارسال فرم (مثلاً نمره زبان، فایل رزومه، چک‌باکس تاییدیه و ...)
    public class StudentFormSubmissionData : BaseEntity<int>
    {
        /// نوع فیلد (Text, Number, File, Checkbox, Dropdown, ...)
        public InputType InputType { get; set; }

        /// مقدار ذخیره‌شده
        /// - برای Text/Number → متن یا عدد
        /// - برای File → آدرس یا نام فایل
        /// - برای Checkbox → true/false
        /// - برای Dropdown → مقدار انتخاب شده (Value)
        public string Value { get; set; }

        /// رفرنس به رکورد اصلی StudentFormSubmission
        /// هر ارسال یک فرم شامل چندین مقدار (Value) می‌شود.
        public int StudentFormSubmissionScoreId { get; set; }
        public StudentFormSubmissionScore StudentFormSubmissionScore  { get; set; }

        /// شناسه تعریف فیلد (کدوم فیلد بوده؟ معدل، زبان، کتاب و ...)
        public int? FormItemDefinitionId { get; set; }
        public FormItemDefinition FormItemDefinition { get; set; }


        public int? FormItemCoefficientDefinitionId { get; set; }
        public FormItemCoefficientDefinition FormItemCoefficientDefinition { get; set; }
    }

}
