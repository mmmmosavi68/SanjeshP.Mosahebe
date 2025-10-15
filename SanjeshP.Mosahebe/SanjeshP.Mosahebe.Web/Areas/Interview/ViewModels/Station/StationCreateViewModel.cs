using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.Interview.ViewModels.Station
{
    public class StationCreateViewModel
    {
        [Required, MaxLength(200)]
        [Display(Name = "نام ایستگاه")]
        public string Name { get; set; }
    }
}
