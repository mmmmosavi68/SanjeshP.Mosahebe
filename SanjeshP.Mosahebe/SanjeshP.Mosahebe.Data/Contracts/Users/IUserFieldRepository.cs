using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SanjeshP.Mosahebe.Data.Contracts.Users
{
    public interface IUserFieldRepository : IEntityFrameworkRepository<UserField>
    {
        Task<List<UserField>> GetExamFieldsByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
