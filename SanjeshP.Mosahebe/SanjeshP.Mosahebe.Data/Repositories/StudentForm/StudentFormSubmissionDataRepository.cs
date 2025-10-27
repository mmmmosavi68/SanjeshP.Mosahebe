using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SanjeshP.Mosahebe.Entities.StudentForm;
using SanjeshP.Mosahebe.Data.Contracts.StudentForm;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SanjeshP.Mosahebe.Data.Repositories.StudentForm
{
    public class StudentFormSubmissionDataRepository : EntityFrameworkRepository<StudentFormSubmissionData>, IStudentFormSubmissionDataRepository, IScopedDependency
    {
        public StudentFormSubmissionDataRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<StudentFormSubmissionData>> GetByStudentFormSubmissionValueIdAsync(int submissionId, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Where(v => v.StudentFormSubmissionScoreId == submissionId && !v.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<StudentFormSubmissionData?> GetByStudentValueIdAsync(int id, CancellationToken cancellationToken)
        {
            return await Table
                .Where(v => v.Id == id && !v.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddStudentFormSubmissionValueAsync(StudentFormSubmissionData value, CancellationToken cancellationToken) =>
            await AddAsync(value, cancellationToken);

        public async Task UpdateStudentFormSubmissionValueAsync(StudentFormSubmissionData value, CancellationToken cancellationToken) =>
            await UpdateAsync(value, cancellationToken);

        public async Task DeleteStudentFormSubmissionValueAsync(int submissionId, CancellationToken cancellationToken)
        {
            var values = await GetByStudentFormSubmissionValueIdAsync(submissionId, cancellationToken);

            if (values == null || !values.Any())
                throw new KeyNotFoundException("هیچ مقدار فرمی برای این شناسه یافت نشد");

            foreach (var value in values)
            {
                value.IsDeleted = true;
                value.UpdatedDate = DateTime.Now;

            }

            await UpdateRangeAsync(values, cancellationToken);
        }

        public async Task<StudentFormSubmissionData?> GetSubmissionDataByIdAsync(int submissionDataId, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Where(v => v.Id == submissionDataId && !v.IsDeleted)                
                .Include(d => d.StudentFormSubmissionScore) // اگر نیاز دارید اطلاعات والد را هم داشته باشید
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
