using SanjeshP.Mosahebe.Entities.Common;
using SanjeshP.Mosahebe.Entities.Exam;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SanjeshP.Mosahebe.Entities.Interview;

namespace SanjeshP.Mosahebe.Entities.Student
{
    public enum InterviewProcessStatus
    {
        NotStarted = 0,   // هنوز شروع نکرده
        Active = 1,       // فعال شده برای مصاحبه
        Completed = 2     // مصاحبه تمام شده
    }

    public class Student : BaseEntity<int>
    {
        [Required]
        public int RegId { get; set; }

        [Required, MaxLength(200)]
        public string FirstName { get; set; }

        [Required, MaxLength(200)]
        public string LastName { get; set; }

        [MaxLength(200)]
        public string FatherName { get; set; }

        [MaxLength(10), RegularExpression(@"^\d{10}$")]
        public string NationalCode { get; set; }

        [MaxLength(20)]
        public string CodeRahgiri { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [EmailAddress, MaxLength(250)]
        public string Email { get; set; }

        public int Gender { get; set; }
        public bool IsPajoheshi { get; set; }

        public bool? IsApproved { get; set; }

        /// دلیل رد شدن فرم (اگر IsApproved = false)
        public string RejectReason { get; set; }

        public InterviewProcessStatus InterviewProcessStatus { get; set; } = InterviewProcessStatus.NotStarted;

        // 1. کلید خارجی به جدول Exams
        [Required]
        public int ExamId { get; set; }

        // 2. navigation به آزمون مربوطه
        [ForeignKey(nameof(ExamId))]
        public Exams Exam { get; set; }

        // ارتباط چند به چند با رشته‌ها
        public ICollection<StudentField> StudentFields { get; set; } = new List<StudentField>();
        public virtual ICollection<InterviewScore> InterviewScores { get; set; }
        public virtual ICollection<EducationDocumentScore> EducationDocumentScores { get; set; }
        //نوع دوره آزمون (آموزشی یا پژوهشی
        //public int ExamCourseType { get; set; } 

    }
}
