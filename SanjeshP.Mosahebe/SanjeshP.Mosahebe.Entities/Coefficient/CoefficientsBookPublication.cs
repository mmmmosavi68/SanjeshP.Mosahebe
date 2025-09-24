using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Coefficient
{
    public class CoefficientsBookPublication : BaseEntity<int>
    {
        public string Title { get; set; }         // نوع کتاب (Authored, Translated, Edited)
        public string PublisherType { get; set; } // ناشر (Local, International)
        public int Year { get; set; }             // سال انتشار
        public double Weight { get; set; }        // ضریب امتیاز
        public string Notes { get; set; }         // توضیحات (اختیاری)
    }

}
