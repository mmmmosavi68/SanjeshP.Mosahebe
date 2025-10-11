using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Interview
{
    public class QuestionAnswer : BaseEntity<int>
    {
        public int StudentID { get; set; }
        public ICollection<SanjeshP.Mosahebe.Entities.Student.Student> Students { get; set; }

        public Guid UserID { get; set; }
        public ICollection<User.User> Users { get; set; }

        public int StationID { get; set; }
        public ICollection<Station> Stations { get; set; }

        public int QuestionID { get; set; }
        public ICollection<Question> Questions { get; set; }

        public float Value { get; set; }
    }
}
