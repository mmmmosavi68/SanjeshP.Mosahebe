using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Coefficient
{
    public class CoefficientsLanguageCertificate : BaseEntity<int>
    {
        public string CertificateName { get; set; }  // نام مدرک (IELTS, TOEFL, GRE)
        public string Level { get; set; }            // سطح (مثلاً Overall Band یا CEFR)
        public double MinScore { get; set; }         // حداقل نمره
        public double MaxScore { get; set; }         // حداکثر نمره
        public double Weight { get; set; }           // ضریب امتیاز
        public string Notes { get; set; }            // توضیحات (اختیاری)
    }
}
