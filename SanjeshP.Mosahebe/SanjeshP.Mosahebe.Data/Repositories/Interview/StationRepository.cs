using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Contracts.Interview;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Entities.Interview;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SanjeshP.Mosahebe.Data.Repositories.Interview
{
    public class StationRepository : EntityFrameworkRepository<Station>, IStationRepository, IScopedDependency
    {
        public StationRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<Station>> GetAllStationsNoTrackingAsync(CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Where(s => !s.IsDeleted)
                .OrderByDescending(s => s.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<Station?> GetByStationIDAsync(int id, CancellationToken cancellationToken)
        {
            return await Table
                .Where(s => !s.IsDeleted && s.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddStationAsync(Station station, CancellationToken cancellationToken)
        {
            await AddAsync(station, cancellationToken);
        }

        public async Task UpdateStationAsync(Station station, CancellationToken cancellationToken)
        {
            await UpdateAsync(station, cancellationToken);
        }

        public async Task DeleteStationAsync(int id, CancellationToken cancellationToken)
        {
            var station = await GetByStationIDAsync(id, cancellationToken);
            if (station == null)
                throw new KeyNotFoundException("ایستگاه مورد نظر یافت نشد");

            station.IsDeleted = true;
            await UpdateAsync(station, cancellationToken);
        }
    }
}
