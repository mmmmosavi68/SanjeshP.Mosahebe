using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Contracts.Interview;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Entities.Interview;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SanjeshP.Mosahebe.Data.Repositories.Interview
{
    public class FieldStationRepository : EntityFrameworkRepository<FieldStation>, IFieldStationRepository, IScopedDependency
    {
        public FieldStationRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<FieldStation>> GetFieldsByStationIdAsync(int stationId, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Include(fs => fs.ExamField)
                .Where(fs => fs.StationId == stationId)
                .OrderBy(fs => fs.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<FieldStation?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await Table
                .Include(fs => fs.ExamField)
                .Include(fs => fs.Station)
                .FirstOrDefaultAsync(fs => fs.Id == id, cancellationToken);
        }

        public async Task AddFieldStationAsync(FieldStation entity, CancellationToken cancellationToken)
        {
            await AddAsync(entity, cancellationToken);
        }

        public async Task UpdateFieldStationAsync(FieldStation entity, CancellationToken cancellationToken)
        {
            await UpdateAsync(entity, cancellationToken);
        }

        public async Task DeleteFieldStationAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity == null)
                throw new KeyNotFoundException("رشته‌ای برای ایستگاه پیدا نشد.");

            await DeleteAsync(entity, cancellationToken);
        }
    }
}