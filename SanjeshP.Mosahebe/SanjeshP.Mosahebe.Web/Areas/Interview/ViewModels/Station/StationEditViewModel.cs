using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.Interview.ViewModels.Station
{
    public class StationEditViewModel
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        [Display(Name = "نام ایستگاه")]
        public string Name { get; set; }

        [Display(Name = "فعال")]
        public bool IsActive { get; set; }
    }
}
