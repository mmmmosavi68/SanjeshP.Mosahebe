using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.StudentForm
{
    // ذخیره پاسخ‌های ارسال شده توسط کاربر
    public class StudentFormSubmission : BaseEntity<int>
    {
        public int FormId { get; set; }
        public StudentForms Form { get; set; }

        public int? StudentId { get; set; } // یا CandidateId

        // برای هر فیلد یک مقدار داریم
        public ICollection<StudentFormSubmissionValue> Values { get; set; }

        public int? Score { get; set; }
        public bool? IsApproved { get; set; }
        public string RejectReason { get; set; }

    }
}
