using SanjeshP.Mosahebe.WebFramework.Api;
using System;
using System.Collections.Generic;

namespace SanjeshP.Mosahebe.Web.Areas.Exam.ViewModels.Exam
{
    public class ExamViewModel : BaseDto<ExamViewModel, SanjeshP.Mosahebe.Entities.Exam.Exams, int>
    {
        public int ExamId { get; set; }
        public string Title { get; set; }
        public int TypeOfExamId { get; set; }
        public string TypeOfExamTitle { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
