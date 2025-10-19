using System.Collections.Generic;

namespace SanjeshP.Mosahebe.Web.Areas.Interview.ViewModels.StudentInterview
{
    public class StudentInterviewQuestionViewModel
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string NationalCode { get; set; }

        public int StationId { get; set; }
        public string StationName { get; set; }

        public List<QuestionScoreViewModel> Questions { get; set; }
    }

    public class QuestionScoreViewModel
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public decimal MaxValue { get; set; }
        public int? SelectedScore { get; set; }
    }
}
