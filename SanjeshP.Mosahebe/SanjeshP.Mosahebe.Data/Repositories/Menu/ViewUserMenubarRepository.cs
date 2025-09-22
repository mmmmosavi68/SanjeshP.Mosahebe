using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Data.Contracts.Menus;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Common.Utilities;
using SanjeshP.Mosahebe.Entities.Menu;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Repositories.Menus
{
    public class ViewUserMenubarRepository : EntityFrameworkRepository<View_UserMenubar>, IViewUserMenubarRepository, IScopedDependency
    {
        public ViewUserMenubarRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public Task<List<View_UserMenubar>> GetUserMenusByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return TableNoTracking.Where(u => u.Id.Equals(userId))
                .Where(vum => vum.Person_Checkecd.Equals(true))
                .OrderBy(u => u.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<View_UserMenubar>> GetUserSelectedAccessMenusByUserIdAsync(Guid userCurrentUserId, Guid userSelectedUserId, CancellationToken cancellationToken)
        {
            CreateSqlParameter createSqlParameter = new CreateSqlParameter();
            SqlParameter[] param_userid = new SqlParameter[2];
            param_userid[0] = createSqlParameter.Create_Param("@CurrentUserId", userCurrentUserId, SqlDbType.UniqueIdentifier, 16);
            param_userid[1] = createSqlParameter.Create_Param("@UserSelected_UserId", userSelectedUserId, SqlDbType.UniqueIdentifier, 16);
            return await Entities.FromSqlRaw($"[dbo].[Get_UserSelectedAccess_With_CurrentUserAccess_For_TreeCheckBox] @CurrentUserId,@UserSelected_UserId", param_userid).AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<List<View_UserMenubar>> GetGroupSelectedAccessMenusByGroupIdAsync(Guid userCurrentUserId, Guid groupSelectedGroupId, CancellationToken cancellationToken)
        {
            CreateSqlParameter createSqlParameter = new CreateSqlParameter();
            SqlParameter[] param_userid = new SqlParameter[2];
            param_userid[0] = createSqlParameter.Create_Param("@CurrentUserId", userCurrentUserId, SqlDbType.UniqueIdentifier, 16);
            param_userid[1] = createSqlParameter.Create_Param("@GroupSelected_GroupId", groupSelectedGroupId, SqlDbType.UniqueIdentifier, 16);

            return await Entities.FromSqlRaw($"[dbo].[Get_GroupAccess_With_CurrentUserAccess_For_TreeCheckBox] @CurrentUserId,@GroupSelected_GroupId", param_userid).AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<bool> IsUserAuthorizedForMenuAsync(Guid userId, string controllerName, string actionName, CancellationToken cancellationToken)
        {
            CreateSqlParameter createSqlParameter = new CreateSqlParameter();
            SqlParameter[] param_userid = new SqlParameter[1];
            param_userid[0] = createSqlParameter.Create_Param("@CurrentUserId", userId, SqlDbType.UniqueIdentifier, 16);

            var result = await Entities.FromSqlRaw($"EXEC Get_UserCurrentAccess @CurrentUserId", param_userid).AsNoTracking().ToListAsync(cancellationToken);
            return result.Any(u => u.ControllerName == controllerName
                                 && u.ActionName == actionName
                                 && (u.Person_Checkecd == true || u.Group_Checkecd == true));
        }

        public List<View_UserMenubar> GetUserAccessMenus(Guid userId, CancellationToken cancellationToken)
        {
            CreateSqlParameter createSqlParameter = new CreateSqlParameter();
            SqlParameter[] param_userid = new SqlParameter[1];
            param_userid[0] = createSqlParameter.Create_Param("@CurrentUserId", userId, SqlDbType.UniqueIdentifier, 16);

            var result = Entities.FromSqlRaw($"EXEC Get_UserCurrentAccess @CurrentUserId", param_userid).AsNoTracking().ToList();
            return result;
        }
    }
}
