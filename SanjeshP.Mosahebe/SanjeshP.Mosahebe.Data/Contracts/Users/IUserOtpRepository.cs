using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Contracts.Users
{
    public interface IUserOtpRepository : IEntityFrameworkRepository<UserOtp>
    {
        Task<UserOtp> GetValidOtpAsync(Guid user_ID, string code, CancellationToken cancellationToken);
    }
}
