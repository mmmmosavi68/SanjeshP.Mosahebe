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
using Aspose.Cells;

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

            // گرفتن دیتا از ریپازیتوری
            // این متد باید DataTable را برگرداند.
            DataTable dt = await _reportRepository.GetReportDataAsync(model.SelectedReportId, cancellationToken: cancellationToken);

            if (dt == null || dt.Rows.Count == 0)
            {
                // می‌توانید یک پیام مناسب‌تر به کاربر برگردانید که داده‌ای یافت نشد.
                return NotFound("داده‌ای برای گزارش انتخابی یافت نشد.");
            }

            // --- شروع تبدیل DataTable به Excel با Aspose.Cells ---

            // 1. ایجاد یک نمونه Workbook جدید
            // توجه: در محیط Production، برای استفاده طولانی مدت از Aspose، لایسنس باید اعمال شود.
            var workbook = new Workbook();

            // 2. دسترسی به اولین Worksheet
            Worksheet worksheet = workbook.Worksheets[0];

            // 3. تنظیم نام شیبیت (اختیاری)
            worksheet.Name = $"Report_{model.SelectedReportId}";

            // 4. انتقال داده‌ها از DataTable به Worksheet
            // استفاده از امضای 6-آرگومانی ImportDataTable برای سازگاری کامل
            worksheet.Cells.ImportDataTable(
                dt,
                true, // نمایش نام ستون‌ها در ردیف اول
                0,    // شروع از ردیف 0 (اولین ردیف)
                0,    // شروع از ستون 0 (اولین ستون)
                dt.Rows.Count, // تعداد ردیف‌های داده (بدون احتساب عنوان)
                dt.Columns.Count // تعداد ستون‌های داده
            );

            // 5. تنظیم عرض ستون‌ها به صورت خودکار برای خوانایی بهتر (اختیاری)
            // این خط حذف/کامنت شده است، زیرا باعث خطای "System.Drawing.Common" در محیط‌های غیر ویندوزی می‌شود.
            // worksheet.AutoFitColumns(); 

            // 6. ذخیره Workbook در یک MemoryStream
            var memoryStream = new MemoryStream();
            // SaveFormat.Xlsx برای فرمت استاندارد جدید اکسل استفاده می‌شود.
            workbook.Save(memoryStream, SaveFormat.Xlsx);

            // 7. بازگرداندن موقعیت استریم به ابتدا برای ارسال
            memoryStream.Position = 0;

            // --- پایان تبدیل DataTable به Excel با Aspose.Cells ---

            string fileName = $"Report_{model.SelectedReportId}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            // توجه: برای فایل XLSX، نوع محتوا باید "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" باشد.
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}