using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Contracts.StudentForm;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Entities.StudentForm;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Repositories.StudentForm
{
    public class FormItemCoefficientDefinitionRepository
        : EntityFrameworkRepository<FormItemCoefficientDefinition>, IFormItemCoefficientDefinitionRepository, IScopedDependency
    {
        public FormItemCoefficientDefinitionRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<FormItemCoefficientDefinition>> GetByCoefficientIdAsync(int coefficientId, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Where(f => f.CoefficientId == coefficientId && !f.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<FormItemCoefficientDefinition?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await TableNoTracking.FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted, cancellationToken);
        }

        // متد جدید: گرفتن تعریف همراه با Coefficients (برای استفاده در محاسبات)
        public async Task<FormItemCoefficientDefinition?> GetWithCoefficientsAsync(int id, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Include(f => f.Coefficients)
                .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted, cancellationToken);
        }

        // استفاده از متد پایه AddAsync معتبر (غیر بازگشتی)
        public async Task AddAsync(FormItemCoefficientDefinition entity, CancellationToken cancellationToken)
        {
            await base.AddAsync(entity, cancellationToken);
        }

        // استفاده از متد پایه UpdateAsync معتبر (غیر بازشکتی)
        public async Task UpdateAsync(FormItemCoefficientDefinition entity, CancellationToken cancellationToken)
        {
            await base.UpdateAsync(entity, cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity == null) return;

            entity.IsDeleted = true;
            await base.UpdateAsync(entity, cancellationToken);
        }

        public async Task<FormItemCoefficientDefinition?> GetFormItemCoefficientDefinitionWithCoefficientsAsync(int id, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Include(f => f.Coefficients)
                .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted, cancellationToken);
        }
    }
}
