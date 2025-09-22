using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Data.Contracts.Menu;
using SanjeshP.Mosahebe.Data.Contracts.Menus;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Entities.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Repositories.Menus
{
    public class AccessMenusGroupRepository : EntityFrameworkRepository<GroupAccessMenus>, IAccessMenusGroupRepository, IScopedDependency
    {
        public AccessMenusGroupRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<List<GroupAccessMenus>> GetGroupAccessMenusByGroupIdAsync(Guid groupId, CancellationToken cancellationToken)
        {
            return await TableNoTracking.Where(u => u.GroupId.Equals(groupId)).ToListAsync(cancellationToken);
        }
    }
}
