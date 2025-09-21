using SanjeshP.Mosahebe.Entities.Common;
using SanjeshP.Mosahebe.Entities.Exam;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Student
{
    public class Student : BaseEntity<int>
    {
        public int RegId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public string NationalCode { get; set; }
        public string CodeRahgiri { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int Gender { get; set; }
        public int IsPajoheshi { get; set; }

        // 1. کلید خارجی به جدول Exams
        [Required]
        public int ExamId { get; set; }

        // 2. navigation به آزمون مربوطه
        [ForeignKey(nameof(ExamId))]
        public Exams Exam { get; set; }

        // ارتباط چند به چند با رشته‌ها
        public ICollection<StudentField> StudentFields { get; set; } = new List<StudentField>();

        //نوع دوره آزمون (آموزشی یا پژوهشی
        public int ExamCourseType { get; set; } 

    }
}
