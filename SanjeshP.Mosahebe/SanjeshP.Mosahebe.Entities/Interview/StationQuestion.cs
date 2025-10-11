using SanjeshP.Mosahebe.Entities.Common;
using SanjeshP.Mosahebe.Entities.Exam;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Interview
{
    public class StationQuestion : BaseEntity<int>
    {
        public int StationID { get; set; }
        public ICollection<Station> Stations { get; set; }

        public int QuestionID { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}
