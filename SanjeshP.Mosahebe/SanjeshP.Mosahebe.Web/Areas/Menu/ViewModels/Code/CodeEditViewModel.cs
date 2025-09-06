using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.Menu.ViewModels.Code
{
    public class CodeEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "عنوان الزامی است")]
        [MaxLength(100, ErrorMessage = "عنوان نباید بیش از 100 کاراکتر باشد")]
        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [Display(Name = "کد استاندارد")]
        public int? StandardCode { get; set; }

        [Display(Name = "کد اصلی")]
        public int? PrimaryCode { get; set; }

        [Display(Name = "کد فرعی")]
        public int? SubCode { get; set; }

        [MaxLength(200, ErrorMessage = "اطلاعات اضافی نباید بیش از 200 کاراکتر باشد")]
        [Display(Name = "اطلاعات اضافی")]
        public string AdditionalInformation { get; set; }

        [Display(Name = "وضعیت فعال بودن")]
        public bool IsActive { get; set; }
    }
}