using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Coefficient
{
    public class CoefficientsResearchContribution : BaseEntity<int>
    {
        public int AuthorCount { get; set; }      // تعداد کل نویسندگان
        public int AuthorOrder { get; set; }      // جایگاه نویسنده (مثلاً نویسنده اول، دوم، سوم)
        public double Weight { get; set; }        // ضریب امتیاز
        public string Notes { get; set; }         // توضیحات (اختیاری)
    }

}
