using SanjeshP.Mosahebe.Entities.Exam;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SanjeshP.Mosahebe.Data.Contracts.Exam
{
    public interface IExamFieldRepository
    {
        Task<IEnumerable<ExamField>> GetAllFieldsByExamIdAsync(int examId, CancellationToken cancellationToken);
        Task<ExamField?> GetByFieldIdAsync(int id, CancellationToken cancellationToken);
        Task AddFieldAsync(ExamField field, CancellationToken cancellationToken);
        Task UpdateFieldAsync(ExamField field, CancellationToken cancellationToken);
        Task DeleteFieldAsync(int id, CancellationToken cancellationToken);
    }
}
