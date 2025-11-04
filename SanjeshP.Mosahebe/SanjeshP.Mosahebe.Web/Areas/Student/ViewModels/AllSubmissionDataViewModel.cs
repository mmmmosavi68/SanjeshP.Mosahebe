using SanjeshP.Mosahebe.Entities.StudentForm;
using System;
using System.Collections.Generic;

namespace SanjeshP.Mosahebe.Web.Areas.Student.ViewModels
{
    public class AllSubmissionDataViewModel
    {
        public int SubmissionId { get; set; }
        public int FormItemId { get; set; }
        public string FormTitle { get; set; }      // عنوان فرم
        public string CreateDatePersian { get; set; }
        public int Order { get; set; }

        // مقادیر فرم
        public List<string> SubmissionValues { get; set; } = new();
    }
}
