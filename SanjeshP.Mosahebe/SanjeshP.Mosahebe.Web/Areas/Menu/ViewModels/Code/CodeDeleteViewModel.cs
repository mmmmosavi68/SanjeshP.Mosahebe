using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.Menu.ViewModels.Code
{
    public class CodeDeleteViewModel
    {
        public int Id { get; set; }

        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [Display(Name = "آیا از حذف این کد مطمئن هستید؟")]
        public bool ConfirmDelete { get; set; }
    }

}
