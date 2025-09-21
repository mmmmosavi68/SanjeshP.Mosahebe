using Microsoft.AspNetCore.Mvc.Rendering;
using SanjeshP.Mosahebe.Web.Areas.Exam.ViewModels.Exam;
using SanjeshP.Mosahebe.Common.Enums.ExamSchedule;
using SanjeshP.Mosahebe.Entities.Exam;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.Exam.ViewModels.ExamSchedule
{
    public class ExamScheduleCreateViewModel
    {
        [Required(ErrorMessage = "انتخاب آزمون الزامی است.")]
        [Display(Name = "آزمون")]
        public int ExamId { get; set; }

        [Required(ErrorMessage = "نوع رویداد الزامی است.")]
        [Display(Name = "نوع رویداد")]
        public ExamEventType EventType { get; set; }

        [Required(ErrorMessage = "تاریخ و زمان شروع الزامی است.")]
        [Display(Name = "زمان شروع")]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "تاریخ و زمان پایان الزامی است.")]
        [Display(Name = "زمان پایان")]
        [DataType(DataType.DateTime)]
        public DateTime EndTime { get; set; }

        public List<ExamViewModel> ExamList { get; set; }

        public List<SelectListItem> EventTypeSelectList { get; set; }

        [Display(Name = "فعال")]
        public bool IsActive { get; set; }

    }
}