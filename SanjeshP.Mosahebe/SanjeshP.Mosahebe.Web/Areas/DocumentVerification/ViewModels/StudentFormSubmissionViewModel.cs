using SanjeshP.Mosahebe.Entities.Student;
using SanjeshP.Mosahebe.Entities.StudentForm;
using System.Collections.Generic;

namespace SanjeshP.Mosahebe.Web.Areas.DocumentVerification.ViewModels
{

    public class StudentFormSubmissionViewModel
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string NationalCode { get; set; }

        public string FielsTitle { get; set; }
        public List<StudentFormSubmission> Forms { get; set; } = new List<StudentFormSubmission>();

        // اضافه‌شده:
        public StudentStatus? Status { get; set; }
        public decimal? FinalScore { get; set; }
        public string RejectReason { get; set; }
    }
}
