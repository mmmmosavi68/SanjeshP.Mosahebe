using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Student
{
    public class FormSubmission : BaseEntity<int>   
    {
        public int FormId { get; set; }              // آی دی فرم
        public int FieldDefinitionId { get; set; }   // آی دی فیلد
        public DateTime SubmittedAt { get; set; }    // زمان ارسال
        public string Value { get; set; }            // مقدار فیلد (متن/عدد/فایل/چک‌باکس)
    }
}
