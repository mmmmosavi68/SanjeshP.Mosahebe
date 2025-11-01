using SanjeshP.Mosahebe.Entities.Common;
using SanjeshP.Mosahebe.Entities.StudentForm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Interview
{
    public class InterviewScore:BaseEntity<int>
    {
        public Guid UserId { get; set; }
        public int StudentId { get; set; }
        public int StationId { get; set; }
        public int QuestionId { get; set; }
        [Column(TypeName = "decimal(6,3)")]
        public double? QuestionMaxValue { get; set; }
        /// امتیاز درج شده توسط استاد برای سوال در مصاحبه
        [Column(TypeName = "decimal(6,3)")]
        public double? QuestionScore { get; set; }
        //امتیاز محاسبه شده در مصاحبه برای هر سوال بر اساس امتیاز هر سوال و امتیازی که استاد وارد کرده است
        [Column(TypeName = "decimal(6,3)")]
        public double? CalculatedScore { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User.User User { get; set; }

        [ForeignKey(nameof(StudentId))]
        public virtual Student.Student Student { get; set; }


    }
}
