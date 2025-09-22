using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Data.Contracts.Groups;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Entities.Group;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SanjeshP.Mosahebe.Data.Repositories.Groups
{
    public class GroupRepository : EntityFrameworkRepository<Group>, IGroupRepository, IScopedDependency
    {
        public GroupRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public IReadOnlyList<Group> GetAllGroups()
        {
            //        return TableNoTracking.Include(ur => ur.UserRoles)
            //                    .ThenInclude(r => r.Role)
            //                    .Include(up => up.UserProfiles)
            //                    .Where(u => u.IsDeleted == false)
            //                    .ToList()
            //.ConfigureAwait(false);

            return TableNoTracking
                    .Where(g => g.IsDeleted != true)
                    .OrderByDescending(g => g.CreatedDate) // مرتب‌سازی براساس CreatedDate به‌صورت نزولی
                    .Include(g => g.Creator)
                    .ToList();
        }
    }
}
