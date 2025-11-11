using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SanjeshP.Mosahebe.Presentation.ViewModels
{
    public class FormItemViewModel
    {
        public int Id { get; set; }   // برای Edit و Delete
        public string Title { get; set; }
        public string Description { get; set; }
        public string PrivateDescription { get; set; }
        public double OneMaxScore { get; set; }
        public double AllMaxScore { get; set; }
        public bool IsActive { get; set; }

        // Foreign Keys
        public int FormItemCoefficientId { get; set; }
        public int FormItemCategoryId { get; set; }

        // برای نمایش نام‌ها در جدول
        public string CoefficientTitle { get; set; }
        public string CategoryTitle { get; set; }

        // برای Dropdown
        public IEnumerable<SelectListItem> Coefficients { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}
