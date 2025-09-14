using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Exam
{
    public class Exams : BaseEntity
    {
        // عنوان آزمون (مثلاً آزمون دکتری 1404)
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        // شناسه نوع آزمون (ارجاع به TypeOfExam)
        [Required]
        public Guid TypeOfExamId { get; set; }

        // توضیحات کلی درباره این آزمون
        [MaxLength(1000)]
        public string Description { get; set; }

        // لیست زمان‌بندی‌های مربوط به آزمون
        public ICollection<ExamSchedule> Schedules { get; set; }
        public ICollection<ExamField> Fields { get; set; }
    }

}
