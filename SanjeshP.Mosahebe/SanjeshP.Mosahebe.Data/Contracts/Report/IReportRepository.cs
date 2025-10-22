using SanjeshP.Mosahebe.Data.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Contracts.Report
{
    public interface IReportRepository : IEntityFrameworkRepository<Entities.Report.Report>
    {
        // گرفتن همه گزارش‌های فعال
        Task<List<Entities.Report.Report>> GetAllReportAsync(CancellationToken cancellationToken);

        // گرفتن گزارش بر اساس ID
        Task<Entities.Report.Report> GetReportByIDAsync(int reportId, CancellationToken cancellationToken);

        // اجرای پروسیجر گزارش و برگرداندن DataTable
        Task<DataTable> GetReportDataAsync(int reportId,Dictionary<string, object> parameters = null,CancellationToken cancellationToken = default);
    }
}
