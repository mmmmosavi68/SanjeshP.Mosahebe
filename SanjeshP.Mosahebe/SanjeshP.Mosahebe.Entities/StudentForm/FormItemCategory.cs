using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.StudentForm
{
    // نمایش دسته بندی آیتم های فرم
    public class FormItemCategory : BaseEntity<int>
    {
        public string Title { get; set; }
        public int MaxScore { get; set; }
        public int MaxCount { get; set; }

        public ICollection<FormItem> FormItem { get; set; }

        // Foreign Key
        public int FormCourseCategoryId { get; set; }

        // Navigation Property
        public FormCourseCategory FormCourseCategory { get; set; }
    }
}
