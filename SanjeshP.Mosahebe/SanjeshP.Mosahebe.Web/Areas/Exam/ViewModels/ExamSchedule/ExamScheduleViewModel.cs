using SanjeshP.Mosahebe.Common.Enums.ExamSchedule;
using SanjeshP.Mosahebe.Entities.Exam;
using SanjeshP.Mosahebe.WebFramework.Api;
using System;
using System.ComponentModel.DataAnnotations;

public class ExamScheduleViewModel : BaseDto<ExamScheduleViewModel, ExamSchedule, Guid>
{
    public Guid ExamScheduleId { get; set; }

    [Display(Name = "عنوان آزمون")]
    public string ExamTitle { get; set; }

    [Display(Name = "نوع رویداد")]
    public ExamEventType EventType { get; set; }

    [Display(Name = "زمان شروع")]
    public DateTime StartTime { get; set; }

    [Display(Name = "زمان پایان")]
    public DateTime EndTime { get; set; }

    [Display(Name = "فعال")]
    public bool IsActive { get; set; }
}
