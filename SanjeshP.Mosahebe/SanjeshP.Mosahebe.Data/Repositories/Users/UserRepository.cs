using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Convertor;
using SanjeshP.Mosahebe.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Repositories.Users
{
    public class UserRepository : EntityFrameworkRepository<User>, IUserRepository, IScopedDependency
    {
        public UserRepository(ApplicationDbContext dbContext)
             : base(dbContext)
        {
        }

        public async Task<IEnumerable<User>> GetAllUsersNoTrackingAsync(CancellationToken cancellationToken)
        {
            return await TableNoTracking.Include(ur => ur.UserRoles)
                .ThenInclude(r => r.Role)
                .Include(up => up.UserProfiles)
                .OrderByDescending(u => u.CreatedDate) // مرتب‌سازی براساس CreatedDate به‌صورت نزولی
                .Where(u => u.IsDeleted == false)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            return await Table.Include(ur => ur.UserRoles)
                .ThenInclude(r => r.Role)
                .Include(up => up.UserProfiles)
                .OrderByDescending(u => u.CreatedDate) // مرتب‌سازی براساس CreatedDate به‌صورت نزولی
                .Where(u => u.IsDeleted == false)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<User> GetUserByUserNameAsync(string userNameUpper, CancellationToken cancellationToken)
        {
            return await Table.Where(p => p.NormalizedUserName == userNameUpper.FixTextUpper())
                .Include(ur => ur.UserRoles)
                .ThenInclude(r => r.Role)
                .Include(up => up.UserProfiles)
                .Where(u => u.IsDeleted == false)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<(List<User>, int totalCount)> GetPagedUsersAsync(SearchPagingParameters parameters, CancellationToken cancellationToken)
        {
            var query = TableNoTracking.Include(ur => ur.UserRoles)
                .ThenInclude(r => r.Role)
                .Include(up => up.UserProfiles)
                .OrderByDescending(u => u.CreatedDate) // مرتب‌سازی براساس CreatedDate به‌صورت نزولی
                .Where(u => !u.IsDeleted);
            // جستجوی عمومی (SearchTerm)، اگر هنوز ازش استفاده می‌کنی
            if (!string.IsNullOrEmpty(parameters.SearchTerm))
            {
                var keyword = parameters.SearchTerm.Trim().ToLower();

                query = query.Where(u =>
                    EF.Functions.Like(u.UserName, $"%{keyword}%") ||
                    EF.Functions.Like(u.EmailAddress, $"%{keyword}%") ||
                    EF.Functions.Like(u.PhoneNumber, $"%{keyword}%") ||
                    u.UserProfiles.Any(p =>
                        EF.Functions.Like(p.FirstName, $"%{keyword}%") ||
                        EF.Functions.Like(p.LastName, $"%{keyword}%") ||
                        EF.Functions.Like(p.NationalCode, $"%{keyword}%")
                    ));
            }

            // اعمال فیلترهای چندستونی دینامیک
            if (parameters.Filters?.Any() == true)
            {
                foreach (var filter in parameters.Filters)
                {
                    var keyword = filter.Value?.Trim().ToLower();
                    if (string.IsNullOrEmpty(keyword)) continue;

                    switch (filter.Key)
                    {
                        case "FirstName":
                            query = query.Where(u => u.UserProfiles.Any(p =>
                                EF.Functions.Like(p.FirstName, $"%{keyword}%")));
                            break;

                        case "LastName":
                            query = query.Where(u => u.UserProfiles.Any(p =>
                                EF.Functions.Like(p.LastName, $"%{keyword}%")));
                            break;

                        case "UserName":
                            query = query.Where(u =>
                                EF.Functions.Like(u.UserName, $"%{keyword}%"));
                            break;

                        case "NationalCode":
                            query = query.Where(u => u.UserProfiles.Any(p =>
                                EF.Functions.Like(p.NationalCode, $"%{keyword}%")));
                            break;

                        case "PhoneNumber":
                            query = query.Where(u =>
                                EF.Functions.Like(u.PhoneNumber, $"%{keyword}%"));
                            break;

                        case "EmailAddress":
                            query = query.Where(u =>
                                EF.Functions.Like(u.EmailAddress, $"%{keyword}%"));
                            break;

                            // می‌تونی فیلترهای اضافه مثل RoleName یا IsActive رو هم اضافه کنی
                    }
                }
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var users = await query
                .OrderByDescending(u => u.CreatedDate)
                .Skip((parameters.PageIndex - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync(cancellationToken);

            return (users, totalCount);
        }



        public User GetUserByUserName(string userNameUpper)
        {
            return Table.Where(p => p.NormalizedUserName == userNameUpper.FixTextUpper())
                .Include(ur => ur.UserRoles)
                .ThenInclude(r => r.Role)
                .Include(up => up.UserProfiles)
                .Where(u => u.IsDeleted == false)
                .SingleOrDefault();
        }

        public async Task<User> GetUserByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await Table.Include(ur => ur.UserRoles)
                .ThenInclude(r => r.Role)
                .Include(up => up.UserProfiles)
                .Where(u => u.IsDeleted == false)
                .Where(p => p.Id == id)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<User> GetUserByEmailAsync(string emailUpper, CancellationToken cancellationToken)
        {
            return await Table.AsNoTracking()
                .Where(u => u.IsDeleted == false && u.NormalizedEmailAddress == emailUpper)
                .Include(ur => ur.UserRoles)
                .ThenInclude(r => r.Role)
                .Include(up => up.UserProfiles)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public User GetUserByEmail(string emailUpper)
        {
            return TableNoTracking.Include(ur => ur.UserRoles)
                .ThenInclude(r => r.Role)
                .Include(up => up.UserProfiles)
                .Where(u => u.IsDeleted == false)
                .Where(p => p.NormalizedEmailAddress == emailUpper)
                .SingleOrDefault();
        }

        public async Task<User> GetUserByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
        {
            return await Table.Include(ur => ur.UserRoles)
                .ThenInclude(r => r.Role)
                .Include(up => up.UserProfiles)
                .Where(u => u.IsDeleted == false)
                .Where(p => p.PhoneNumber == phoneNumber)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public User GetUserByPhoneNumber(string phoneNumber)
        {
            return Table.Include(ur => ur.UserRoles)
                .ThenInclude(r => r.Role)
                .Include(up => up.UserProfiles)
                .Where(u => u.IsDeleted == false)
                .Where(p => p.PhoneNumber == phoneNumber)
                .SingleOrDefault();
        }

        public async Task UpdateUserSecurityStampAsync(User user, CancellationToken cancellationToken)
        {
            user.SecurityStamp = Guid.NewGuid();
            await UpdateAsync(user, cancellationToken).ConfigureAwait(false);
        }

        public override void Update(User entity, bool saveNow = true)
        {
            entity.SecurityStamp = Guid.NewGuid();
            base.Update(entity, saveNow);
        }

        public async Task UpdateUserLastLoginDateAsync(User user, CancellationToken cancellationToken)
        {
            user.LastLoginDate = DateTimeOffset.Now;
            await UpdateAsync(user, cancellationToken).ConfigureAwait(false);
        }

        public async Task UpdateUserAsync(User user, Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteUserAsync(User user, CancellationToken cancellationToken)
        {
            user.IsDeleted = true;
            await UpdateAsync(user, cancellationToken).ConfigureAwait(false);
        }

        public async Task<User> GetUserByUserNameAndPasswordAsync(string userName, string passwordHash, CancellationToken cancellationToken)
        {
            return await Table.Where(p => p.UserName == userName && p.PasswordHash == passwordHash)
                .Include(ur => ur.UserRoles)
                .ThenInclude(r => r.Role)
                .Include(up => up.UserProfiles)
                .Where(u => u.IsDeleted == false)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<User> GetByGuidIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await TableNoTracking.Include(ur => ur.UserRoles)
               .ThenInclude(r => r.Role)
               .Include(up => up.UserProfiles)
               .Include(u => u.CreatorUser)
               .Include(u => u.EditorUser)
               .Where(u => u.IsDeleted == false && u.Id == id)
               .FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
               .ConfigureAwait(false);
        }


    }
}
