using SanjeshP.Mosahebe.WebFramework.Api;

namespace SanjeshP.Mosahebe.Web.Areas.Exam.ViewModels.ExamField
{
    public class ExamFieldViewModel : BaseDto<ExamFieldViewModel, Entities.Exam.ExamField, int>
    {
        public int StandardCode { get; set; }
        public string Title { get; set; }
        public string ExamTitle { get; set; }
        public bool IsActive { get; set; }
    }
}
