using SanjeshP.Mosahebe.Entities.Common;
using System.Collections.Generic;

namespace SanjeshP.Mosahebe.Entities.StudentForm
{
    public class FormItemCoefficient : BaseEntity<int>
    {
        public int CoefficientId { get; set; }       // ارتباط با جدول ضرایب
        public string Title { get; set; }
        public ICollection<FormItem> FormItem { get; set; }
    }
}