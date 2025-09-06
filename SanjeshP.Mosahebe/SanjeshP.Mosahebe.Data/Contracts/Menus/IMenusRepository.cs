using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Entities.Menu;
using SanjeshP.Mosahebe.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Contracts.Menus
{
    public interface IMenusRepository : IEntityFrameworkRepository<Entities.Menu.Menu>
    {
        public Task<List<Entities.Menu.Menu>> GetAllMenusAsync(CancellationToken cancellationToken);
        public Task<List<Entities.Menu.Menu>> GetAllActiveMenusAsync(CancellationToken cancellationToken);

    }
}
