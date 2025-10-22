using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Contracts.Report;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Repositories.Report
{
    public class ReportRepository : EntityFrameworkRepository<Entities.Report.Report>, IReportRepository, IScopedDependency
    {
        private readonly ApplicationDbContext _dbContext;

        public ReportRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        // گرفتن همه گزارش‌های فعال
        public Task<List<Entities.Report.Report>> GetAllReportAsync(CancellationToken cancellationToken)
        {
            return TableNoTracking
                     .Where(r => r.IsActive == true)
                     .OrderBy(r => r.Id)
                     .ToListAsync(cancellationToken);
        }

        // گرفتن گزارش بر اساس ID
        public Task<Entities.Report.Report> GetReportByIDAsync(int reportId, CancellationToken cancellationToken)
        {
            return TableNoTracking
                     .Where(r => r.IsActive == true && r.Id == reportId)
                     .SingleOrDefaultAsync(cancellationToken);
        }

        // اجرای هر پروسیجر گزارش با پارامترهای داینامیک
        public async Task<DataTable> GetReportDataAsync(int reportId, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            // 1. دریافت اطلاعات پروسیجر از جدول Reports
            var report = await GetReportByIDAsync(reportId, cancellationToken);
            if (report == null)
                throw new KeyNotFoundException($"گزارش با شناسه {reportId} پیدا نشد.");

            // 2. آماده سازی DataTable
            var dt = new DataTable();

            // 3. گرفتن اتصال از DbContext
            var conn = _dbContext.Database.GetDbConnection();
            await using (conn)
            {
                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync(cancellationToken);

                await using var cmd = conn.CreateCommand();
                cmd.CommandText = $"[dbo].[{report.Proc_Name}]";
                cmd.CommandType = CommandType.StoredProcedure;

                // 4. اضافه کردن پارامترهای داینامیک (اگر موجود است)
                if (parameters != null)
                {
                    foreach (var p in parameters)
                    {
                        var param = new SqlParameter(p.Key, p.Value ?? DBNull.Value);
                        cmd.Parameters.Add(param);
                    }
                }

                // 5. اجرای reader و پر کردن DataTable
                await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                dt.Load(reader);
            }

            return dt;
        }
    }
}
