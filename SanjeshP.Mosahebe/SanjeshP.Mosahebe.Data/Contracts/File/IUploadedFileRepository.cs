using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Entities.File;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SanjeshP.Mosahebe.Data.Contracts.File
{
    public interface IUploadedFileRepository : IEntityFrameworkRepository<UploadedFile>
    {
        Task<UploadedFile?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<UploadedFile>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken);
        Task<IEnumerable<UploadedFile>> GetByStudentIdAndStudentFormSubmissionIdAsync(int studentId,int StudentFormSubmissionDataId, CancellationToken cancellationToken);
        Task<IEnumerable<UploadedFile>> GetBySubmissionIdAsync(int submissionId, CancellationToken cancellationToken);
        Task AddUploadedFileAsync(UploadedFile uploadedFile, CancellationToken cancellationToken);
        Task DeleteUploadedFileAsync(int id, CancellationToken cancellationToken);
    }
}
