using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Student
{
    public enum FieldType
    {
        Text=1,
        Number=2,
        Url=3,
        File=4,
        Checkbox=5
    }

    public class FieldDefinition : BaseEntity<int>
    {
        public int FormId { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public FieldType Type { get; set; }
        public string TypeText { get; set; }
        public bool IsRequired { get; set; }
        public string Description { get; set; }
        public string ErrorMessage { get; set; }
    }
}
