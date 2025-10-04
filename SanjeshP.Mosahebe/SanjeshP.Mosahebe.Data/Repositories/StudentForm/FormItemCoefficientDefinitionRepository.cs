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

        public async Task AddAsync(FormItemCoefficientDefinition entity, CancellationToken cancellationToken) =>
            await AddAsync(entity, cancellationToken);

        public async Task UpdateAsync(FormItemCoefficientDefinition entity, CancellationToken cancellationToken) =>
            await UpdateAsync(entity, cancellationToken);

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity == null) return;

            entity.IsDeleted = true;
            await UpdateAsync(entity, cancellationToken);
        }
    }
}
