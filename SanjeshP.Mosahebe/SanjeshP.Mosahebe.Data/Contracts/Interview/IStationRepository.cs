using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Entities.Interview;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SanjeshP.Mosahebe.Data.Contracts.Interview
{
    public interface IStationRepository : IEntityFrameworkRepository<Station>
    {
        Task<IEnumerable<Station>> GetAllStationsNoTrackingAsync(CancellationToken cancellationToken);
        Task<IEnumerable<Station>> GetStationsByFieldIdAsync(int fieldId,CancellationToken cancellationToken);

        Task<Station?> GetByStationIDAsync(int id, CancellationToken cancellationToken);
        Task AddStationAsync(Station station, CancellationToken cancellationToken);
        Task UpdateStationAsync(Station station, CancellationToken cancellationToken);
        Task DeleteStationAsync(int id, CancellationToken cancellationToken);
    }
}
