using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Entities.User;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Contracts.Users
{
    public interface IUserProfileRepository : IEntityFrameworkRepository<UserProfile>
    {
        Task<UserProfile> GetProfileByIdAsync(int id, CancellationToken cancellationToken);
        Task<UserProfile> GetProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<UserProfile> GetProfileByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken);
        Task AddUserProfileAsync(UserProfile userProfile, Guid userId, CancellationToken cancellationToken);
        Task UpdateUserProfileAsync(UserProfile userProfile, int id, CancellationToken cancellationToken);
        Task DeleteUserProfileAsync(Guid id, CancellationToken cancellationToken);

        Task<List<string>> GetAllNationalCodesAsync(CancellationToken cancellationToken);

    }
}
