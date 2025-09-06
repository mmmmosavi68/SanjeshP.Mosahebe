using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Entities.Exam;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Contracts.Exam
{
    public interface IExamRepository : IEntityFrameworkRepository<Exams>
    {
        Task<IEnumerable<Exams>> GetAllExamsNoTrackingAsync(CancellationToken cancellationToken);
        Task<Exams> GetByExamsIDAsync(Guid id, CancellationToken cancellationToken);
        Task AddExamsAsync(Exams exams, CancellationToken cancellationToken);
        Task UpdateExamsAsync(Exams exams, CancellationToken cancellationToken);
        Task DeleteExamsAsync(Guid id, CancellationToken cancellationToken);
    }
}
