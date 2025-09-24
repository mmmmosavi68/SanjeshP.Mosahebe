using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.StudentForm
{

    // دسته بندی آینم های فرم بر اساس آموزشی و پژوهشی
    public class FormCourseCategory:BaseEntity<int>
    {
        public string Title { get; set; }
        public int MaxScore { get; set; }

        public ICollection<FormItemCategory> FormItemCategory { get; set; }
    }
}
