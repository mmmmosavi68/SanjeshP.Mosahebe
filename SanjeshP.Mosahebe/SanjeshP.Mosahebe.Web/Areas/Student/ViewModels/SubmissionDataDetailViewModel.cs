using SanjeshP.Mosahebe.Entities.StudentForm;
using SanjeshP.Mosahebe.WebFramework.Api;
using System;
using System.Collections.Generic;

namespace SanjeshP.Mosahebe.Web.Areas.Student.ViewModels
{
    public class SubmissionDataDetailViewModel :BaseDto<SubmissionDataDetailViewModel,StudentFormSubmissionScore,int>
    {
        /// شناسه فرم اصلی (مثلاً فرم اطلاعات زبان، فرم سوابق پژوهشی)
        public int FormItemId { get; set; }
        public FormItem FormItem { get; set; }

        /// شناسه دانشجو یا داوطلبی که این فرم را پر کرده
        public int? StudentId { get; set; }

        /// مجموعه مقادیر پر شده برای این ارسال (هر فیلد یک مقدار)
        public ICollection<StudentFormSubmissionData> StudentFormSubmissionDatas { get; set; }

    }
}
