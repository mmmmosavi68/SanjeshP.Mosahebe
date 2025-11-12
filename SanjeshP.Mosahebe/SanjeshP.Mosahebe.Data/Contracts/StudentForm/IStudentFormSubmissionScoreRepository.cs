using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Entities.StudentForm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SanjeshP.Mosahebe.Data.Contracts.StudentForm
{
    public interface IStudentFormSubmissionScoreRepository : IEntityFrameworkRepository<StudentFormSubmissionScore>
    {
        Task<StudentFormSubmissionScore> GetDetailsForEditAsync(int id, CancellationToken cancellationToken);
        Task<StudentFormSubmissionScore?> GetExistingScoreAsync(int studentId, int formSubmissionId, Guid createdBy, CancellationToken cancellationToken);
        Task<StudentFormSubmissionScore?> GetByStudentFormSubmissionScoreByStudentIdUserIDAsync(int id, CancellationToken cancellationToken);
        Task AddStudentFormSubmissionScoreAsync(StudentFormSubmissionScore studentFormSubmissionScore, CancellationToken cancellationToken);
        Task UpdateStudentFormSubmissionScoreAsync(StudentFormSubmissionScore studentFormSubmissionScore, CancellationToken cancellationToken);
        Task DeleteStudentFormSubmissionScoreAsync(int id, CancellationToken cancellationToken);
    }
}
