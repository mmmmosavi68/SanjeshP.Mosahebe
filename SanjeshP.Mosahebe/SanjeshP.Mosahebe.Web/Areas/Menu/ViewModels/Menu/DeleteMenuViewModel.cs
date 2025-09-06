using System;
using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.Menu.ViewModels.Menu
{
    public class DeleteMenuViewModel
    {
        [Required(ErrorMessage = "شناسه منو الزامی است")]
        [Display(Name = "شناسه منو")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "عنوان الزامی است")]
        [MaxLength(100, ErrorMessage = "عنوان نباید بیش از 100 کاراکتر باشد")]
        [Display(Name = "عنوان")]
        public string MenuTitle { get; set; }
    }
}
