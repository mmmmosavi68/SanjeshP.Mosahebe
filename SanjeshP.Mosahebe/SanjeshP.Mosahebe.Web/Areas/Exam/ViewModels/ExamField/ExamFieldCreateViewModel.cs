using SanjeshP.Mosahebe.WebFramework.Api;
using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.Exam.ViewModels.ExamField
{
    public class ExamFieldCreateViewModel : BaseDto<ExamFieldCreateViewModel, Entities.Exam.ExamField, int>
    {
        [Required]
        public int ExamId { get; set; }

        [Required]
        public int StandardCode { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }
    }
}
