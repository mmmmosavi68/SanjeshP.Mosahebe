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
    public interface IStudentFormSubmissionScoreRepository : IEntityFrameworkRepository<StudentFormSubmissionScore>
    {
        Task<IEnumerable<StudentFormSubmissionScore>> GetByStudentFormIdAsync(int formId, CancellationToken cancellationToken);
        Task<StudentFormSubmissionScore?> GetByStudentFormSubmissionIdAsync(int id, CancellationToken cancellationToken);
        Task AddStudentFormSubmissionAsync(StudentFormSubmissionScore studentFormSubmission, CancellationToken cancellationToken);
        Task UpdateStudentFormSubmissionAsync(StudentFormSubmissionScore studentFormSubmission, CancellationToken cancellationToken);
        Task DeleteStudentFormSubmissionAsync(int id, CancellationToken cancellationToken);
        Task<StudentFormSubmissionScore?> GetByStudentAndFormAsync(int studentId, int formItemId, CancellationToken cancellationToken);
        Task<IEnumerable<StudentFormSubmissionScore>> GetByStudentAsync(int studentId, CancellationToken cancellationToken);
        Task<StudentFormSubmissionScore> GetFormSubmissionDataByStudentAndFormAsync(int studentId, int formItemId, CancellationToken cancellationToken);
        //Mousavi
        Task<IEnumerable<StudentFormSubmissionScore>> GetAllByStudentIdAsync(int studentId, CancellationToken cancellationToken);
        Task<StudentFormSubmissionScore> GetDetailsAsync(int id, CancellationToken cancellationToken);
        Task<StudentFormSubmissionScore> GetDetailsAsyncReadOnly(int id, CancellationToken cancellationToken);
        Task<StudentFormSubmissionScore> GetDetailsForEditAsync(int id, CancellationToken cancellationToken);
    }

}
