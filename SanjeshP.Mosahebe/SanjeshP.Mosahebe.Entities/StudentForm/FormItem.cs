using SanjeshP.Mosahebe.Entities.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string PrivateDescription { get; set; }
        [Column(TypeName = "decimal(6,3)")]
        public double OneMaxScore { get; set; }
        [Column(TypeName = "decimal(6,3)")]
        public double AllMaxScore { get; set; }
        public FormSourceType SourceType { get; set; }
        public int MaxCount { get; set; }

        // Foreign Key
        public int FormItemCategoryId { get; set; }
        // Navigation Property
        public FormItemCategory FormItemCategory { get; set; }

        public ICollection<FormItemDefinition> FormItemDefinitions { get; set; }

        public int FormItemCoefficientId { get; set; }
        public FormItemCoefficient FormItemCoefficient { get; set; }
    }
}