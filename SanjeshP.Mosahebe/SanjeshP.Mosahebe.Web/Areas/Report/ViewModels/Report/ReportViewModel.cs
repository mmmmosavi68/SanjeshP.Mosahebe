using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.Report.ViewModels.Report
{
    public class ReportViewModel
    {
        public int Id { get; set; }

        [Display(Name = "عنوان گزارش")]
        public string Title { get; set; }
    }
}
