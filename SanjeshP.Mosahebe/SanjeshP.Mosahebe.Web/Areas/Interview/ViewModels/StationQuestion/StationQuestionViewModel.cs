using SanjeshP.Mosahebe.Entities.Interview;
using SanjeshP.Mosahebe.WebFramework.Api;

namespace SanjeshP.Mosahebe.Web.Areas.Interview.ViewModels.StationQuestion
{
    public class StationQuestionViewModel : BaseDto<StationQuestionViewModel, Entities.Interview.StationQuestion,int>
    {
        public string Text { get; set; }

        public decimal MaxValue { get; set; }

        public int StationId { get; set; }
    }
}
