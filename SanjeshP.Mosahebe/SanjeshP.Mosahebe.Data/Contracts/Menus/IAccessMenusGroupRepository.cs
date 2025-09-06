using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Entities.Menu;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Contracts.Menu
{
    public interface IAccessMenusGroupRepository : IEntityFrameworkRepository<GroupAccessMenus>
    {
        public Task<List<GroupAccessMenus>> GetGroupAccessMenusByGroupIdAsync(Guid groupId, CancellationToken cancellationToken);
    }
}
