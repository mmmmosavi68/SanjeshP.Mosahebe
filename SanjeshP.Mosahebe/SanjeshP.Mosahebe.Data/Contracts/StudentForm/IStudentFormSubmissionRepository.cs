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
        Task<StudentFormSubmission?> GetByStudentAndFormAsync(int studentId, int formItemId, CancellationToken cancellationToken);
        Task<IEnumerable<StudentFormSubmission>> GetByStudentAndFormsAsync(int studentId, int formItemId, CancellationToken cancellationToken);

        Task<IEnumerable<StudentFormSubmission>> GetByStudentAsync(int studentId, CancellationToken cancellationToken);
        Task<StudentFormSubmission> GetFormSubmissionDataByStudentAndFormAsync(int studentId, int formItemId, CancellationToken cancellationToken);
        Task<StudentFormSubmission> GetFormSubmissionDataByStudentAndSubmissionsIDAsync(int studentId, int SubmissionId, CancellationToken cancellationToken);

        //Mousavi
        Task<IEnumerable<StudentFormSubmission>> GetAllByStudentIdAsync(int studentId, CancellationToken cancellationToken);
        Task<StudentFormSubmission> GetDetailsAsync(int id, CancellationToken cancellationToken);

        Task<StudentFormSubmission> GetDetailsAsyncReadOnly(int id, CancellationToken cancellationToken);
        Task<StudentFormSubmission> GetDetailsForEditAsync(int id, CancellationToken cancellationToken);

    }

}
