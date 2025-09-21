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
        Task<IEnumerable<ExamSchedule>> GetAllSchedulesByExamIdAsync(int examId, CancellationToken cancellationToken);
        Task<ExamSchedule?> GetByScheduleIdAsync(int id, CancellationToken cancellationToken);
        Task<List<ExamEventType>> GetUsedEventTypesByExamIdAsync(int examId, CancellationToken cancellationToken);

        Task AddScheduleAsync(ExamSchedule schedule, CancellationToken cancellationToken);
        Task UpdateScheduleAsync(ExamSchedule schedule, CancellationToken cancellationToken);
        Task DeleteScheduleAsync(int id, CancellationToken cancellationToken);
    }
}
