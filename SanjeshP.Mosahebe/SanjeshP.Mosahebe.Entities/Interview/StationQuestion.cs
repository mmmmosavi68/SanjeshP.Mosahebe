using SanjeshP.Mosahebe.Entities.Common;
using SanjeshP.Mosahebe.Entities.StudentForm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Interview
{
    public class StationQuestion :BaseEntity<int>
    {
        [Required]
        [MaxLength(500)]
        public string Text { get; set; }

        [Required]
        //[Column(TypeName = "decimal(6,2)")]
        [Range(0, 10000)]
        public double MaxValue { get; set; } // دقت بهتر برای محاسبه نمره

        public int StationId { get; set; }
        [ForeignKey(nameof(StationId))]
        public Station Station { get; set; }
    }
}
