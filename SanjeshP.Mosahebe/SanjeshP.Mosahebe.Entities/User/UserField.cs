using SanjeshP.Mosahebe.Entities.Common;
using SanjeshP.Mosahebe.Entities.Exam;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.User
{
    public class UserField : BaseEntity<int>
    {
        public  Guid UserID { get; set; }

        [ForeignKey(nameof(UserID))]
        public User Users { get; set; }

        public int ExamFieldID { get; set; }
        public ExamField ExamFields { get; set; }
    }
}
