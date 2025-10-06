using SanjeshP.Mosahebe.Entities.StudentForm;
using System;
using System.Collections.Generic;

namespace SanjeshP.Mosahebe.Web.Areas.Student.ViewModels
{
    public class SubmissionDataDetailViewModel
    {
        // فیلدهای جدول اصلی
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public int FormItemId { get; set; }

        // فیلدهای جدول FormItem
        public string FormTitle { get; set; }
        public string FormDescription { get; set; }

        // فیلدهای جدول StudentFormSubmissionData
        public string SubmissionDataValue { get; set; }
        public string SubmissionDataInputType { get; set; }
    }
}
