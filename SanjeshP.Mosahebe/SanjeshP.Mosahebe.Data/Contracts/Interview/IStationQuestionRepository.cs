using SanjeshP.Mosahebe.Entities.Interview;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SanjeshP.Mosahebe.Data.Contracts.Common;

namespace SanjeshP.Mosahebe.Data.Contracts.Interview
{
    public interface IStationQuestionRepository : IEntityFrameworkRepository<StationQuestion>
    {
        Task<IEnumerable<StationQuestion>> GetAllQuestionsByStationIdAsync(int stationId, CancellationToken cancellationToken);
        Task<StationQuestion?> GetByQuestionIdAsync(int id, CancellationToken cancellationToken);
        Task AddQuestionAsync(StationQuestion question, CancellationToken cancellationToken);
        Task UpdateQuestionAsync(StationQuestion question, CancellationToken cancellationToken);
        Task DeleteQuestionAsync(int id, CancellationToken cancellationToken);
    }
}
