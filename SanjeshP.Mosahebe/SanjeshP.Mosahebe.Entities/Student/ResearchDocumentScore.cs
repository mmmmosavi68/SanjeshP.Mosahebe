using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Student
{
    public enum StudentStatus
    {
        [Display(Name ="غایب")]
        Absent = 0,    // غایب
        [Display(Name = "عدم تأیید")]
        Rejected = 1,  // عدم تأیید
        [Display(Name = "تأیید")]
        Approved = 2   // تأیید
    }
    public class ResearchDocumentScore : BaseEntity<int>
    {
        [ForeignKey(nameof(StudentId))]
        public int StudentId { get; set; }
        [Display(Name = "نمره نهایی")]
        [Column(TypeName = "decimal(6,3)")]
        public double FinalTotalScore { get; set; }
        /// وضعیت تایید/رد فرم توسط مدیر یا کارشناس
        [Display(Name = "وضعیت")]
        public StudentStatus? IsApproved { get; set; }

        /// دلیل رد شدن فرم (اگر IsApproved = false)
        [Display(Name = "علت عدم تایید")]
        public string RejectReason { get; set; }

        public virtual Student Student { get; set; }
    }
}
