using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Entities.StudentForm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SanjeshP.Mosahebe.Data.Contracts.StudentForm
{
    public interface IFormItemRepository : IEntityFrameworkRepository<FormItem>
    {
        Task<IEnumerable<FormItem>> GetByFormIdAsync(int formId, CancellationToken cancellationToken);
        Task<FormItem> GetByFormItemIdAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<FormItem>> GetFormItemsWithIncludesAsync(CancellationToken cancellationToken);
        Task AddFormItemAsync(FormItem formItem, CancellationToken cancellationToken);
        Task UpdateFormItemAsync(FormItem formItem, CancellationToken cancellationToken);
        Task DeleteFormItemAsync(int id, CancellationToken cancellationToken);
    }
}
