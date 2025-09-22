using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Repositories.Users
{
    public class UserRoleRepository : EntityFrameworkRepository<UserRole>, IUserRoleRepository, IScopedDependency
    {
        public UserRoleRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public UserRole GetUserRoleByUserId(Guid userId)
        {
            return TableNoTracking.Where(ur => ur.IsDeleted == false && ur.UserId == userId).FirstOrDefault();
        }
    }
}
