using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Entities.User;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Repositories.Users
{
    public class UserOtpRepository : EntityFrameworkRepository<UserOtp>, IUserOtpRepository, IScopedDependency
    {
        public UserOtpRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<UserOtp> GetValidOtpAsync(Guid user_ID, string code, CancellationToken cancellationToken)
        {
            return await Table.Where(p => p.UserId == user_ID && p.Code == code)
              .SingleOrDefaultAsync(cancellationToken)
              .ConfigureAwait(false);
        }
    }
}
