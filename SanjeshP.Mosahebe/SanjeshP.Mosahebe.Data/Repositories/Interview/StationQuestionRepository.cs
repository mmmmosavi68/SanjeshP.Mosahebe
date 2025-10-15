using SanjeshP.Mosahebe.Data.Contracts.Interview;
using SanjeshP.Mosahebe.Entities.Interview;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SanjeshP.Mosahebe.Data.Repositories.Interview
{
    public class StationQuestionRepository : EntityFrameworkRepository<StationQuestion>, IStationQuestionRepository, IScopedDependency
    {
        public StationQuestionRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<StationQuestion>> GetAllQuestionsByStationIdAsync(int stationId, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Where(q => !q.IsDeleted && q.StationId == stationId)
                .OrderBy(q => q.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<StationQuestion?> GetByQuestionIdAsync(int id, CancellationToken cancellationToken)
        {
            return await Table
                .Where(q => !q.IsDeleted && q.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddQuestionAsync(StationQuestion question, CancellationToken cancellationToken)
        {
            await AddAsync(question, cancellationToken);
        }

        public async Task UpdateQuestionAsync(StationQuestion question, CancellationToken cancellationToken)
        {
            await UpdateAsync(question, cancellationToken);
        }

        public async Task DeleteQuestionAsync(int id, CancellationToken cancellationToken)
        {
            var question = await GetByQuestionIdAsync(id, cancellationToken);
            if (question == null)
                throw new KeyNotFoundException("سؤال مورد نظر یافت نشد");

            question.IsDeleted = true;
            await UpdateAsync(question, cancellationToken);
        }
    }
}
