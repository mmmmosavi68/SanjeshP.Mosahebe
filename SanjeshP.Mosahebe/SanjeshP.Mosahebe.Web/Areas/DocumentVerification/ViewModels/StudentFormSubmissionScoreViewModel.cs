using SanjeshP.Mosahebe.Entities.StudentForm;
using System.Collections.Generic;

namespace SanjeshP.Mosahebe.Web.Areas.DocumentVerification.ViewModels
{
    public class StudentFormSubmissionScoreViewModel
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string NationalCode { get; set; }

        public string FielsTitle { get; set; }
        public List<StudentFormSubmissionScore> Forms { get; set; } = new List<StudentFormSubmissionScore>();
    }
}
