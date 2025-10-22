using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.Report.ViewModels.Report
{
    public class ReportDownloadViewModel
    {
        [Display(Name = "فهرست گزارشات فعال")]
        public int SelectedReportId { get; set; }

        public List<ReportViewModel> Reports { get; set; }
    }
}
