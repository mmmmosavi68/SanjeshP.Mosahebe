using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Entities.Exam;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Contracts.Exam
{
    public interface ITypeOfExamRepository : IEntityFrameworkRepository<TypeOfExam>
    {
        Task<IEnumerable<TypeOfExam>> GetAllTypeOfExamNoTrackingAsync(CancellationToken cancellationToken);
        Task<TypeOfExam> GetByTypeOfExamIDAsync(Guid id, CancellationToken cancellationToken);
        Task AddTypeOfExamAsync(TypeOfExam typeOfExam, CancellationToken cancellationToken);
        Task UpdateTypeOfExamAsync(TypeOfExam typeOfExam, CancellationToken cancellationToken);
        Task DeleteTypeOfExamAsync(Guid id, CancellationToken cancellationToken);

    }
}
