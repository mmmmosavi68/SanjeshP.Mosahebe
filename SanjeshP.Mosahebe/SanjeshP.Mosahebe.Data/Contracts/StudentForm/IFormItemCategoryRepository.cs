using SanjeshP.Mosahebe.Entities.StudentForm;
using SanjeshP.Mosahebe.Data.Contracts.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Contracts.StudentForm
{
    public interface IFormItemCategoryRepository : IEntityFrameworkRepository<FormItemCategory>
    {
        Task<IEnumerable<FormItemCategory>> GetAllAsync(CancellationToken cancellationToken);
        Task<FormItemCategory?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task AddAsync(FormItemCategory entity, CancellationToken cancellationToken);
        Task UpdateAsync(FormItemCategory entity, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
