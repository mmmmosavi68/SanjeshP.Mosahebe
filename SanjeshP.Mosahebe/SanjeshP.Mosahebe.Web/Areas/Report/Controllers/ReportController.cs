using Microsoft.AspNetCore.Mvc;
using SanjeshP.Mosahebe.Data.Contracts.Report;
using SanjeshP.Mosahebe.Web.Areas.Report.ViewModels.Report;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace SanjeshP.Mosahebe.Web.Areas.Report.Controllers
{
    [AllowAnonymous]
    [Area("Report")]
    public class ReportController : Controller
    {
        private readonly IReportRepository _reportRepository;

        public ReportController(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var reports = await _reportRepository.GetAllReportAsync(cancellationToken);
            var model = new ReportDownloadViewModel
            {
                Reports = reports.Select(r => new ReportViewModel
                {
                    Id = r.Id,
                    Title = r.Title
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DownloadExcel(ReportDownloadViewModel model, CancellationToken cancellationToken)
        {
            if (model.SelectedReportId == 0)
                return BadRequest("گزارشی انتخاب نشده است.");

            // گرفتن دیتا
            DataTable dt = await _reportRepository.GetReportDataAsync(model.SelectedReportId, cancellationToken: cancellationToken);

            // تبدیل DataTable به Excel (اینجا با CSV ساده مثال زدم، می‌تونی با EPPlus یا ClosedXML هم بزنیش)
            var memoryStream = new MemoryStream();
            using (var writer = new StreamWriter(memoryStream, leaveOpen: true))
            {
                // ستون‌ها
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (i > 0) writer.Write(",");
                    writer.Write(dt.Columns[i].ColumnName);
                }
                writer.WriteLine();

                // داده‌ها
                foreach (DataRow row in dt.Rows)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (i > 0) writer.Write(",");
                        writer.Write(row[i]?.ToString().Replace(",", " ") ?? "");
                    }
                    writer.WriteLine();
                }

                writer.Flush();
            }

            memoryStream.Position = 0;
            string fileName = $"Report_{model.SelectedReportId}_{DateTime.Now:yyyyMMddHHmmss}.csv";
            return File(memoryStream, "text/csv", fileName);
        }
    }
}