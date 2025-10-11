using SanjeshP.Mosahebe.Entities.Common;
using SanjeshP.Mosahebe.Entities.Exam;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.User
{
    public class UserField : BaseEntity<int>
    {
        public  Guid UserID { get; set; }
        public ICollection<User> Users { get; set; }

        public int ExamFieldID { get; set; }
        public ICollection<ExamField> ExamFields { get; set; }
    }
}
