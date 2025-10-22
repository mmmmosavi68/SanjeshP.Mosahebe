using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Report
{
    [Table("Reports")]
    public class Report : BaseEntity<int>
    {

        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(100)]
        public string Proc_Name { get; set; }

        [MaxLength(100)]
        public bool IsActive { get; set; } // توجه: nvarchar(100) در جدول SQL

        public bool? OnlyAdmin { get; set; }
    }
}
