using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Interview
{
    public class QuestionAnswer : BaseEntity<int>
    {
        public int StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        public Student.Student Student { get; set; }

        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User.User User { get; set; }

        public int StationId { get; set; }
        [ForeignKey(nameof(StationId))]
        public Station Station { get; set; }

        public int QuestionId { get; set; }
        [ForeignKey(nameof(QuestionId))]
        public StationQuestion Question { get; set; }

        [Required]
        [Column(TypeName = "decimal(6,2)")]
        [Range(0, 10000)]
        public decimal Value { get; set; }
    }

}
