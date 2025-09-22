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
    public class StudentFormSubmissionValueRepository : EntityFrameworkRepository<StudentFormSubmissionValue>, IStudentFormSubmissionValueRepository, IScopedDependency
    {
        public StudentFormSubmissionValueRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<StudentFormSubmissionValue>> GetByStudentFormSubmissionValueIdAsync(int submissionId, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Where(v => v.FormSubmissionId == submissionId && !v.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<StudentFormSubmissionValue?> GetByStudentValueIdAsync(int id, CancellationToken cancellationToken)
        {
            return await Table
                .Where(v => v.Id == id && !v.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddStudentFormSubmissionValueAsync(StudentFormSubmissionValue value, CancellationToken cancellationToken) =>
            await AddAsync(value, cancellationToken);

        public async Task UpdateStudentFormSubmissionValueAsync(StudentFormSubmissionValue value, CancellationToken cancellationToken) =>
            await UpdateAsync(value, cancellationToken);

        public async Task DeleteStudentFormSubmissionValueAsync(int id, CancellationToken cancellationToken)
        {
            var value = await GetByStudentValueIdAsync(id, cancellationToken);
            if (value is null)
                throw new KeyNotFoundException("مقدار فرم مورد نظر یافت نشد");

            value.IsDeleted = true;
            await UpdateAsync(value, cancellationToken);
        }
    }
}
