using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Entities.Student;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SanjeshP.Mosahebe.Entities.StudentForm;

namespace SanjeshP.Mosahebe.Data.Contracts.StudentForm
{
    public interface IStudentFormSubmissionRepository : IEntityFrameworkRepository<StudentFormSubmission>
    {
        Task<IEnumerable<StudentFormSubmission>> GetByStudentFormIdAsync(int formId, CancellationToken cancellationToken);
        Task<StudentFormSubmission?> GetByStudentFormSubmissionIdAsync(int id, CancellationToken cancellationToken);
        Task AddStudentFormSubmissionAsync(StudentFormSubmission studentFormSubmission, CancellationToken cancellationToken);
        Task UpdateStudentFormSubmissionAsync(StudentFormSubmission studentFormSubmission, CancellationToken cancellationToken);
        Task DeleteStudentFormSubmissionAsync(int id, CancellationToken cancellationToken);
    }
}
