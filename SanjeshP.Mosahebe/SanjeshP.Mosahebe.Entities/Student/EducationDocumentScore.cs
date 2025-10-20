using SanjeshP.Mosahebe.Entities.Common;
using SanjeshP.Mosahebe.Entities.Exam;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Student
{
    public class EducationDocumentScore : BaseEntity<int>
    {
        [ForeignKey(nameof(StudentId))]
        public int StudentId { get; set; }
        public double FinalTotalScore { get; set; }
    }
}
