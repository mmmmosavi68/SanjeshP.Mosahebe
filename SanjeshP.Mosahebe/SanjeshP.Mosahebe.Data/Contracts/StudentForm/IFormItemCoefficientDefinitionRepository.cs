using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Entities.StudentForm;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Contracts.StudentForm
{
    public interface IFormItemCoefficientDefinitionRepository : IEntityFrameworkRepository<FormItemCoefficientDefinition>
    {
        Task<IEnumerable<FormItemCoefficientDefinition>> GetByCoefficientIdAsync(int coefficientId, CancellationToken cancellationToken);
        Task<FormItemCoefficientDefinition?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task AddAsync(FormItemCoefficientDefinition entity, CancellationToken cancellationToken);
        Task UpdateAsync(FormItemCoefficientDefinition entity, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
