using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Entities.Exam;
using SanjeshP.Mosahebe.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Repositories.Users
{
    public class UserFieldRepository : EntityFrameworkRepository<UserField>, IUserFieldRepository, IScopedDependency
    {

        public UserFieldRepository(ApplicationDbContext dbContext)
           : base(dbContext)
        {
        }
        public async Task<List<UserField>> GetExamFieldsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await Table
                .Where(uf => !uf.IsDeleted && uf.UserID == userId)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        public async Task<IEnumerable<UserField>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Include(uf => uf.ExamFields)
                .Where(uf => !uf.IsDeleted && uf.UserID == userId)
                .OrderBy(uf => uf.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<UserField>> GetByUserIdsAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken)
        {
            if (userIds == null || !userIds.Any()) return Array.Empty<UserField>();

            return await TableNoTracking
                .Include(uf => uf.ExamFields)
                .Where(uf => !uf.IsDeleted && userIds.Contains(uf.UserID))
                .ToListAsync(cancellationToken);
        }

        public async Task<UserField?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await Table
                .Include(uf => uf.ExamFields)
                .Include(uf => uf.Users)
                .FirstOrDefaultAsync(uf => uf.Id == id && !uf.IsDeleted, cancellationToken);
        }

        public async Task AddUserFieldAsync(UserField entity, CancellationToken cancellationToken)
        {
            await AddAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        public async Task UpdateUserFieldAsync(UserField entity, CancellationToken cancellationToken)
        {
            await UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteUserFieldAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity == null) throw new KeyNotFoundException("UserField not found.");
            await DeleteAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<int>> GetExamFieldIdsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Where(uf => !uf.IsDeleted && uf.UserID == userId)
                .Select(uf => uf.ExamFieldID)
                .ToListAsync(cancellationToken);
        }
    }
}