using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Entities.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SanjeshP.Mosahebe.Data.Contracts.Report
{
    public interface IReportStimulsoftRepository : IEntityFrameworkRepository<ReportsStimulsoft>
    {
        Task<List<ReportsStimulsoft>> GetAllReportsAsync(CancellationToken cancellationToken);
        Task<ReportsStimulsoft> GetReportByIdAsync(int reportId, CancellationToken cancellationToken);
        Task<DataTable> GetReportDataAsync(int reportId, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default);
    }
}
