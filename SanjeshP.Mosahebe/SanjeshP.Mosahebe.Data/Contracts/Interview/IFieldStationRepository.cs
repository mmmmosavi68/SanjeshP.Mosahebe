using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Entities.Interview;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SanjeshP.Mosahebe.Data.Contracts.Common;

namespace SanjeshP.Mosahebe.Data.Contracts.Interview
{
    public interface IFieldStationRepository : IEntityFrameworkRepository<FieldStation>
    {
        Task<IEnumerable<FieldStation>> GetFieldsByStationIdAsync(int stationId, CancellationToken cancellationToken);
        Task<FieldStation?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task AddFieldStationAsync(FieldStation entity, CancellationToken cancellationToken);
        Task UpdateFieldStationAsync(FieldStation entity, CancellationToken cancellationToken);
        Task DeleteFieldStationAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<FieldStation>> GetStationsByUserFieldsAsync(IEnumerable<int> examFieldIds, CancellationToken cancellationToken);
    }
}
