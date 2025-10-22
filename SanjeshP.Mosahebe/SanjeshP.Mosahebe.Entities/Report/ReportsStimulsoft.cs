using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Report
{
    [Table("Reports_stimulsoft")]
    public class ReportsStimulsoft : BaseEntity<int>
    {

        [MaxLength(500)]
        public string Report_Title { get; set; }

        [MaxLength(500)]
        public string Report_FileName { get; set; }

        public bool? IsActive { get; set; }

        [MaxLength(500)]
        public string QueryName { get; set; }
    }
}
