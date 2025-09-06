using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Entities.User;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Contracts.Users
{
    public interface IUserTokenRepository : IEntityFrameworkRepository<UserToken>
    {
        Task<List<UserToken>> GetUserTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<UserToken> GetUserTokenByIdAsync(Guid id, CancellationToken cancellationToken);
        Task AddUserTokenAsync(UserToken token, CancellationToken cancellationToken, bool saveNow = true);
        Task UpdateTokenDateTimeAsync(Guid id, CancellationToken cancellationToken);
        Task DeleteUserTokenByIdAsync(Guid id, CancellationToken cancellationToken);
        Task DeleteUserTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task GetTokenByCodeAsync(string verificationCode, CancellationToken cancellationToken);
        Task<List<UserToken>> GetExpiredTokensAsync(CancellationToken cancellationToken);
        Task MarkExpiredTokensAsDeletedAsync(Guid userId, CancellationToken cancellationToken);
    }
}
