using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Data.Contracts.Exam;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Common.Enums.ExamSchedule;
using SanjeshP.Mosahebe.Entities.Exam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Repositories.Exam
{
    public class ExamScheduleRepository : EntityFrameworkRepository<ExamSchedule>, IExamScheduleRepository, IScopedDependency
    {
        public ExamScheduleRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<ExamSchedule>> GetAllSchedulesByExamIdAsync(int examId, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Where(s => !s.IsDeleted && s.ExamId == examId)
                .OrderBy(s => s.StartTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<ExamSchedule?> GetByScheduleIdAsync(int id, CancellationToken cancellationToken)
        {
            return await Table
                .Where(s => !s.IsDeleted && s.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<List<ExamEventType>> GetUsedEventTypesByExamIdAsync(int examId, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Where(x => x.ExamId == examId)
                .Select(x => x.EventType)
                .ToListAsync(cancellationToken);
        }

        public async Task AddScheduleAsync(ExamSchedule schedule, CancellationToken cancellationToken)
        {
            await AddAsync(schedule, cancellationToken);
        }

        public async Task UpdateScheduleAsync(ExamSchedule schedule, CancellationToken cancellationToken)
        {
            await UpdateAsync(schedule, cancellationToken);
        }

        public async Task DeleteScheduleAsync(int id, CancellationToken cancellationToken)
        {
            var schedule = await GetByScheduleIdAsync(id, cancellationToken);
            if (schedule == null)
                throw new KeyNotFoundException("زمان‌بندی آزمون مورد نظر یافت نشد");

            schedule.IsDeleted = true;
            await UpdateAsync(schedule, cancellationToken);
        }
    }
}