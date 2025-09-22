using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.StudentForm
{
    public class StudentForms : BaseEntity<int>
    {
        public string Title { get; set; }      // عنوان فرم
        public string Description { get; set; }  // توضیح کوتاه (اختیاری)
        public ICollection<StudentFieldDefinition> Fields { get; set; }

    }
}
