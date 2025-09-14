using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Exam
{
    public class PreviousField : BaseEntity<int>
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }
    }
}
