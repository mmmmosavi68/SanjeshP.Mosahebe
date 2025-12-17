using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Common.Exceptions;
using SanjeshP.Mosahebe.Common.Utilities;
using SanjeshP.Mosahebe.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Repositories.Users
{
    public class UserProfileRepository : EntityFrameworkRepository<UserProfile>, IUserProfileRepository, IScopedDependency
    {
        public UserProfileRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<UserProfile> GetProfileByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await Table.Where(p => p.Id == id).SingleOrDefaultAsync(cancellationToken);
        }
        public async Task<UserProfile> GetProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await Table.Where(p => p.UserId == userId).SingleOrDefaultAsync(cancellationToken);
        }
        public async Task<UserProfile> GetProfileByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken)
        {
            return await Table.Where(p => p.NationalCode == nationalCode).SingleOrDefaultAsync(cancellationToken);
        }
        public async Task AddUserProfileAsync(UserProfile userProfile, Guid userId, CancellationToken cancellationToken)
        {
            userProfile.UserId = userId;
            await base.AddAsync(userProfile, cancellationToken);
        }
        public async Task UpdateUserProfileAsync(UserProfile userProfile, int id, CancellationToken cancellationToken)
        {
            await Table.Where(p => p.Id == id).SingleOrDefaultAsync(cancellationToken);
        }
        public async Task DeleteUserProfileAsync(Guid userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<List<string>> GetAllNationalCodesAsync(CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Where(x => x.NationalCode != null)
                .Select(x => x.NationalCode)
                .ToListAsync(cancellationToken);
        }

    }
}
