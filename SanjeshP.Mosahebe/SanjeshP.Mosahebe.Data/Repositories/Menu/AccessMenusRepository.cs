using Microsoft.EntityFrameworkCore;
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
    public class AccessMenusRepository : EntityFrameworkRepository<UserAccessMenus>, IAccessMenusRepository, IScopedDependency
    {
        protected readonly ApplicationDbContext _dbContext;
        public AccessMenusRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<UserAccessMenus>> GetUserAccessMenusByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            // Fore modify UserAccessMenus table
            // IsDelete don't checked
            return await TableNoTracking.Where(u => u.UserId.Equals(userId)).ToListAsync(cancellationToken);
        }

    }
}
