using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Entities.Student;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SanjeshP.Mosahebe.Data.Contracts.Students
{
    public interface IEducationDocumentScoreRepository : IEntityFrameworkRepository<EducationDocumentScore>
    {
        Task<double> GetEducationDocumentScoreValueByStudentId(int studentId, CancellationToken cancellationToken);
        Task<Dictionary<int, double>> GetScoresByStudentIdsAsync(List<int> studentIds, Guid UserId, CancellationToken cancellationToken);
        Task SaveOrUpdateFinalScoreAsync(int studentId, double finalScore, CancellationToken cancellationToken);
        Task CalculateAndSaveFinalScoreAsync(int studentId, Guid userId, CancellationToken cancellationToken);

    }
}
