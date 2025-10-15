using SanjeshP.Mosahebe.WebFramework.Api;
using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.Interview.ViewModels.StationQuestion
{
    public class StationQuestionCreateViewModel : BaseDto<StationQuestionCreateViewModel, Entities.Interview.StationQuestion, int>
    {
        [Required(ErrorMessage = "متن سوال الزامی است")]
        [MaxLength(500, ErrorMessage = "حداکثر طول متن {1} کاراکتر است")]
        [Display(Name = "متن سؤال")]
        public string Text { get; set; }

        [Required(ErrorMessage = "حداکثر نمره الزامی است")]
        [Range(0, 10000, ErrorMessage = "حداکثر نمره باید بین {1} تا {2} باشد")]
        [Display(Name = "حداکثر نمره")]
        public decimal MaxValue { get; set; }

        [Required]
        public int StationId
        {
            get; set;
        }
    }
}