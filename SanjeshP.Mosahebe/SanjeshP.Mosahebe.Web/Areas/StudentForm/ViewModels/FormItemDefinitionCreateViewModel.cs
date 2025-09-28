using SanjeshP.Mosahebe.Entities.StudentForm;
using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Presentation.ViewModels
{
    public class FormItemDefinitionCreateViewModel
    {
        public int Id { get; set; } // برای Edit

        [Required]
        public int FormItemId { get; set; } // ارتباط با FormItem

        [Required(ErrorMessage = "نام فیلد الزامی است")]
        public string Name { get; set; }

        [Required(ErrorMessage = "عنوان فیلد الزامی است")]
        public string Label { get; set; }

        [Required(ErrorMessage = "نوع فیلد الزامی است")]
        public InputType InputType { get; set; }

        public string InputTypeText { get; set; }

        public string Description { get; set; }

        public bool IsRequired { get; set; }

        public string ValidationPattern { get; set; }

        public string ErrorMessage { get; set; }
    }
}
