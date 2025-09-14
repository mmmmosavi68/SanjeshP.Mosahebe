using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Student
{
    public class FormSubmission : BaseEntity<int>   
    {
        public int FormId { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string JsonData { get; set; }    // مقادیر فیلدها به‌صورت JSON
    }
}
