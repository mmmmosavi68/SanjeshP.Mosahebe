using SanjeshP.Mosahebe.WebFramework.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.Exam.ViewModels.Exam
{
    public class ExamCreateViewModel : BaseDto<ExamEditViewModel, SanjeshP.Mosahebe.Entities.Exam.Exams, Guid>
    {
        [Required(ErrorMessage = "عنوان آزمون را وارد نمایید.")]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required(ErrorMessage = "نوع آزمون را انتخاب نمایید.")]
        public Guid TypeOfExamId { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }


    }
}
