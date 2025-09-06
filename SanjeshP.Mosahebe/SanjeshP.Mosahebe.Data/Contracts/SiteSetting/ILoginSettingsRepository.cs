using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Entities.Menu;
using SanjeshP.Mosahebe.Entities.SiteSetting;
using SanjeshP.Mosahebe.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Contracts.SiteSetting
{
    public interface ILoginSettingsRepository : IEntityFrameworkRepository<LoginSettings>
    {
        Task<LoginSettings> GetActiveLoginSetting(CancellationToken cancellationToken);
    }
}
