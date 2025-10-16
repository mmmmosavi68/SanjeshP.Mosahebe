using SanjeshP.Mosahebe.WebFramework.Api;
using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.Exam.ViewModels.ExamField
{
    public class ExamFieldEditViewModel:BaseDto<ExamFieldEditViewModel, Entities.Exam.ExamField,int>
    {
        [Required]
        public int ExamId { get; set; }
        [Required]
        public int StandardCode { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }
    }
}
