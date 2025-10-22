using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Contracts.Report;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Entities.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace SanjeshP.Mosahebe.Data.Repositories.Report
{
    public class ReportStimulsoftRepository : EntityFrameworkRepository<ReportsStimulsoft>, IReportStimulsoftRepository, IScopedDependency
    {
        private readonly ApplicationDbContext _dbContext;

        public ReportStimulsoftRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<ReportsStimulsoft>> GetAllReportsAsync(CancellationToken cancellationToken)
        {
            return TableNoTracking
                .Where(r => r.IsActive == true)
                .OrderBy(r => r.Id)
                .ToListAsync(cancellationToken);
        }

        public Task<ReportsStimulsoft> GetReportByIdAsync(int reportId, CancellationToken cancellationToken)
        {
            return TableNoTracking
                .Where(r => r.IsActive == true && r.Id == reportId)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<DataTable> GetReportDataAsync(int reportId, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            var report = await GetReportByIdAsync(reportId, cancellationToken);

            if (report == null)
                throw new KeyNotFoundException($"گزارش با شناسه {reportId} پیدا نشد.");

            var dt = new DataTable();

            var conn = _dbContext.Database.GetDbConnection();
            await using (conn)
            {
                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync(cancellationToken);

                await using var cmd = conn.CreateCommand();
                cmd.CommandText = $"[dbo].[{report.QueryName}]"; // نام پروسیجر
                cmd.CommandType = CommandType.StoredProcedure;

                // اضافه کردن پارامترها
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        var sqlParam = new SqlParameter(param.Key, param.Value ?? DBNull.Value);
                        cmd.Parameters.Add(sqlParam);
                    }
                }

                await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                dt.Load(reader);
            }

            return dt;
        }
    }
}
