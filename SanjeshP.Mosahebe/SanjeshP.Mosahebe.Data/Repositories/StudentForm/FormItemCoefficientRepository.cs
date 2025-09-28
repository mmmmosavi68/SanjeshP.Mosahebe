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
    public class FormItemCoefficientRepository : EntityFrameworkRepository<FormItemCoefficient>, IFormItemCoefficientRepository, IScopedDependency
    {
        public FormItemCoefficientRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<FormItemCoefficient>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await TableNoTracking.ToListAsync(cancellationToken);
        }

        public async Task<FormItemCoefficient?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await TableNoTracking.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
        }

        public async Task AddAsync(FormItemCoefficient entity, CancellationToken cancellationToken) =>
            await AddAsync(entity, cancellationToken);

        public async Task UpdateAsync(FormItemCoefficient entity, CancellationToken cancellationToken) =>
            await UpdateAsync(entity, cancellationToken);

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity == null) throw new KeyNotFoundException("ضریب مورد نظر یافت نشد");

            // اگر حذف نرم‌افزاری دارید:
            // entity.IsDeleted = true;
            // await UpdateAsync(entity, cancellationToken);

            // اگر حذف فیزیکی:
            await DeleteAsync(entity, cancellationToken);
        }
    }
}
