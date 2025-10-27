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
            submission.UpdatedDate = DateTime.Now;
            await UpdateAsync(submission, cancellationToken);
        }
        public async Task<StudentFormSubmissionScore> GetByStudentAndFormAsync(int studentId, int formItemId, CancellationToken cancellationToken)
        {
            return await Table
                    .FirstOrDefaultAsync(
                        x => x.StudentId == studentId
                          && x.FormItemId == formItemId
                          && x.IsDeleted == false
                          && x.IsActive == true,
                        cancellationToken)
                    .ConfigureAwait(false);
        }

        // Rezaei
        public async Task<IEnumerable<StudentFormSubmissionScore>> GetByStudentAsync(int studentId, CancellationToken cancellationToken)
        {

            return await TableNoTracking
            .Include(fss => fss.FormItem)                      // جدول FormItems
            .Include(fss => fss.StudentFormSubmissionDatas)    // جدول StudentFormSubmissionData
                .ThenInclude(d => d.FormItemDefinition)
            .Include(s => s.StudentFormSubmissionDatas)
                    .ThenInclude(d => d.FormItemCoefficientDefinition)
            .Where(fss => fss.StudentId == studentId
                            && !fss.IsDeleted
                                  && fss.IsActive)
            .ToListAsync(cancellationToken);

        }

        // Rezaei
        public async Task<StudentFormSubmissionScore> GetFormSubmissionDataByStudentAndFormAsync(int studentId, int formItemId, CancellationToken cancellationToken)
        {
            var result = await TableNoTracking
                   .Include(fss => fss.FormItem)
                   .Include(fss => fss.StudentFormSubmissionDatas)
                       .ThenInclude(d => d.FormItemDefinition)
                   .Include(fss => fss.StudentFormSubmissionDatas)
                       .ThenInclude(d => d.FormItemCoefficientDefinition)
                   .Where(fss => fss.StudentId == studentId
                              && fss.FormItemId == formItemId
                              && !fss.IsDeleted
                              && fss.IsActive)
                   .FirstOrDefaultAsync(cancellationToken)
                   .ConfigureAwait(false);

            // فیلتر کردن دستی بعد از دریافت از دیتابیس
            if (result != null)
            {
                result.StudentFormSubmissionDatas = result.StudentFormSubmissionDatas
                    .Where(d => d.IsActive && !d.IsDeleted)
                    .ToList();
            }

            return result;

        }

        public async Task<StudentFormSubmissionScore> GetFormSubmissionDataByStudentAndSubmissionScoresIDAsync(int studentId, int SubmissionId, CancellationToken cancellationToken)
        {
            var result = await TableNoTracking
                 .Include(s => s.FormItem)
                     .ThenInclude(f => f.FormItemDefinitions)
                 .Include(s => s.StudentFormSubmissionDatas)
                     .ThenInclude(d => d.FormItemDefinition)
                 .Include(s => s.StudentFormSubmissionDatas)
                     .ThenInclude(d => d.FormItemCoefficientDefinition)
                 .Where(s => s.StudentId == studentId && s.Id==SubmissionId && s.IsActive && !s.IsDeleted)
                   .FirstOrDefaultAsync(cancellationToken)
                   .ConfigureAwait(false);




            // همون منطق اضافه‌کردن فیلدهای خالی و مرتب‌سازی بمونه ✅

            result.StudentFormSubmissionDatas = result.StudentFormSubmissionDatas
                .Where(d => d.IsActive && !d.IsDeleted)
                .ToList();

            var allDefs = result.FormItem?.FormItemDefinitions?.Where(d => d.IsActive && !d.IsDeleted).ToList() ?? new();

            foreach (var def in allDefs)
            {
                if (!result.StudentFormSubmissionDatas.Any(d => d.FormItemDefinitionId == def.Id))
                {
                    result.StudentFormSubmissionDatas.Add(new StudentFormSubmissionData
                    {
                        FormItemDefinitionId = def.Id,
                        FormItemDefinition = def,
                        Value = null,
                        InputType = def.inputType,
                        IsActive = true
                    });
                }
            }

            result.StudentFormSubmissionDatas = result.StudentFormSubmissionDatas
                    .OrderBy(d =>d.CreatedDate)
                    .ToList();


            return result;

        }

        // Mousavi
        public async Task<IEnumerable<StudentFormSubmissionScore>> GetAllByStudentIdAsync(int studentId, CancellationToken cancellationToken)
        {
            var results = await TableNoTracking
                .Include(s => s.FormItem)
                    .ThenInclude(f => f.FormItemCoefficient)
                .Include(s => s.FormItem)
                    .ThenInclude(f => f.FormItemDefinitions)
                .Include(s => s.StudentFormSubmissionDatas)
                    .ThenInclude(d => d.FormItemDefinition)
                .Include(s => s.StudentFormSubmissionDatas)
                    .ThenInclude(d => d.FormItemCoefficientDefinition)
                .Where(s => s.StudentId == studentId && s.IsActive && !s.IsDeleted)
                .OrderByDescending(s => s.CreatedDate)
                .ToListAsync(cancellationToken);

            // همون منطق اضافه‌کردن فیلدهای خالی و مرتب‌سازی بمونه ✅
            foreach (var score in results)
            {
                var datas = score.StudentFormSubmissionDatas ?? new List<StudentFormSubmissionData>();

                score.StudentFormSubmissionDatas = score.StudentFormSubmissionDatas
                    .Where(d => d.IsActive && !d.IsDeleted)
                    .ToList();

                score.StudentFormSubmissionDatas = datas
                                                    .Where(d => d.IsActive && !d.IsDeleted)
                                                    .ToList();


                var allDefs = score.FormItem?.FormItemDefinitions?.Where(d => d.IsActive && !d.IsDeleted).ToList() ?? new();

                foreach (var def in allDefs)
                {
                    if (!score.StudentFormSubmissionDatas.Any(d => d.FormItemDefinitionId == def.Id))
                    {
                        score.StudentFormSubmissionDatas.Add(new StudentFormSubmissionData
                        {
                            FormItemDefinitionId = def.Id,
                            FormItemDefinition = def,
                            Value = null,
                            InputType = def.inputType,
                            IsActive = true
                        });
                    }
                }

                score.StudentFormSubmissionDatas = score.StudentFormSubmissionDatas
                    .OrderBy(d => d.FormItemDefinition?.CreatedDate)
                    .ToList();
            }

            return results;
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
                    .ThenInclude(s=>s.FormItemCoefficient)
                .Include(s => s.StudentFormSubmissionDatas)
                    .ThenInclude(d => d.FormItemDefinition)
                .Include(s => s.StudentFormSubmissionDatas)
                    .ThenInclude(d => d.FormItemCoefficientDefinition)
                        .ThenInclude(fd => fd.Coefficients) // در صورت وجود نِیوِیگیشن
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<StudentFormSubmissionScore>> GetByStudentAndFormsAsync(int studentId, int formItemId, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                    .Where(x => x.StudentId == studentId
                          && x.FormItemId == formItemId
                          && x.IsDeleted == false
                          && x.IsActive == true)
                    .ToListAsync()
                    .ConfigureAwait(false);

        }
    }
}
