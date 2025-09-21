using SanjeshP.Mosahebe.Entities.Common;
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
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public int IsPajoheshi { get; set; }

        // 1. کلید خارجی به جدول Exams
        [Required]
        public int ExamId { get; set; }

        // 2. navigation به آزمون مربوطه
        [ForeignKey(nameof(ExamId))]
        public Exams Exam { get; set; }
        // ارتباط چند به چند با دانشجوها
        public ICollection<Student.StudentField> StudentFields { get; set; }
    }
}
