using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Data.Contracts.Exam;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Entities.Exam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Repositories.Exam
{
    public class ExamsRepository : EFRepository<Exams>, IExamRepository, IScopedDependency
    {
        public ExamsRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<Exams>> GetAllExamsNoTrackingAsync(CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Where(e => !e.IsDeleted)
                .Include(e => e.TypeOfExam)
                .OrderByDescending(e => e.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<Exams?> GetByExamsIDAsync(Guid id, CancellationToken cancellationToken)
        {
            return await Table
                .Where(e => !e.IsDeleted && e.Id == id)
                .Include(e => e.TypeOfExam)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddExamsAsync(Exams exam, CancellationToken cancellationToken)
        {
            await AddAsync(exam, cancellationToken);
        }

        public async Task UpdateExamsAsync(Exams exam, CancellationToken cancellationToken)
        {
            await UpdateAsync(exam, cancellationToken);
        }

        public async Task DeleteExamsAsync(Guid id, CancellationToken cancellationToken)
        {
            var exam = await GetByExamsIDAsync(id, cancellationToken);
            if (exam is null)
                throw new KeyNotFoundException("آزمون مورد نظر یافت نشد");

            exam.IsDeleted = true;
            await UpdateAsync(exam, cancellationToken);
        }
    }
}
