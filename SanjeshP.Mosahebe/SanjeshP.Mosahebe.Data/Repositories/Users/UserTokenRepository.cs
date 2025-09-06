using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Repositories.Users
{
    public class UserTokenRepository : EFRepository<UserToken>, IUserTokenRepository, IScopedDependency
    {
        public UserTokenRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<UserToken> GetUserTokenByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await Table.Where(p => p.Id == id)
                .Include(p => p.User)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<List<UserToken>> GetUserTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await Table.Where(p => p.UserId == userId && p.IsDeleted == false).Include(p => p.User).ToListAsync(cancellationToken);
        }

        public async Task AddUserTokenAsync(UserToken token, CancellationToken cancellationToken, bool saveNow = true)
        {
            await DeleteUserTokensByUserIdAsync(token.UserId, cancellationToken);
            await base.AddAsync(token, cancellationToken);
        }

        public async Task UpdateTokenDateTimeAsync(Guid id, CancellationToken cancellationToken)
        {
            var token = await GetUserTokenByIdAsync(id, cancellationToken);
            token.CreatedDate = DateTime.Now;
            await UpdateAsync(token, cancellationToken);
        }

        public async Task DeleteUserTokenByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var token = await GetUserTokenByIdAsync(id, cancellationToken);
            token.IsDeleted = true;
            await UpdateAsync(token, cancellationToken);
        }

        public async Task DeleteUserTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var tokens = await GetUserTokensByUserIdAsync(userId, cancellationToken);
            if (tokens != null)
            {
                foreach (var token in tokens)
                {
                    token.IsDeleted = true;
                    await UpdateAsync(token, cancellationToken);
                }
            }
        }

        public Task GetTokenByCodeAsync(string verificationCode, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<List<UserToken>> GetExpiredTokensAsync(CancellationToken cancellationToken)
        {
            return await TableNoTracking.Where(token => token.ExpirationDate < DateTime.Now && !token.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task MarkExpiredTokensAsDeletedAsync(Guid userId, CancellationToken cancellationToken)
        {
            // دریافت توکن‌های منقضی‌شده
            var expiredTokens = await TableNoTracking
                .Where(token => token.ExpirationDate < DateTime.Now && !token.IsDeleted && token.UserId == userId)
                .ToListAsync(cancellationToken);

            // به‌روزرسانی وضعیت توکن‌ها
            foreach (var token in expiredTokens)
            {
                token.IsDeleted = true;
            }

            // ذخیره تغییرات در پایگاه داده
            await SaveChangesAsync(cancellationToken);
        }

    }
}
