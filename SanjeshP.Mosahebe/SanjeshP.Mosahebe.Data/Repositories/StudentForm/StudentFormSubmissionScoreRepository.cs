using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Contracts.StudentForm;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Entities.StudentForm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace SanjeshP.Mosahebe.Data.Repositories.StudentForm
{
    public class StudentFormSubmissionScoreRepository : EntityFrameworkRepository<StudentFormSubmissionScore>, IStudentFormSubmissionScoreRepository, IScopedDependency
    {
        private readonly IFormItemRepository _formItemRepository;

        public StudentFormSubmissionScoreRepository(ApplicationDbContext dbContext, IFormItemRepository formItemRepository) : base(dbContext)
        {
            _formItemRepository = formItemRepository;
        }

        public async Task<StudentFormSubmissionScore> GetDetailsForEditAsync(int id, CancellationToken cancellationToken)
        {
            return await Table
                .Include(s => s.StudentFormSubmission)
                    .ThenInclude(sf => sf.FormItem)
                        .ThenInclude(fi => fi.FormItemCoefficient)
                .Include(s => s.StudentFormSubmission)
                    .ThenInclude(sf => sf.StudentFormSubmissionDatas)
                        .ThenInclude(d => d.FormItemDefinition)
                .Include(s => s.StudentFormSubmission)
                    .ThenInclude(sf => sf.StudentFormSubmissionDatas)
                        .ThenInclude(d => d.FormItemCoefficientDefinition)
                            .ThenInclude(fd => fd.Coefficients)
                .FirstOrDefaultAsync(s => s.StudentFormSubmission.Id == id, cancellationToken);

        }
        public async Task<StudentFormSubmissionScore?> GetExistingScoreAsync(int studentId, int formSubmissionId, Guid createdBy, CancellationToken cancellationToken)
        {
            return await Table
                .Include(s => s.StudentFormSubmission)
                .FirstOrDefaultAsync(s =>
                    s.StudentId == studentId &&
                    s.StudentFormSubmissionId == formSubmissionId &&
                    s.CreatedBy == createdBy &&
                    !s.IsDeleted,
                    cancellationToken);
        }


        public async Task<StudentFormSubmissionScore?> GetByStudentFormSubmissionScoreByStudentIdUserIDAsync(int id, CancellationToken cancellationToken)
        {
            return await Table
                .Where(s => s.Id == id && !s.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddStudentFormSubmissionScoreAsync(StudentFormSubmissionScore studentFormSubmissionScore, CancellationToken cancellationToken) =>
         await AddAsync(studentFormSubmissionScore, cancellationToken);

        public async Task UpdateStudentFormSubmissionScoreAsync(StudentFormSubmissionScore studentFormSubmissionScore, CancellationToken cancellationToken) =>
            await UpdateAsync(studentFormSubmissionScore, cancellationToken);

        public async Task DeleteStudentFormSubmissionScoreAsync(int id, CancellationToken cancellationToken)
        {
            var submission = await GetByStudentFormSubmissionScoreByStudentIdUserIDAsync(id, cancellationToken);
            if (submission is null)
                throw new KeyNotFoundException("ارسال فرم مورد نظر یافت نشد");

            submission.IsDeleted = true;
            submission.UpdatedDate = DateTime.Now;
            await UpdateAsync(submission, cancellationToken);
        }
    }
}
