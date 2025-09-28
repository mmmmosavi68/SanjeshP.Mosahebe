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
    public class FormItemCategoryRepository : EntityFrameworkRepository<FormItemCategory>, IFormItemCategoryRepository, IScopedDependency
    {
        public FormItemCategoryRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<FormItemCategory>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await TableNoTracking.ToListAsync(cancellationToken);
        }

        public async Task<FormItemCategory?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await TableNoTracking.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
        }

        public async Task AddAsync(FormItemCategory entity, CancellationToken cancellationToken) =>
            await AddAsync(entity, cancellationToken);

        public async Task UpdateAsync(FormItemCategory entity, CancellationToken cancellationToken) =>
            await UpdateAsync(entity, cancellationToken);

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity == null) throw new KeyNotFoundException("دسته بندی مورد نظر یافت نشد");

            // اگر حذف نرم‌افزاری دارید:
            // entity.IsDeleted = true;
            // await UpdateAsync(entity, cancellationToken);

            // اگر حذف فیزیکی:
            await DeleteAsync(entity, cancellationToken);
        }
    }
}
