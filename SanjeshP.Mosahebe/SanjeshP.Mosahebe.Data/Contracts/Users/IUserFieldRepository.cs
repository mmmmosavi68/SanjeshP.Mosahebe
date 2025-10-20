using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SanjeshP.Mosahebe.Data.Contracts.Users
{
    public interface IUserFieldRepository : IEntityFrameworkRepository<UserField>
    {
        Task<List<UserField>> GetExamFieldsByUserIdAsync(Guid userId, CancellationToken cancellationToken);

        Task<IEnumerable<UserField>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<UserField?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task AddUserFieldAsync(UserField entity, CancellationToken cancellationToken);
        Task UpdateUserFieldAsync(UserField entity, CancellationToken cancellationToken);
        Task DeleteUserFieldAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<UserField>> GetByUserIdsAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken);
        Task<IEnumerable<int>> GetExamFieldIdsByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
