using System;

namespace SanjeshP.Mosahebe.Web.Areas.Student.ViewModels
{
    public class FormSubmissionViewModel
    {
        public string FieldLabel { get; set; }    // Label فیلد
        public string Value { get; set; }         // مقدار ثبت شده
        public DateTime SubmittedAt { get; set; } // زمان ارسال
    }
}
