using SanjeshP.Mosahebe.Data.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SanjeshP.Mosahebe.Entities.StudentForm;

namespace SanjeshP.Mosahebe.Data.Contracts.StudentForm
{
    public interface IStudentFormSubmissionValueRepository : IEntityFrameworkRepository<StudentFormSubmissionValue>
    {
        Task<IEnumerable<StudentFormSubmissionValue>> GetByStudentFormSubmissionValueIdAsync(int submissionId, CancellationToken cancellationToken);
        Task<StudentFormSubmissionValue?> GetByStudentValueIdAsync(int id, CancellationToken cancellationToken);
        Task AddStudentFormSubmissionValueAsync(StudentFormSubmissionValue studentFormSubmissionValue, CancellationToken cancellationToken);
        Task UpdateStudentFormSubmissionValueAsync(StudentFormSubmissionValue studentFormSubmissionValue, CancellationToken cancellationToken);
        Task DeleteStudentFormSubmissionValueAsync(int id, CancellationToken cancellationToken);
    }
}
