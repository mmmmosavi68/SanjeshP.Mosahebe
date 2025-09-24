using SanjeshP.Mosahebe.Entities.Common;
using System.Collections.Generic;

namespace SanjeshP.Mosahebe.Entities.StudentForm
{
    public enum FormSourceType
    {
        None = 0,
        AvgNumber = 1,
        Book = 2,
        Language = 3,
        Partner = 4
    }

    public class FormItem : BaseEntity<int>
    {
        public string Title { get; set; }      // عنوان فرم
        public string Description { get; set; }  // توضیح کوتاه (اختیاری)
        public int OneMaxScore { get; set; }
        public int AllMaxScore { get; set; }
        public FormSourceType SourceType { get; set; }

        // Foreign Key
        public int FormItemCategoryId { get; set; }
        // Navigation Property
        public FormItemCategory FormItemCategory { get; set; }

        public ICollection<FormItemDefinition> FormItemDefinitions { get; set; }

        public int FormItemTypeId { get; set; }
        public FormItemCoefficient FormItemType { get; set; }
    }
}