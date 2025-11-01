using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.StudentForm
{
    public class Coefficient : IEntity
    {
        [Key]
        public int Id { get; set; }
        public int CoefficientId { get; set; }
        public int CodeFarei { get; set; }
        public string Name { get; set; }
        public float RangeStart { get; set; }
        public float RangeEnd { get; set; }
        [Column(TypeName = "decimal(6,3)")]
        public double Value { get; set; }

    }
}
