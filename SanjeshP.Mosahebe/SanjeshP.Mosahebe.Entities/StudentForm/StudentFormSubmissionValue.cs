using SanjeshP.Mosahebe.Entities.Common;
using SanjeshP.Mosahebe.Entities.Student;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.StudentForm
{
    // مقدار هر فیلد در یک ارسال فرم
    public class StudentFormSubmissionValue : BaseEntity<int>
    {
        public int FormSubmissionId { get; set; }
        public StudentFormSubmission FormSubmission { get; set; }

        public FieldType Type { get; set; }    // نوع فیلد (متن، عدد، فایل و...)

        public int FieldDefinitionId { get; set; }
        public StudentFieldDefinition FieldDefinition { get; set; }

        public string Value { get; set; }   // مقدار ذخیره شده (متن/آدرس فایل/true/false و...)

    }
}
