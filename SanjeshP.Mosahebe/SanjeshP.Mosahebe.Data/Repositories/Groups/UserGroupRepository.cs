using SanjeshP.Mosahebe.Data.Contracts.Groups;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Entities.Group;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Data.Repositories.Groups
{
    public class UserGroupRepository : EFRepository<GroupUsers>, IUserGroupRepository, IScopedDependency
    {
        public UserGroupRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }
}
