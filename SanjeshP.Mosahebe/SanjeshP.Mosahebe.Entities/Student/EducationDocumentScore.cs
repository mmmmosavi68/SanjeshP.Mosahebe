using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Student
{
    public class EducationDocumentScore : BaseEntity<int>
    {
        public int StudentId { get; set; }
        public double FinalTotalScore { get; set; }
    }
}
