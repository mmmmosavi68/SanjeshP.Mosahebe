using SanjeshP.Mosahebe.Entities.Common;
using SanjeshP.Mosahebe.Entities.Exam;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Interview
{
    public class FieldStation : BaseEntity<int>
    {
        public int ExamFieldId { get; set; }
        [ForeignKey(nameof(ExamFieldId))]
        public Exam.ExamField ExamField { get; set; }

        public int StationId { get; set; }
        [ForeignKey(nameof(StationId))]
        public Station Station { get; set; }
    }
}
