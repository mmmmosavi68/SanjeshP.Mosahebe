using SanjeshP.Mosahebe.Entities.StudentForm;
using SanjeshP.Mosahebe.Data.Contracts.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Contracts.StudentForm
{
    public interface IFormItemCoefficientRepository : IEntityFrameworkRepository<FormItemCoefficient>
    {
        Task<IEnumerable<FormItemCoefficient>> GetAllAsync(CancellationToken cancellationToken);
        Task<FormItemCoefficient?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task AddAsync(FormItemCoefficient entity, CancellationToken cancellationToken);
        Task UpdateAsync(FormItemCoefficient entity, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
