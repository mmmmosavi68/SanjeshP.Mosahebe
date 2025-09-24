using SanjeshP.Mosahebe.Entities.Common;
using System.Collections.Generic;

namespace SanjeshP.Mosahebe.Entities.StudentForm
{
    public class FormItemType : BaseEntity<int>
    {
        public string Title { get; set; }

        public ICollection<FormItem> FormItem { get; set; }
    }
}