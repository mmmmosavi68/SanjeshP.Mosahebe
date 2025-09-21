using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Student
{
    public class StudentField: BaseEntity<int>
    {
        public int RegId { get; set; }

        public int StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; }

        public int ExamFieldId { get; set; }
        [ForeignKey(nameof(ExamFieldId))]
        public Exam.ExamField ExamField { get; set; }
    }
}
