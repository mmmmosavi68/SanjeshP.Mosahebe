using SanjeshP.Mosahebe.Entities.StudentForm;
using System.Collections.Generic;

namespace SanjeshP.Mosahebe.Web.Areas.Student.ViewModels
{
    public class FormSubmissionViewModel
    {
        public int FormId { get; set; }

        // فیلدهای تعریف شده در فرم
        public IEnumerable<FormItemDefinition> Fields { get; set; } = new List<FormItemDefinition>();

        // فیلدهای ضریب
        public IEnumerable<FormItemCoefficientDefinition> CoefficientFields { get; set; } = new List<FormItemCoefficientDefinition>();
        public IEnumerable<FormItemCoefficient> Coefficients { get; set; } = new List<FormItemCoefficient>();
        public IEnumerable<Coefficient> CoefficientList { get; set; } 

        public string FieldSource { get; set; } // "Definition" یا "Coefficient"
    }
}
