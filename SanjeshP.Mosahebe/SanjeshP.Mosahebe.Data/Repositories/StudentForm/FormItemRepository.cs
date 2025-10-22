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
using Microsoft.EntityFrameworkCore;

namespace SanjeshP.Mosahebe.Data.Repositories.StudentForm
{
    public class FormItemRepository : EntityFrameworkRepository<FormItem>, IFormItemRepository, IScopedDependency
    {
        public FormItemRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<FormItem>> GetByFormIdAsync(int formId, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Where(f => f.Id == formId && !f.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<FormItem> GetByFormItemIdAsync(int id, CancellationToken cancellationToken)
        {
            return await Table
                .Where(f => f.Id == id && !f.IsDeleted)
                .Include(c=>c.FormItemCategory)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddFormItemAsync(FormItem formItem, CancellationToken cancellationToken) =>
            await AddAsync(formItem, cancellationToken);

        public async Task UpdateFormItemAsync(FormItem formItem, CancellationToken cancellationToken) =>
            await UpdateAsync(formItem, cancellationToken);

        public async Task DeleteFormItemAsync(int id, CancellationToken cancellationToken)
        {
            var item = await GetByFormItemIdAsync(id, cancellationToken);
            if (item is null)
                throw new KeyNotFoundException("آیتم فرم مورد نظر یافت نشد");

            item.IsDeleted = true;
            await UpdateAsync(item, cancellationToken);
        }
        public async Task<IEnumerable<FormItem>> GetFormItemsWithIncludesAsync(CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Include(f => f.FormItemCoefficient)
                .Include(f => f.FormItemCategory)
                .Where(f => !f.IsDeleted)
                .ToListAsync(cancellationToken);
        }
    }

}
