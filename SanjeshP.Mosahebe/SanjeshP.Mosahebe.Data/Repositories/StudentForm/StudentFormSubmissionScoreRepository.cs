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
using SanjeshP.Mosahebe.Entities.Exam;

namespace SanjeshP.Mosahebe.Data.Repositories.StudentForm
{
    public class StudentFormSubmissionScoreRepository : EntityFrameworkRepository<StudentFormSubmissionScore>, IStudentFormSubmissionScoreRepository, IScopedDependency
    {
        public StudentFormSubmissionScoreRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<StudentFormSubmissionScore>> GetByStudentFormIdAsync(int formId, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Where(s => s.FormItemId == formId && !s.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<StudentFormSubmissionScore?> GetByStudentFormSubmissionIdAsync(int id, CancellationToken cancellationToken)
        {
            return await Table
                .Where(s => s.Id == id && !s.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddStudentFormSubmissionAsync(StudentFormSubmissionScore studentFormSubmission, CancellationToken cancellationToken) =>
            await AddAsync(studentFormSubmission, cancellationToken);

        public async Task UpdateStudentFormSubmissionAsync(StudentFormSubmissionScore studentFormSubmission, CancellationToken cancellationToken) =>
            await UpdateAsync(studentFormSubmission, cancellationToken);

        public async Task DeleteStudentFormSubmissionAsync(int id, CancellationToken cancellationToken)
        {
            var submission = await GetByStudentFormSubmissionIdAsync(id, cancellationToken);
            if (submission is null)
                throw new KeyNotFoundException("ارسال فرم مورد نظر یافت نشد");

            submission.IsDeleted = true;
            await UpdateAsync(submission, cancellationToken);
        }
        public async Task<StudentFormSubmissionScore> GetByStudentAndFormAsync(int studentId, int formItemId, CancellationToken cancellationToken)
        {
            return await Table
                .FirstOrDefaultAsync(x => x.StudentId == studentId && x.FormItemId == formItemId && !x.IsDeleted, cancellationToken);
        }

        public async Task<IEnumerable<StudentFormSubmissionScore>> GetByStudentAsync(int studentId, CancellationToken cancellationToken)
        {

            return await TableNoTracking
            .Include(fss => fss.FormItem)                      // جدول FormItems
            .Include(fss => fss.StudentFormSubmissionDatas)    // جدول StudentFormSubmissionData
            .Where(fss => fss.StudentId == studentId)
            .ToListAsync(cancellationToken);

        }

        public async Task<StudentFormSubmissionScore> GetFormSubmissionDataByStudentAndFormAsync(int studentId, int formItemId, CancellationToken cancellationToken)
        {
            return await TableNoTracking
            .Include(fss => fss.FormItem)                      // جدول FormItems
            .Include(fss => fss.StudentFormSubmissionDatas)    // جدول StudentFormSubmissionData
            .Where(fss => fss.StudentId == studentId)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
        }
    }
}
