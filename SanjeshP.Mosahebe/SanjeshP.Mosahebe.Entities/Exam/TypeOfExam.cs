using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Exam
{
    public class TypeOfExam : BaseEntity
    {
        // کد استاناندارد
        [Required]
        public int StandardCode { get; set; }

        // عنوان نوع آزمون (مثل ارشد، دکتری، زبان و ...)
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        // توضیحات اختیاری درباره نوع آزمون
        [MaxLength(500)]
        public string Description { get; set; }
    }

}
