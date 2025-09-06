using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Common.Enums.ExamSchedule;
using SanjeshP.Mosahebe.Entities.Exam;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Contracts.Exam
{
    public interface IExamScheduleRepository : IEntityFrameworkRepository<ExamSchedule>
    {
        Task<IEnumerable<ExamSchedule>> GetAllSchedulesByExamIdAsync(Guid examId, CancellationToken cancellationToken);
        Task<ExamSchedule?> GetByScheduleIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<ExamEventType>> GetUsedEventTypesByExamIdAsync(Guid examId, CancellationToken cancellationToken);

        Task AddScheduleAsync(ExamSchedule schedule, CancellationToken cancellationToken);
        Task UpdateScheduleAsync(ExamSchedule schedule, CancellationToken cancellationToken);
        Task DeleteScheduleAsync(Guid id, CancellationToken cancellationToken);
    }
}
