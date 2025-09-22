using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Data.Contracts.SiteSetting;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Entities.SiteSetting;
using SanjeshP.Mosahebe.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Repositories.SiteSetting
{
    public class LoginSettingsRepository : EntityFrameworkRepository<LoginSettings>, ILoginSettingsRepository, IScopedDependency
    {
        public LoginSettingsRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<LoginSettings> GetActiveLoginSetting(CancellationToken cancellationToken)
        {
            return await Table.Where(p => p.IsActive == true && p.IsDeleted == false)
              .SingleOrDefaultAsync(cancellationToken)
              .ConfigureAwait(false);
        }
    }
}