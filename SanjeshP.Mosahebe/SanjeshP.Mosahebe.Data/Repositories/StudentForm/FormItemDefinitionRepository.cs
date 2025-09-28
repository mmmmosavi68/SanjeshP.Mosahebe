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
    public class FormItemDefinitionRepository : EntityFrameworkRepository<FormItemDefinition>, IFormItemDefinitionRepository, IScopedDependency
    {
        public FormItemDefinitionRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<FormItemDefinition>> GetByFormItemIdAsync(int formItemId, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Where(f => f.FormItemId == formItemId && !f.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<FormItemDefinition?> GetByDefinitionIdAsync(int id, CancellationToken cancellationToken)
        {
            return await Table
                .Where(f => f.Id == id && !f.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddDefinitionAsync(FormItemDefinition entity, CancellationToken cancellationToken) =>
            await AddAsync(entity, cancellationToken);

        public async Task UpdateDefinitionAsync(FormItemDefinition entity, CancellationToken cancellationToken) =>
            await UpdateAsync(entity, cancellationToken);

        public async Task DeleteDefinitionAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await GetByDefinitionIdAsync(id, cancellationToken);
            if (entity == null) return;

            entity.IsDeleted = true;
            await UpdateAsync(entity, cancellationToken);
        }
    }
}
