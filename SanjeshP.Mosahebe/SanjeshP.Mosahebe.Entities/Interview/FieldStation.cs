using SanjeshP.Mosahebe.Entities.Common;
using SanjeshP.Mosahebe.Entities.Exam;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Interview
{
    public class FieldStation : BaseEntity<int >
    {
        public int ExamFieldID { get; set; }
        public ICollection<ExamField> ExamFields { get; set; }

        public int StationID { get; set; }
        public ICollection<Station> Stations { get; set; }
    }
}
