using SanjeshP.Mosahebe.Data.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SanjeshP.Mosahebe.Entities.StudentForm;

namespace SanjeshP.Mosahebe.Data.Contracts.StudentForm
{
    public interface IStudentFormSubmissionDataRepository : IEntityFrameworkRepository<StudentFormSubmissionData>
    {
        Task<IEnumerable<StudentFormSubmissionData>> GetByStudentFormSubmissionValueIdAsync(int submissionId, CancellationToken cancellationToken);
        Task<StudentFormSubmissionData?> GetByStudentValueIdAsync(int id, CancellationToken cancellationToken);
        Task AddStudentFormSubmissionValueAsync(StudentFormSubmissionData studentFormSubmissionValue, CancellationToken cancellationToken);
        Task UpdateStudentFormSubmissionValueAsync(StudentFormSubmissionData studentFormSubmissionValue, CancellationToken cancellationToken);
        Task DeleteStudentFormSubmissionValueAsync(int submissionId, CancellationToken cancellationToken);
        // در IStudentFormSubmissionRepository (یا repository مناسب مربوط به StudentFormSubmissionData)
        Task<StudentFormSubmissionData?> GetSubmissionDataByIdAsync(int submissionDataId, CancellationToken cancellationToken);

    }
}
