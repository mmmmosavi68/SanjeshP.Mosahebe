using SanjeshP.Mosahebe.Web.Areas.Exam.ViewModels.TypeOfExam;
using SanjeshP.Mosahebe.WebFramework.Api;
using System;
using System.Collections.Generic;

namespace SanjeshP.Mosahebe.Web.Areas.Exam.ViewModels.Exam
{
    public class ExamViewModel : BaseDto<ExamViewModel, SanjeshP.Mosahebe.Entities.Exam.Exams, Guid>
    {
        public Guid ExamId { get; set; }
        public string Title { get; set; }
        public Guid TypeOfExamId { get; set; }
        public string TypeOfExamTitle { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
