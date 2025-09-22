using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.StudentForm
{
    public enum FieldType
    {
        Text = 1,
        Number = 2,
        Url = 3,
        File = 4,
        Checkbox = 5,
        Date = 6
    }

    public class StudentFieldDefinition : BaseEntity<int>
    {
        public int FormId { get; set; }
        public StudentForms Form { get; set; }

        public string Name { get; set; }       // نام فیلد (کلید یکتا در فرم)
        public string Label { get; set; }      // عنوان قابل نمایش برای کاربر
        public FieldType Type { get; set; }    // نوع فیلد (متن، عدد، فایل و...)
        public bool IsRequired { get; set; }
        public string Description { get; set; }
        public string ValidationPattern { get; set; }  // الگوی اعتبارسنجی Regex (اختیاری)

    }
}
