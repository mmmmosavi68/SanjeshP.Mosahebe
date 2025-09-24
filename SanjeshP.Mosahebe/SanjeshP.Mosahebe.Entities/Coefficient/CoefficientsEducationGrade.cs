using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Coefficient
{
    public class CoefficientsEducationGrade : BaseEntity<int>
    {
        public string Degree { get; set; }        // مقطع (Bachelor, Master, PhD)
        public double MinGPA { get; set; }        // حداقل معدل
        public double MaxGPA { get; set; }        // حداکثر معدل
        public double Weight { get; set; }        // ضریب/وزن
        public string Notes { get; set; }         // توضیحات (اختیاری)
    }
}
