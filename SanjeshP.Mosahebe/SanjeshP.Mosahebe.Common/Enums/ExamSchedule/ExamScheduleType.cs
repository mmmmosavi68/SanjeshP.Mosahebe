using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SanjeshP.Mosahebe.Common.Enums.ExamSchedule
{
    public enum ExamEventType
    {
        [Display(Name = "ثبت‌نام")]
        Registration = 10,

        [Display(Name = "تاریخ برگزاری آزمون")]
        ExamDate = 11,

        [Display(Name = "اعتراض")]
        Objection = 12,

        [Display(Name = "اعلام نتایج")]
        ResultPublis = 13
    }
}
