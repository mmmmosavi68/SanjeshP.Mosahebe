using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Entities.Exam;
using SanjeshP.Mosahebe.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Repositories.Users
{
    public class UserFieldRepository : EntityFrameworkRepository<UserField>, IUserFieldRepository, IScopedDependency
    {

        public UserFieldRepository(ApplicationDbContext dbContext)
           : base(dbContext)
        {
        }
        public async Task<List<UserField>> GetExamFieldsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await Table
                .Where(uf => !uf.IsDeleted && uf.UserID == userId)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
