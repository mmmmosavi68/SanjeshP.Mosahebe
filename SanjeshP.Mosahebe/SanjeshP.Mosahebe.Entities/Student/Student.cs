using SanjeshP.Mosahebe.Entities.Common;
using SanjeshP.Mosahebe.Entities.Exam;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Student
{
    public class Student : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NationalCode { get; set; }
        public string CodeRahgiri { get; set; }
        public string PhonNumber { get; set; }
        public string Email { get; set; }
        public bool Gender { get; set; }

        // 1. کلید خارجی به جدول Exams
        [Required]
        public Guid ExamId { get; set; }

        // 2. navigation به آزمون مربوطه
        [ForeignKey(nameof(ExamId))]
        public Exams Exam { get; set; }

        // 3. کلید خارجی به جدول Field
        [Required]
        public Guid FieldId { get; set; }

        // 4. navigation به آزمون مربوطه
        [ForeignKey(nameof(FieldId))]
        public Field Field { get; set; }
    }
}
