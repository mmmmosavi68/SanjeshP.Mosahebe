using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Entities.Menu;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Contracts.Menus
{
    public interface IAccessMenusRepository : IEntityFrameworkRepository<UserAccessMenus>
    {
        public Task<List<UserAccessMenus>> GetUserAccessMenusByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
