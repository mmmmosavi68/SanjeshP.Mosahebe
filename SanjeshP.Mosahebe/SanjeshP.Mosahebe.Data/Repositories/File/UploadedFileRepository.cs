using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Contracts.File;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Entities.File;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SanjeshP.Mosahebe.Data.Repositories.File
{
    public class UploadedFileRepository : EntityFrameworkRepository<UploadedFile>, IUploadedFileRepository, IScopedDependency
    {
        public UploadedFileRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<UploadedFile?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await Table
                .Where(x => !x.IsDeleted && x.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<UploadedFile>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Where(x => !x.IsDeleted && x.StudentId == studentId)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<UploadedFile>> GetBySubmissionIdAsync(int submissionId, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Where(x => !x.IsDeleted && x.StudentFormSubmissionScoreId == submissionId)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task AddUploadedFileAsync(UploadedFile uploadedFile, CancellationToken cancellationToken)
        {
            await AddAsync(uploadedFile, cancellationToken);
        }

        public async Task DeleteUploadedFileAsync(int id, CancellationToken cancellationToken)
        {
            var file = await GetByIdAsync(id, cancellationToken);
            if (file is null)
                throw new KeyNotFoundException("فایل یافت نشد");

            file.IsDeleted = true;
            await UpdateAsync(file, cancellationToken);
        }

        public async Task<IEnumerable<UploadedFile>> GetByStudentIdAndStudentFormSubmissionScoreIdAsync(int studentId, int StudentFormSubmissionScoreId, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                 .Where(x => !x.IsDeleted && x.StudentId == studentId && x.StudentFormSubmissionScoreId==StudentFormSubmissionScoreId)
                 .OrderByDescending(x => x.CreatedDate)
                 .ToListAsync(cancellationToken);
        }
    }
}
