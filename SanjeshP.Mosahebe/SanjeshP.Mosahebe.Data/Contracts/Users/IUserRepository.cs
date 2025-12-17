using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Entities.User;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Contracts.Users
{
    public interface IUserRepository : IEntityFrameworkRepository<User>
    {
        Task<IEnumerable<User>> GetAllUsersNoTrackingAsync(CancellationToken cancellationToken);
        Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken);

        Task<User> GetByGuidIdAsync(Guid id, CancellationToken cancellationToken);
        Task<User> GetByGuidIdTrackedAsync(Guid id, CancellationToken cancellationToken);

        Task<User> GetUserByUserNameAsync(string userNameUpper, CancellationToken cancellationToken);
        Task<(List<User>, int totalCount)> GetPagedUsersAsync(SearchPagingParameters parameters, CancellationToken cancellationToken);

        User GetUserByUserName(string userNameUpper);
        Task<User> GetUserByUserNameAndPasswordAsync(string userName, string passwordHash, CancellationToken cancellationToken);
        Task<User> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<User> GetUserByEmailAsync(string emailUpper, CancellationToken cancellationToken);
        User GetUserByEmail(string emailUpper);
        Task<User> GetUserByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken);
        User GetUserByPhoneNumber(string phoneNumber);
        Task UpdateUserAsync(User user, CancellationToken cancellationToken);
        Task DeleteUserAsync(User user, CancellationToken cancellationToken);
        Task UpdateUserSecurityStampAsync(User user, CancellationToken cancellationToken);
        Task UpdateUserLastLoginDateAsync(User user, CancellationToken cancellationToken);

        Task<List<string>> GetAllEmailsAsync(CancellationToken cancellationToken);
        Task<List<string>> GetAllUserNamesAsync(CancellationToken cancellationToken);
        Task<List<string>> GetAllPhonesAsync(CancellationToken cancellationToken);


    }
}
