using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Entities.User;
using System;

namespace SanjeshP.Mosahebe.Data.Contracts.Users
{
    public interface IUserRoleRepository : IEntityFrameworkRepository<UserRole>
    {
        public UserRole GetUserRoleByUserId(Guid userId);
    }
}
