using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Contracts.Exam;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Entities.Exam;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SanjeshP.Mosahebe.Data.Repositories.Exam
{
    public class ExamFieldRepository : EntityFrameworkRepository<ExamField>, IExamFieldRepository, IScopedDependency
    {
        public ExamFieldRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<ExamField>> GetAllFieldsByExamIdAsync(int examId, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Include(f => f.Exam)
                .Where(f => !f.IsDeleted && f.ExamId == examId)
                .OrderBy(f => f.Title)
                .ToListAsync(cancellationToken);
        }

        public async Task<ExamField?> GetByFieldIdAsync(int id, CancellationToken cancellationToken)
        {
            return await Table
                .Include(f => f.Exam)
                .Where(f => !f.IsDeleted && f.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddFieldAsync(ExamField field, CancellationToken cancellationToken)
        {
            await AddAsync(field, cancellationToken);
        }

        public async Task UpdateFieldAsync(ExamField field, CancellationToken cancellationToken)
        {
            await UpdateAsync(field, cancellationToken);
        }

        public async Task DeleteFieldAsync(int id, CancellationToken cancellationToken)
        {
            var field = await GetByFieldIdAsync(id, cancellationToken);
            if (field == null)
                throw new KeyNotFoundException("رشته مورد نظر یافت نشد");

            field.IsDeleted = true;
            await UpdateAsync(field, cancellationToken);
        }
    }
}
