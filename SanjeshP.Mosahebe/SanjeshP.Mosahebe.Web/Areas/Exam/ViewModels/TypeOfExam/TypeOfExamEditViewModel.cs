using SanjeshP.Mosahebe.WebFramework.Api;
using System;
using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.Exam.ViewModels.TypeOfExam
{
    public class TypeOfExamEditViewModel : BaseDto<TypeOfExamEditViewModel, SanjeshP.Mosahebe.Entities.Exam.TypeOfExam, Guid>
    {
        [Required]
        public Guid TypeOfExamId { get; set; }

        [Required(ErrorMessage = "{0} را وارد نمایید.")]
        [Display(Name = "کد استاندارد")]
        public int StandardCode { get; set; }

        [Required(ErrorMessage = "{0} را وارد نمایید.")]
        [MaxLength(100, ErrorMessage = "{0} نمی‌تواند بیشتر از 100 کاراکتر باشد.")]
        [Display(Name = "عنوان آزمون")]
        public string Title { get; set; }

        [MaxLength(500, ErrorMessage = "{0} نمی‌تواند بیشتر از 500 کاراکتر باشد.")]
        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        [Display(Name = "فعال")]
        public bool IsActive { get; set; }
    }
}