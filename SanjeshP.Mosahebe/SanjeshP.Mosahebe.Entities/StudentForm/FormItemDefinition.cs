using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.StudentForm
{
    public enum InputType
    {
        Text = 1,
        Number = 2,
        Url = 3,
        File = 4,
        Checkbox = 5,
        Date = 6,
        Dropdown = 7
    }

    public class FormItemDefinition : BaseEntity<int>
    {
        public string Name { get; set; }       // نام فیلد (کلید یکتا در فرم)
        public string Label { get; set; }      // عنوان قابل نمایش برای کاربر
        public InputType inputType { get; set; }    // نوع فیلد (متن، عدد، فایل و...)
        public string InputTypeText { get; set; }
        public bool IsRequired { get; set; }
        public string Description { get; set; }
        public string ValidationPattern { get; set; }  // الگوی اعتبارسنجی Regex (اختیاری)
        public string ErrorMessage { get; set; }

        // Foreign Key
        public int FormItemId { get; set; }
        // Navigation Property
        public FormItem FormItem { get; set; }

    }
}
