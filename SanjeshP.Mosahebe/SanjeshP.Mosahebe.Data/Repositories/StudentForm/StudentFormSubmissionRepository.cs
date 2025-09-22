using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Entities.Student;
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
    public class StudentFormSubmissionRepository : EntityFrameworkRepository<StudentFormSubmission>, IStudentFormSubmissionRepository, IScopedDependency
    {
        public StudentFormSubmissionRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<StudentFormSubmission>> GetByStudentFormIdAsync(int formId, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Where(s => s.FormId == formId && !s.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<StudentFormSubmission?> GetByStudentFormSubmissionIdAsync(int id, CancellationToken cancellationToken)
        {
            return await Table
                .Where(s => s.Id == id && !s.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddStudentFormSubmissionAsync(StudentFormSubmission studentFormSubmission, CancellationToken cancellationToken) =>
            await AddAsync(studentFormSubmission, cancellationToken);

        public async Task UpdateStudentFormSubmissionAsync(StudentFormSubmission studentFormSubmission, CancellationToken cancellationToken) =>
            await UpdateAsync(studentFormSubmission, cancellationToken);

        public async Task DeleteStudentFormSubmissionAsync(int id, CancellationToken cancellationToken)
        {
            var submission = await GetByStudentFormSubmissionIdAsync(id, cancellationToken);
            if (submission is null)
                throw new KeyNotFoundException("ارسال فرم مورد نظر یافت نشد");

            submission.IsDeleted = true;
            await UpdateAsync(submission, cancellationToken);
        }
    }
}
