using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Entities.Interview;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Contracts.Interview
{
    public interface IInterviewScoreRepository : IEntityFrameworkRepository<InterviewScore>
    {
        Task<IEnumerable<InterviewScore>> GetAllNoTrackingAsync(CancellationToken cancellationToken);
        Task<InterviewScore?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task AddScoreAsync(InterviewScore score, CancellationToken cancellationToken);
        Task UpdateScoreAsync(InterviewScore score, CancellationToken cancellationToken);
        Task DeleteScoreAsync(int id, CancellationToken cancellationToken);
        Task SaveInterviewScoresAsync(Guid userId, int studentId,int stationId,Dictionary<int, double> scores,string hostIP,CancellationToken cancellationToken);
        Task<List<InterviewScore>> GetInterviewScoresAsync(IEnumerable<int> studentIds, int stationId, CancellationToken cancellationToken);
    }
}
