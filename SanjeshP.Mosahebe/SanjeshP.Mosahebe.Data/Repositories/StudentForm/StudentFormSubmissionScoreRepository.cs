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

        // Rezaei
        public async Task<IEnumerable<StudentFormSubmissionScore>> GetByStudentAsync(int studentId, CancellationToken cancellationToken)
        {

            return await TableNoTracking
            .Include(fss => fss.FormItem)                      // جدول FormItems
            .Include(fss => fss.StudentFormSubmissionDatas)    // جدول StudentFormSubmissionData
            .Where(fss => fss.StudentId == studentId)
            .ToListAsync(cancellationToken);

        }

        // Rezaei
        public async Task<StudentFormSubmissionScore> GetFormSubmissionDataByStudentAndFormAsync(int studentId, int formItemId, CancellationToken cancellationToken)
        {
            return await TableNoTracking
            .Include(fss => fss.FormItem)                      // جدول FormItems
            .Include(fss => fss.StudentFormSubmissionDatas)    // جدول StudentFormSubmissionData
            .Where(fss => fss.StudentId == studentId)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
        }

        // Mousavi
        public async Task<IEnumerable<StudentFormSubmissionScore>> GetAllByStudentIdAsync(int studentId, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Include(s => s.FormItem)
                .Include(s => s.StudentFormSubmissionDatas)
                    .ThenInclude(d => d.FormItemDefinition)
                .Include(s => s.StudentFormSubmissionDatas)
                    .ThenInclude(d => d.FormItemCoefficientDefinition)
                .Where(s => s.StudentId == studentId)
                .OrderByDescending(s => s.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        //Mousavi
        public async Task<StudentFormSubmissionScore> GetDetailsAsync(int id, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Include(s => s.FormItem)
                .Include(s => s.StudentFormSubmissionDatas)
                    .ThenInclude(d => d.FormItemDefinition)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }
        //Mousavi
        public async Task<StudentFormSubmissionScore> GetDetailsAsyncReadOnly(int id, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Include(s => s.FormItem)
                    .ThenInclude(fi => fi.FormItemCoefficient)
                        .ThenInclude(fic => fic.CoefficientId) // اگر FormItemCoefficient مجموعه Coefficients نگه می‌دارد
                .Include(s => s.StudentFormSubmissionDatas)
                    .ThenInclude(d => d.FormItemDefinition)
                .Include(s => s.StudentFormSubmissionDatas)
                    .ThenInclude(d => d.FormItemCoefficientDefinition)
                        .ThenInclude(ficd => ficd.CoefficientId) // اگر FormItemCoefficientDefinition هم Collection Coefficients دارد
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }
        //Mousavi
        //public async Task<StudentFormSubmissionScore> GetDetailsForEditAsync(int id, CancellationToken cancellationToken)
        //{
        //    // از Table (با Tracking) استفاده می‌کنیم تا تغییرات روی همین موجودیت‌ها قابل ذخیره باشد
        //    return await Table
        //        .Include(s => s.FormItem)
        //            .ThenInclude(fi => fi.FormItemCoefficient)
        //                .ThenInclude(fic => fic.CoefficientId)
        //        .Include(s => s.StudentFormSubmissionDatas)
        //            .ThenInclude(d => d.FormItemDefinition)
        //        .Include(s => s.StudentFormSubmissionDatas)
        //            .ThenInclude(d => d.FormItemCoefficientDefinition)
        //                .ThenInclude(ficd => ficd.CoefficientId)
        //        .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        //}
        public async Task<StudentFormSubmissionScore> GetDetailsForEditAsync(int id, CancellationToken cancellationToken)
        {
            return await Table
                .Include(s => s.FormItem)
                .Include(s => s.StudentFormSubmissionDatas)
                    .ThenInclude(d => d.FormItemDefinition)
                .Include(s => s.StudentFormSubmissionDatas)
                    .ThenInclude(d => d.FormItemCoefficientDefinition)
                        .ThenInclude(fd => fd.Coefficients) // در صورت وجود نِیوِیگیشن
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

    }
}
