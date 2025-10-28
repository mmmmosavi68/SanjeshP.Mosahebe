using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Student
{
    public class ResearchDocumentScore : BaseEntity<int>
    {
        [ForeignKey(nameof(StudentId))]
        public int StudentId { get; set; }
        public double FinalTotalScore { get; set; }
        public virtual Student Student { get; set; }
    }
}
