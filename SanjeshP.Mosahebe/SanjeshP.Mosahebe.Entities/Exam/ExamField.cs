using SanjeshP.Mosahebe.Entities.Common;
using SanjeshP.Mosahebe.Entities.Interview;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Exam
{
    public class ExamField : BaseEntity<int>
    {
        [Required]
        public int StandardCode { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public bool IsPajoheshi { get; set; }

        // 1. کلید خارجی به جدول Exams
        [Required]
        public int ExamId { get; set; }

        // 2. navigation به آزمون مربوطه
        [ForeignKey(nameof(ExamId))]
        public Exams Exam { get; set; }

        public ICollection<FieldStation> FieldStations { get; set; } = new List<FieldStation>();
    }
}
