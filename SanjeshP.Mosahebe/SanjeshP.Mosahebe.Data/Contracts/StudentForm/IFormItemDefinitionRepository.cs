using SanjeshP.Mosahebe.Entities.StudentForm;
using SanjeshP.Mosahebe.Data.Contracts.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Contracts.StudentForm
{
    public interface IFormItemDefinitionRepository : IEntityFrameworkRepository<FormItemDefinition>
    {
        Task<IEnumerable<FormItemDefinition>> GetByFormItemIdAsync(int formItemId, CancellationToken cancellationToken);
        Task<FormItemDefinition?> GetByDefinitionIdAsync(int id, CancellationToken cancellationToken);
        Task AddDefinitionAsync(FormItemDefinition entity, CancellationToken cancellationToken);
        Task UpdateDefinitionAsync(FormItemDefinition entity, CancellationToken cancellationToken);
        Task DeleteDefinitionAsync(int id, CancellationToken cancellationToken);
    }
}
