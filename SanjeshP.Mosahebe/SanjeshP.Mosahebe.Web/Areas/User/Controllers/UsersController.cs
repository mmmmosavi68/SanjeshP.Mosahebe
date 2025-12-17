using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SanjeshP.Mosahebe.Web.Areas.User.ViewModels.Menu;
using SanjeshP.Mosahebe.Web.Areas.User.ViewModels.User;
using SanjeshP.Mosahebe.Web.SharedViewModels;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Convertor;
using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Data.Contracts.Menus;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using SanjeshP.Mosahebe.Entities.Menu;
using SanjeshP.Mosahebe.Entities.User;
using SanjeshP.Mosahebe.Security;
using SanjeshP.Mosahebe.WebFramework.Api;
using SanjeshP.Mosahebe.WebFramework.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using SanjeshP.Mosahebe.Data.Contracts.Interview;
using SanjeshP.Mosahebe.Entities.Student;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using SanjeshP.Mosahebe.Data.Repositories.Users;
using SkiaSharp;
using SanjeshP.Mosahebe.Data.Contracts.Exam;

namespace SanjeshP.Mosahebe.Web.Areas.User.Controllers
{
    [Area("User")]
    public class UsersController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly ILogger<UsersController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IUserProfileRepository _userProfilesRepository;
        private readonly IEntityFrameworkRepository<Role> _eFRepositoryRole;
        private readonly IMenusRepository _menuRepository;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly IAccessMenusRepository _accessMenuRepository;
        private readonly IUserRoleRepository _userRoleReository;
        private readonly IMemoryCache _memoryCache;
        private readonly IStationRepository _stationRepository;
        private readonly IUserFieldRepository _userFieldRepository;
        private readonly DbContext _dbContext;
        private readonly IExamFieldRepository _examFieldRepository;
        private readonly IViewUserMenubarRepository _view_UserMenubarRepository;

        public UsersController(IMapper mapper
                                , ICompositeViewEngine viewEngine
                                , ILogger<UsersController> logger
                                , IUserRepository userRepository
                                , IUserProfileRepository userProfilesRepository
                                , IEntityFrameworkRepository<Role> eFRepositoryRole
                                , IMenusRepository menuRepository
                                , IViewUserMenubarRepository view_UserMenubarRepository
                                , IUserTokenRepository userTokenRepository
                                , IAccessMenusRepository accessMenuRepository
                                , IUserRoleRepository userRoleReository
                                , IMemoryCache memoryCache
                                , IStationRepository stationRepository
                                , IUserFieldRepository userFieldRepository
                                , DbContext dbContext
                                , IExamFieldRepository examFieldRepository)
        {
            _mapper = mapper;
            _viewEngine = viewEngine;
            _logger = logger;
            _userRepository = userRepository;
            _userProfilesRepository = userProfilesRepository;
            _eFRepositoryRole = eFRepositoryRole;
            _menuRepository = menuRepository;
            _userTokenRepository = userTokenRepository;
            _accessMenuRepository = accessMenuRepository;
            _userRoleReository = userRoleReository;
            _memoryCache = memoryCache;
            this._stationRepository = stationRepository;
            this._userFieldRepository = userFieldRepository;
            this._dbContext = dbContext;
            this._examFieldRepository = examFieldRepository;
            _view_UserMenubarRepository = view_UserMenubarRepository;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            return View();
        }

        [Obsolete("این متد منسوخ شده و نباید استفاده شود. لطفاً از متد UsersListPaged استفاده کنید.")]
        public async Task<IActionResult> UsersList(CancellationToken cancellationToken)
        {
            try
            {
                var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
                var hasAccess = _memoryCache.HasAccess(token, "UsersList");
                ViewData["MemoryCache"] = _memoryCache;
                var users = await _userRepository.GetAllUsersNoTrackingAsync(cancellationToken);

                List<UserViewModel> newList = users.Select(user => new UserViewModel
                {
                    UserId = user.Id,
                    FirstName = user.UserProfiles.Select(p => p.FirstName).FirstOrDefault(),
                    LastName = user.UserProfiles.Select(p => p.LastName).FirstOrDefault(),
                    NationalCode = user.UserProfiles.Select(p => p.NationalCode).FirstOrDefault(),
                    UserName = user.UserName,
                    EmailAddress = user.EmailAddress,
                    PhoneNumber = user.PhoneNumber,
                    UserTypeTitle = user.UserRoles.Select(p => p.Role.RoleNameFa).Last(),
                    RoleId = user.UserRoles.Select(p => p.RoleId).Last(),
                    IsActive = user.IsActive,
                    IsActiveName = (IsActiveNameType)(user.IsActive ? 1 : 0),

                }).ToList();
                return PartialView("UsersList", newList);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public class UserSearchFilter
        {
            [FromQuery(Name = "pageIndex")]
            public int PageIndex { get; set; } = 1;

            [FromQuery(Name = "pageSize")]
            public int PageSize { get; set; } = 10;

            [FromQuery(Name = "sortColumn")]
            public string SortColumn { get; set; }

            [FromQuery(Name = "sortAscending")]
            public bool SortAscending { get; set; } 
        }
        public async Task<IActionResult> UsersListPaged([FromQuery] UserSearchFilter filter, CancellationToken cancellationToken = default)
        {
            // جلوگیری از نال بودن مدل
            filter ??= new UserSearchFilter();

            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            var memoryCache = HttpContext.RequestServices.GetService<IMemoryCache>();

            // سطح دسترسی برای دکمه "افزودن کاربر"
            ViewData["CreateUser"] = memoryCache.HasAccess(token, "CreateUser");


            var mainParams = new[] { "pageIndex", "pageSize", "sortColumn", "sortAscending", "_" };

            // دریافت همه QueryStringها بجز pageIndex/pageSize
            var filters = Request.Query
                .Where(kv => kv.Key != "pageIndex" && kv.Key != "pageSize")
                .ToDictionary(kv => kv.Key, kv => kv.Value.ToString()?.Trim());

            var parameters = new SearchPagingParameters
            {
                PageIndex = filter.PageIndex,
                PageSize = filter.PageSize,
                Filters = filters,
                SortColumn = filter.SortColumn,
                SortAscending = filter.SortAscending
            };

            parameters.SortAscending= filter.SortAscending;
            // دریافت کاربران با جستجوهای چندفیلدی و صفحه‌بندی
            var (users, totalCount) = await _userRepository.GetPagedUsersAsync(parameters, cancellationToken);

            // ساخت لیست ViewModel با سطح دسترسی
            var viewModels = users.Select(user =>
            {
                //var vm = _mapper.Map<UserViewModel>(user);
                UserViewModel vm = new()
                {
                    UserId = user.Id,
                    FirstName = user.UserProfiles.Select(p => p.FirstName).FirstOrDefault(),
                    LastName = user.UserProfiles.Select(p => p.LastName).FirstOrDefault(),
                    NationalCode = user.UserProfiles.Select(p => p.NationalCode).FirstOrDefault(),
                    UserName = user.UserName,
                    EmailAddress = user.EmailAddress,
                    PhoneNumber = user.PhoneNumber,
                    IsActive = user.IsActive,
                    IsActiveName = (IsActiveNameType)(user.IsActive ? 1 : 0),
                    UserFieldsTitles = string.Join(" - ",
                    user.UserFields
                        .Select(uf => uf.ExamFields.Title) // مستقیم به Title دسترسی پیدا کنید
                        .Distinct())
                };

                var lastRole = user.UserRoles.LastOrDefault();
                vm.UserTypeTitle = lastRole?.Role?.RoleNameFa;
                vm.RoleId = lastRole?.RoleId ?? 0;

                return new ListItemAccessViewModel<UserViewModel>
                {
                    Data = vm,
                    Permissions = new Dictionary<string, bool>
            {
                { "EditUser", memoryCache.HasAccess(token, "EditUser") },
                { "DeleteUser", memoryCache.HasAccess(token, "DeleteUser") },
                { "DetailUser", memoryCache.HasAccess(token, "DetailUser") },
                { "UserAccessMenu", memoryCache.HasAccess(token, "UserAccessMenu") }
            }
                };
            }).ToList();

            // ساخت مدل نهایی همراه با اطلاعات صفحه‌بندی
            var result = new PagedListAccessViewModel<UserViewModel>
            {
                Items = viewModels,
                TotalCount = totalCount,
                PageIndex = filter.PageIndex, // تغییر از pageIndex به filter.PageIndex
                PageSize = filter.PageSize,   // تغییر از pageSize به filter.PageSize
                SortColumn = filter.SortColumn, // اضافه کنید به کلاس مدل
                SortAscending = filter.SortAscending // اضافه کنید به کلاس مدل
            };

            return PartialView("UsersListPaged", result);
        }

        public async Task<IActionResult> DetailUser(Guid userid, CancellationToken cancellationToken)
        {
            try
            {
                if (userid == Guid.Empty)
                {
                    return NotFound();
                }
                var user = await _userRepository.GetByGuidIdAsync(userid, cancellationToken);
                //var user1 = await _userRepository.GetByGuidIdAsync(userid,cancellationToken);
                if (user == null)
                {
                    return NotFound();
                }
                var registerDto = new UserDetailViewModel
                {
                    UserId = user.Id,
                    FirstName = user.UserProfiles.Any() ? user.UserProfiles.Select(p => p.FirstName).FirstOrDefault() : null,
                    LastName = user.UserProfiles.Any() ? user.UserProfiles.Select(p => p.LastName).FirstOrDefault() : null,
                    NationalCode = user.UserProfiles.Any() ? user.UserProfiles.Select(p => p.NationalCode).FirstOrDefault() : null,
                    UserName = user.UserName,
                    EmailAddress = user.EmailAddress,
                    PhoneNumber = user.PhoneNumber,
                    UserTypeTitle = user.UserRoles.Any() ? user.UserRoles.Select(p => p.Role.RoleNameFa).Last() : null,
                    IsActiveName = (IsActiveNameType)(user.IsActive ? 1 : 0),
                    CreatedByUserName = user.CreatorUser?.UserName,
                    CreateDate = user.CreatedDate.ToShamsiDateTime(),
                    EditedByUserName = user.EditorUser?.UserName,
                    EditDate = user.EditDate == null || user.EditDate == DateTime.MinValue ? "- - - -" : user.EditDate.ToShamsiDateTime(),
                    RoleId = user.UserRoles.Any() ? user.UserRoles.Select(p => p.RoleId).Last() : 0,
                    IsActive = user.IsActive,

                };
                return PartialView("DetailUser", registerDto);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #region Add User
        public IActionResult CreateUser()
        {
            var roles = _eFRepositoryRole.TableNoTracking.ToList();
            var model = new UserCreateViewModel();
            roles.Insert(0, new Role
            {
                Id = 0,
                RoleNameFa = "..."
            });
            ViewBag.ListofRoles = roles;

            return PartialView("CreateUser", model); // تغییر به PartialView
        }

        [HttpPost]
        public async Task<ApiResult> CreateUser([Bind("FirstName,LastName,NationalCode,UserName,Password,EmailAddress,PhoneNumber,RoleId,IsActive")] UserCreateViewModel userCreateViewModel, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            var curentUserToken = User.Identity.FindFirstValue("Token");
            var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(curentUserToken), cancellationToken);
            userCreateViewModel = await CheckValidation(userCreateViewModel, token.UserId, cancellationToken);
            if (!ModelState.IsValid)
            {
                var role = _eFRepositoryRole.TableNoTracking.ToList();
                role.Insert(0, new Role
                {
                    Id = 0,
                    RoleNameFa = "..."
                });
                ViewBag.ListofRoles = role;
                try
                {
                    var validationSummary = string.Join("<br>", ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage));

                    return new BadRequestObjectResult(ModelState);

                }
                catch (Exception ex)
                {

                    throw;
                }


            }

            Entities.User.User user = new Entities.User.User()
            {
                Id = Guid.NewGuid(),
                UserName = userCreateViewModel.UserName,
                NormalizedUserName = userCreateViewModel.UserName.FixTextUpper(),
                EmailAddress = userCreateViewModel.EmailAddress,
                NormalizedEmailAddress = userCreateViewModel.EmailAddress.FixTextUpper(),
                EmailAddressConfirmed = false,
                PasswordHash = PasswordHelper.HashPasswordBCrypt(userCreateViewModel.Password),
                ConcurrencyStamp = Guid.NewGuid(),
                PhoneNumber = userCreateViewModel.PhoneNumber,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnd = null,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                CreatedBy = token.UserId,
                CreatorUser = token.User,
                HostIp = Request.HttpContext.Connection.RemoteIpAddress.ToString()
            };

            var userProfile = _mapper.Map<UserProfile>(userCreateViewModel);
            userProfile.UserId = user.Id;
            userProfile.CreatedBy = user.CreatedBy;

            UserRole userRole = new UserRole()
            {
                UserId = user.Id,
                RoleId = userCreateViewModel.RoleId,
                IsActive = true,
                IsDeleted = false,
                CreatedDate = DateTime.Now,
                CreatedBy = user.CreatedBy,
                HostIp = Request.HttpContext.Connection.RemoteIpAddress.ToString()
            };

            await _userRepository.AddAsync(user, cancellationToken, false);
            await _userProfilesRepository.AddAsync(userProfile, cancellationToken, false);
            await _userRoleReository.AddAsync(userRole, cancellationToken, false);

            await _userRepository.SaveChangesAsync(cancellationToken);
            await _userProfilesRepository.SaveChangesAsync(cancellationToken);
            await _userRoleReository.SaveChangesAsync(cancellationToken);

            return Ok();
        }
        #endregion End Add User

        #region Edit User
        public async Task<IActionResult> EditUser(Guid userid, CancellationToken cancellationToken)
        {
            try
            {
                var roles = _eFRepositoryRole.TableNoTracking.ToList();
                var model = new UserEditViewModel();
                roles.Insert(0, new Role
                {
                    Id = 0,
                    RoleNameFa = "..."
                });
                ViewBag.ListofRoles = roles;

                if (userid == null)
                {
                    return NotFound();
                }
                var user = await _userRepository.GetByGuidIdAsync(userid, cancellationToken);
                if (user == null)
                {
                    return NotFound();
                }
                var lastUserRole = user.UserRoles.LastOrDefault();

                var userEditViewModel = new UserEditViewModel
                {
                    UserId = user.Id,
                    FirstName = user.UserProfiles.Select(p => p.FirstName).FirstOrDefault(),
                    LastName = user.UserProfiles.Select(p => p.LastName).FirstOrDefault(),
                    NationalCode = user.UserProfiles.Select(p => p.NationalCode).FirstOrDefault(),
                    UserName = user.UserName,
                    Password = string.Empty,
                    EmailAddress = user.EmailAddress,
                    PhoneNumber = user.PhoneNumber,
                    RoleId = lastUserRole?.RoleId ?? 0, // اگر نقش نداشت، مقدار پیش‌فرض
                    IsActive = user.IsActive,
                };


                return PartialView("EditUser", userEditViewModel);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<ApiResult> EditUser([Bind("UserId,FirstName,LastName,NationalCode,UserName,Password,EmailAddress,PhoneNumber,RoleId,IsActive")] UserEditViewModel userEditViewModel, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(userEditViewModel.Password))
                {
                    ModelState.Remove(nameof(userEditViewModel.Password));
                }

                if (!ModelState.IsValid)
                {
                    return new BadRequestObjectResult(ModelState);
                }

                var user = await _userRepository.Table.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userEditViewModel.UserId, cancellationToken);
                var userProfile = await _userProfilesRepository.Table.AsNoTracking().FirstOrDefaultAsync(up => up.UserId == userEditViewModel.UserId, cancellationToken);
                var userRole = await _userRoleReository.Table.AsNoTracking().FirstOrDefaultAsync(ur => ur.UserId == userEditViewModel.UserId, cancellationToken);

                var curentUserToken = User.Identity.FindFirstValue("Token");
                var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(curentUserToken), cancellationToken);

                if (user.NormalizedUserName != userEditViewModel.UserName.FixTextUpper())
                {
                    var userExist = await _userRepository.Table.AsNoTracking().FirstOrDefaultAsync(u => u.NormalizedUserName == userEditViewModel.UserName.FixTextUpper(), cancellationToken);
                    if (userExist != null)
                    {
                        ModelState.AddModelError("UserName", "نام کاربری تکراری است");
                    }
                }
                if (user.NormalizedEmailAddress != userEditViewModel.EmailAddress.FixTextUpper())
                {
                    var userExist = await _userRepository.Table.AsNoTracking().FirstOrDefaultAsync(u => u.NormalizedEmailAddress == userEditViewModel.EmailAddress.FixTextUpper(), cancellationToken);
                    if (userExist != null)
                    {
                        ModelState.AddModelError("EmailAddress", "ایمیل تکراری است");
                    }
                }
                if (user.PhoneNumber != userEditViewModel.PhoneNumber)
                {
                    var userExist = await _userRepository.Table.AsNoTracking().FirstOrDefaultAsync(u => u.PhoneNumber == userEditViewModel.PhoneNumber, cancellationToken);
                    if (userExist != null)
                    {
                        ModelState.AddModelError("PhoneNumber", "شماره همراه تکراری است");
                    }
                }
                if (userProfile.NationalCode != userEditViewModel.NationalCode)
                {
                    var userExist = await _userProfilesRepository.Table.AsNoTracking().FirstOrDefaultAsync(up => up.NationalCode == userEditViewModel.NationalCode.FixTextUpper(), cancellationToken);
                    if (userExist != null)
                    {
                        ModelState.AddModelError("PhoneNumber", "کدملی تکراری است");
                    }
                }

                if (!ModelState.IsValid)
                {
                    var role = _eFRepositoryRole.TableNoTracking.ToList();
                    role.Insert(0, new Role
                    {
                        Id = 0,
                        RoleNameFa = "..."
                    });
                    ViewBag.ListofRoles = role;
                    return new BadRequestObjectResult(ModelState);
                }

                user.UserName = userEditViewModel.UserName;
                user.NormalizedUserName = userEditViewModel.UserName.FixTextUpper();
                user.EmailAddress = userEditViewModel.EmailAddress;
                user.NormalizedEmailAddress = userEditViewModel.EmailAddress.FixTextUpper();
                user.EmailAddressConfirmed = false;
                user.ConcurrencyStamp = Guid.NewGuid();
                user.PhoneNumber = userEditViewModel.PhoneNumber;
                user.PhoneNumberConfirmed = false;
                user.TwoFactorEnabled = false;
                user.LockoutEnd = null;
                user.LockoutEnabled = false;
                user.AccessFailedCount = 0;
                user.EditorUser = token.User;
                user.EditDate = DateTime.Now;
                user.IsActive = userEditViewModel.IsActive;
                user.HostIp = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                if (!string.IsNullOrEmpty(userEditViewModel.Password))
                {
                    user.PasswordHash = PasswordHelper.HashPasswordBCrypt(userEditViewModel.Password);
                }

                userProfile.FirstName = userEditViewModel.FirstName;
                userProfile.LastName = userEditViewModel.LastName;
                userProfile.NationalCode = userEditViewModel.NationalCode;
                userProfile.IsActive = userEditViewModel.IsActive;
                userProfile.HostIp = Request.HttpContext.Connection.RemoteIpAddress.ToString();

                userRole.RoleId = userEditViewModel.RoleId;

                _userRepository.Attach(user);
                _userProfilesRepository.Attach(userProfile);
                _userRoleReository.Attach(userRole);

                _userRepository.Update(user);
                _userProfilesRepository.Update(userProfile);
                _userRoleReository.Update(userRole);

                await _userRepository.SaveChangesAsync(cancellationToken);
                await _userProfilesRepository.SaveChangesAsync(cancellationToken);
                await _userRoleReository.SaveChangesAsync(cancellationToken);

                // تست ثبت لاگ کاربر
                //var oldUser = dbContext.Users.AsNoTracking().FirstOrDefault(u => u.Id == user.Id);

                //// Update user
                //dbContext.Users.Update(user);
                //dbContext.SaveChanges();

                //// Create AuditLog entry
                //var auditLog = new AuditLog
                //{
                //    AuditLogId = Guid.NewGuid(),
                //    EntityId = user.Id,
                //    TableName = "User",
                //    OperationType = "Update",
                //    OperationTime = DateTime.UtcNow,
                //    UserId = user.CreatorID,
                //    OldValue = JsonConvert.SerializeObject(oldUser),
                //    NewValue = JsonConvert.SerializeObject(user),
                //    Description = "User updated"
                //};
                //dbContext.AuditLogs.Add(auditLog);
                //dbContext.SaveChanges();


                return Ok();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }
        #endregion End Edit User

        [HttpPost]
        public async Task<IActionResult> DeleteUser(Guid userid, CancellationToken cancellationToken)
        {
            try
            {


                if (userid == null)
                {
                    return Json(new { isSuccess = false, message = "کاربری جهت حذف ارسال نشده است یا خطایی رخ داده." });
                }
                var curentUserToken = User.Identity.FindFirstValue("Token");
                var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(curentUserToken), cancellationToken);
                if (token != null)
                {
                    if (userid == token.UserId)
                    {
                        return Json(new { isSuccess = false, message = "امکان حذف کاربری جاری وجود ندارد." });
                    }
                }

                var user = await _userRepository.GetByIdAsync(cancellationToken, userid);
                if (user == null)
                {
                    return Json(new { isSuccess = false, message = "کاربری یافت نشد." });
                }
                user.IsDeleted = true;

                await _userRepository.UpdateAsync(user, cancellationToken);

                return Json(new { isSuccess = true, message = "کاربر با موفقیت حذف شد." });
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #region بررسی و صحت سنجی
        private async Task<UserCreateViewModel> CheckValidation(UserCreateViewModel userCreateViewModel, Guid CurentUserID, CancellationToken cancellationToken)
        {
            if (userCreateViewModel.RoleId == 0)
            {
                ModelState.AddModelError("RoleId", "نوع کاربری را انتخاب کنید");
            }
            var ExistEamil = await _userRepository.GetUserByEmailAsync(userCreateViewModel.EmailAddress.FixTextUpper(), cancellationToken);
            if (ExistEamil != null)
            {
                ModelState.AddModelError("EmailAddress", " Email تکراری است");
            }
            var ExistUserName = await _userRepository.GetUserByUserNameAsync(userCreateViewModel.UserName.FixTextUpper(), cancellationToken);
            if (ExistUserName != null)
            {
                ModelState.AddModelError("UserName", "نام کاربری تکراری است");
            }
            var ExistPhoneNumber = await _userRepository.GetUserByPhoneNumberAsync(userCreateViewModel.PhoneNumber.FixTextUpper(), cancellationToken);
            if (ExistPhoneNumber != null)
            {
                ModelState.AddModelError("PhoneNumber", " شماره همراه تکراری است");
            }
            var ExistNationalCode = await _userProfilesRepository.GetProfileByNationalCodeAsync(userCreateViewModel.NationalCode.FixTextUpper(), cancellationToken);
            if (ExistNationalCode != null)
            {
                ModelState.AddModelError("NationalCode", " کد ملی تکراری است");
            }
            return userCreateViewModel;
        }
        #endregion

        #region دریافت فهرست منو بر اساس سطح دسترسی کاربر و نمای سطح دسترسی کاربر انتخابی
        public IActionResult UserAccessMenu(Guid userid)
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            var memoryCache = HttpContext.RequestServices.GetService<IMemoryCache>();

            ViewData["userId"] = userid;
            ViewBag.Modify_SelectedNodes_SelectedUser = memoryCache.HasAccess(token, "Modify_SelectedNodes_SelectedUser");
            return PartialView("UserAccessMenu");
        }
        public async Task<ActionResult<string>> GetUserAccessMenuItem(Guid userid, CancellationToken cancellationToken)
        {
            #region Convert ListMenu to UserAccessMenusViewModel for use jstree
            List<Entities.Menu.Menu> listMenus = await _menuRepository.GetAllMenusAsync(cancellationToken);
            List<UserAccessMenusViewModel> userAccessMenusViewModel = new List<UserAccessMenusViewModel>();
            foreach (var item in listMenus)
            {
                if (item.ParentId == null)
                {
                    userAccessMenusViewModel.Add(new UserAccessMenusViewModel
                    {
                        id = item.Id,
                        parent = "#",
                        text = item.MenuTitle,
                    });
                }
                else
                {
                    userAccessMenusViewModel.Add(new UserAccessMenusViewModel
                    {
                        id = item.Id,
                        parent = item.ParentId.ToString(),
                        text = item.MenuTitle,
                    });
                }
            }
            #endregion

            #region Add User Access item
            List<View_UserMenubar> view_UserMenubars = _view_UserMenubarRepository.GetUserAccessMenus(userid, cancellationToken);
            foreach (var item in view_UserMenubars)
            {
                foreach (var item2 in userAccessMenusViewModel)
                {
                    if (item.Id.Equals(item2.id))
                    {
                        item2.Person_Checkecd = item.Person_Checkecd;
                        item2.Group_Checkecd = item.Group_Checkecd;
                    }
                }
            }
            #endregion

            #region Add options jstree
            foreach (var item in userAccessMenusViewModel)
            {
                if (item.Person_Checkecd == true)
                {
                    var result = true;
                    foreach (var item2 in userAccessMenusViewModel)
                    {
                        if (item2.parent == item.id.ToString())
                        {
                            result = false;
                            break;
                        }
                    }

                    if (result)
                    {
                        UserAccessMenusStateViewModel userAccessMenusStateViewModel = new UserAccessMenusStateViewModel();
                        userAccessMenusStateViewModel.selected = true;
                        userAccessMenusStateViewModel.opened = true;
                        userAccessMenusStateViewModel.disabled = item.Group_Checkecd;
                        item.state = userAccessMenusStateViewModel;
                        if (item.Group_Checkecd == true)
                        {
                            JStreeAttr jStreeAttr = new JStreeAttr()
                            {
                                href = "test1",
                                title = "TitleTest"
                            };
                            item.a_attr = jStreeAttr;
                        }
                    }
                    else
                    {
                        UserAccessMenusStateViewModel userAccessMenusStateViewModel = new UserAccessMenusStateViewModel();
                        userAccessMenusStateViewModel.selected = false;
                        userAccessMenusStateViewModel.opened = false;
                        userAccessMenusStateViewModel.disabled = item.Group_Checkecd;
                        item.state = userAccessMenusStateViewModel;
                        if (item.Group_Checkecd == true)
                        {
                            JStreeAttr jStreeAttr = new JStreeAttr()
                            {
                                href = "test1",
                                title = "TitleTest"
                            };
                            item.a_attr = jStreeAttr;
                        }
                    }
                }
            }
            #endregion

            var json = JsonConvert.SerializeObject(userAccessMenusViewModel);
            return json;
        }
        #endregion پایان

        #region افزودن و اصلاح سطح دسترسی کاربر انتخابی
        //public async Task<IActionResult> Modify_SelectedNodes_SelectedUser(string list, Guid userId, CancellationToken cancellationToken)
        public async Task<IActionResult> Modify_SelectedNodes_SelectedUser([FromBody] ModifyUserAcceesMenuViewModel request, CancellationToken cancellationToken)
        {
            List<UserAccessMenus> userAccessMenus = await _accessMenuRepository.GetUserAccessMenusByUserIdAsync(request.UserId, cancellationToken);
            //Remove all access
            if (request.List is null)
            {
                foreach (var item in userAccessMenus)
                {
                    item.IsDeleted = true;
                    await _accessMenuRepository.UpdateAsync(item, cancellationToken);
                }
            }
            else
            {
                #region Old Code
                Guid[] strArray = request.List.ToArray();
                foreach (var item in strArray)
                {
                    bool isExist = userAccessMenus.Any(ua => ua.MenuId.Equals(item));
                    if (!isExist)
                    {
                        UserAccessMenus NewAccess = new UserAccessMenus();
                        NewAccess.MenuId = item;
                        NewAccess.UserId = request.UserId;
                        NewAccess.CreatedBy = new Guid(User.Identity.FindFirstValue(ClaimTypes.NameIdentifier));
                        NewAccess.IsDeleted = false;
                        NewAccess.IsActive = true;
                        NewAccess.HostIp = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                        await _accessMenuRepository.AddAsync(NewAccess, cancellationToken);
                    }
                }
                foreach (var item in userAccessMenus)
                {
                    bool isExist = strArray.Any(ua => ua.Equals(item.MenuId));
                    if (!isExist)
                    {
                        item.IsDeleted = true;
                        await _accessMenuRepository.UpdateAsync(item, cancellationToken);
                    }
                    else if (isExist && item.IsDeleted == true)
                    {
                        item.IsDeleted = false;
                        await _accessMenuRepository.UpdateAsync(item, cancellationToken);
                    }
                }
                #endregion

                #region New Code
                // تبدیل لیست درخواست به آرایه از Guid
                //Guid[] menuIds = request.List.ToArray();

                //// تبدیل آرایه به HashSet برای جستجو سریعتر
                //HashSet<Guid> menuIdSet = new HashSet<Guid>(menuIds);

                //// پیدا کردن آیتم‌هایی که باید اضافه بشن (آنهایی که در userAccessMenus وجود ندارند)
                //var newMenus = menuIds.Where(menuId => !userAccessMenus.Any(ua => ua.MenuId.Equals(menuId)))
                //                      .Select(menuId => new UserAccessMenus
                //                      {
                //                          MenuId = menuId,
                //                          UserId = request.UserId,
                //                          CreatedBy = new Guid(User.Identity.FindFirstValue(ClaimTypes.NameIdentifier)),
                //                          IsDeleted = false,
                //                          HostIp = Request.HttpContext.Connection.RemoteIpAddress.ToString()
                //                      }).ToList();

                //// اضافه کردن آیتم‌های جدید به پایگاه داده
                //foreach (var newAccess in newMenus)
                //{
                //    await _accessMenuRepository.AddAsync(newAccess, cancellationToken);
                //}

                //// پیدا کردن آیتم‌هایی که باید حذف بشن (آنهایی که در menuIds نیستند)
                //var menusToDelete = userAccessMenus.Where(item => item.MenuId.HasValue && !menuIdSet.Contains(item.MenuId.Value)) // اصلاح برای استفاده از Value
                //                                   .ToList();

                //// پیدا کردن آیتم‌هایی که باید وضعیت آنها تغییر کند (از حذف به فعال)
                //var menusToActivate = userAccessMenus.Where(item => item.MenuId.HasValue && !menuIdSet.Contains(item.MenuId.Value) && item.IsDeleted)
                //                                     .ToList();


                //// حذف آیتم‌های قدیمی
                //foreach (var item in menusToDelete)
                //{
                //    item.IsDeleted = true;
                //    await _accessMenuRepository.UpdateAsync(item, cancellationToken);
                //}

                //// فعال‌سازی آیتم‌هایی که قبلاً حذف شده‌اند
                //foreach (var item in menusToActivate)
                //{
                //    item.IsDeleted = false;
                //    await _accessMenuRepository.UpdateAsync(item, cancellationToken);
                //}
                #endregion

            }
            return Json(new { isSuccess = true, message = "اصلاح دسترسی با موفقیت انجام شد." });
        }
        #endregion پایان


        [HttpPost]
        public async Task<IActionResult> UserFieldList(Guid userid, CancellationToken cancellationToken)
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> ActiveStation(Guid userid, CancellationToken cancellationToken)
        {
            try
            {
                var stations = await _stationRepository.GetAllStationsNoTrackingAsync(cancellationToken);
                var user = await _userRepository.GetByGuidIdAsync(userid, cancellationToken);
                var result = new StationViewModel
                {
                    Stations = stations,
                    ActiveStation = stations.Where(s => s.Id == user.ActiveStation).Select(s => s.Name).FirstOrDefault(),
                    UserId = user.Id
                };
                return PartialView("ActiveStation", result);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetActiveStation([FromBody] SetStationRequest model, CancellationToken cancellationToken)
        {
            if (model == null || model.StationId <= 0)
                return BadRequest(new { success = false, message = "ایستگاه نامعتبر است." });

            // منطق ذخیره سازی برای کاربر جاری
            // مثلاً ذخیره در پروفایل کاربر یا session
            var user = await _userRepository.GetByGuidIdTrackedAsync(model.UserId, cancellationToken);
            if (user == null) return NotFound();


            user.ActiveStation = model.StationId;



            await _userRepository.UpdateUserAsync(user, cancellationToken);


            return Ok(new { success = true, message = "ایستگاه با موفقیت ذخیره شد." });
        }

        #region CreateUser Excel

        [HttpGet]
        public IActionResult UploadUserExcel()
        {
            return PartialView("UploadUserExcel");
        }

        [HttpPost]
        public IActionResult UploadUserExcel(IFormFile file)
        {
            // 1. بررسی اعتبار فایل
            if (file == null || file.Length == 0)
                return BadRequest("فایل نامعتبر است");

            // 2. پردازش فایل اکسل
            using var stream = file.OpenReadStream();

            // 💡 نکته: در صورت استفاده از Aspose.Cells، اطمینان حاصل کنید که فایل آن را در مسیر پروژه یا
            // به عنوان یک پکیج Nuget به درستی اضافه کرده‌اید.
            var workbook = new Aspose.Cells.Workbook(stream);
            var sheet = workbook.Worksheets[0];

            // 3. استخراج سربرگ ستون‌ها
            var columns = new List<string>();
            // فرض می‌شود که سطر اول (Index 0) شامل سربرگ‌ها است
            for (int c = 0; c <= sheet.Cells.MaxDataColumn; c++)
            {
                var header = sheet.Cells[0, c].StringValue?.Trim();
                if (!string.IsNullOrEmpty(header))
                {
                    columns.Add(header);
                }
            }

            // 4. ذخیره فایل در Session و تولید کلید موقت
            var key = Guid.NewGuid().ToString();
            var sessionKey = $"IMPORT_USER_EXCEL_{key}";

            using var ms = new MemoryStream();
            workbook.Save(ms, Aspose.Cells.SaveFormat.Xlsx);
            // 💡 نکته: برای این خط، نیاز به using Microsoft.AspNetCore.Http دارید.
            HttpContext.Session.Set(sessionKey, ms.ToArray());

            // 5. ساخت ViewModel نهایی
            var model = new ImportUserExcelMappingViewModel
            {
                TempKey = sessionKey,
                ExcelColumns = columns,
                // 💡 نکته: فرض بر این است که _eFRepositoryRole یک کلاس Repository معتبر برای نقش‌ها است.
                Roles = _eFRepositoryRole.TableNoTracking
                             .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.RoleNameFa })
                             .ToList()
            };

            // 6. بازگشت محتوا به عنوان PartialView
            // این اقدام از بازگشت تگ‌های <html> و <body> جلوگیری کرده و خطای جاوا اسکریپت را رفع می‌کند.
            return PartialView("UploadUserExcelMapping", model);
        }
        [HttpPost]
        public IActionResult UploadUserExcelMapping()
        {
            return PartialView("UploadUserExcelMapping");
        }

        [HttpPost]
        public async Task<IActionResult> SaveUsersFromExcel(ImportUserExcelMappingViewModel model, CancellationToken cancellationToken)
        {
            if (!HttpContext.Session.TryGetValue(model.TempKey, out byte[] bytes) || bytes == null)
            {
                return BadRequest("فایل اکسل منقضی شده است.");
            }

            using var ms = new MemoryStream(bytes);
            var workbook = new Aspose.Cells.Workbook(ms);
            var sheet = workbook.Worksheets[0];

            int GetColIndex(string colName)
            {
                if (string.IsNullOrWhiteSpace(colName)) return -1;
                for (int i = 0; i <= sheet.Cells.MaxDataColumn; i++)
                {
                    if (sheet.Cells[0, i].StringValue?.Trim() == colName.Trim())
                        return i;
                }
                return -1;
            }

            var existEmails = (await _userRepository.GetAllEmailsAsync(cancellationToken)).Select(x => x.FixTextUpper()).ToHashSet();
            var existUserNames = (await _userRepository.GetAllUserNamesAsync(cancellationToken)).Select(x => x.FixTextUpper()).ToHashSet();
            var existPhones = (await _userRepository.GetAllPhonesAsync(cancellationToken)).Select(x => x.FixTextUpper()).ToHashSet();
            var existNationalCodes = (await _userProfilesRepository.GetAllNationalCodesAsync(cancellationToken)).Select(x => x.FixTextUpper()).ToHashSet();

            // 2. پیدا کردن ایندکس تمام ستون‌های مورد نیاز
            int firstNameCol = GetColIndex(model.FirstNameColumn);
            int lastNameCol = GetColIndex(model.LastNameColumn);
            int phoneNumberCol = GetColIndex(model.PhoneNumberColumn);
            int nationalCodeCol = GetColIndex(model.NationalCodeColumn);
            int emailAddressCol = GetColIndex(model.EmailAddressColumn);
            int userNameCol = GetColIndex(model.UserNameColumn);
            int passwordCol = GetColIndex(model.PasswordColumn);
            int isActiveCol = GetColIndex(model.IsActiveColumn);
            int fieldCodesCol = GetColIndex(model.FieldCodesColumn);

            // 3. اعتبارسنجی ستون‌های اجباری
            if (firstNameCol == -1 || lastNameCol == -1 || userNameCol == -1 || passwordCol == -1)
            {
                return BadRequest("لطفاً ستون‌های نام، نام خانوادگی، نام کاربری و رمز عبور را انتخاب کنید.");
            }

            // 4. دریافت اطلاعات کاربر ایجادکننده
            var curentUserToken = User.Identity.FindFirstValue("Token");
            var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(curentUserToken), cancellationToken);
            var createdByUserId = token.UserId;
            var creatorUser = token.User;
            var hostIp = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            var createdDate = DateTime.Now;

            // 5. متغیرهای شمارشی
            int successCount = 0;
            var failedRows = new List<string>();

            var examFields = await _examFieldRepository.GetAllFieldsByExamIdAsync(cancellationToken);

            // 6. حلقه برای پردازش سطر به سطر کاربران (شروع از سطر ۱)
            for (int r = 1; r <= sheet.Cells.MaxDataRow; r++)
            {
                // 6.1. استخراج داده‌ها (همانند قبل)
                var firstName = sheet.Cells[r, firstNameCol].StringValue?.Trim();
                var lastName = sheet.Cells[r, lastNameCol].StringValue?.Trim();
                var phoneNumber = phoneNumberCol != -1 ? sheet.Cells[r, phoneNumberCol].StringValue?.Trim() : null;
                var nationalCode = nationalCodeCol != -1 ? sheet.Cells[r, nationalCodeCol].StringValue?.Trim() : null;
                var emailAddress = emailAddressCol != -1 ? sheet.Cells[r, emailAddressCol].StringValue?.Trim() : null;
                var userName = userNameCol != -1 ? sheet.Cells[r, userNameCol].StringValue?.Trim() : null;
                var password = passwordCol != -1 ? sheet.Cells[r, passwordCol].StringValue?.Trim() : null;
                var isActiveString = isActiveCol != -1 ? sheet.Cells[r, isActiveCol].StringValue?.Trim() : null;
                var isActive = isActiveString?.ToLower() == "فعال" || isActiveString == "1" || bool.TryParse(isActiveString, out bool activeResult) && activeResult;
                string fieldCodesString = fieldCodesCol != -1 ? sheet.Cells[r, fieldCodesCol].StringValue?.Trim() : null;

                // 6.2. اعتبارسنجی ساده هر سطر
                if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                    string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                {
                    failedRows.Add($"ردیف {r}: اطلاعات نام، نام خانوادگی، نام کاربری یا رمز عبور کامل نیست.");
                    continue;
                }

                var validationResult = ValidateUserForImport(
                                        new UserCreateViewModel
                                        {
                                            EmailAddress = emailAddress,
                                            UserName = userName,
                                            PhoneNumber = phoneNumber,
                                            NationalCode = nationalCode,
                                            RoleId = model.RoleId
                                        },
                                        existEmails,
                                        existUserNames,
                                        existPhones,
                                        existNationalCodes
                                    );

                if (!validationResult.IsValid)
                {
                    failedRows.Add($"ردیف {r}: {string.Join("، ", validationResult.Errors)}");
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(nationalCode))
                {
                    nationalCode = nationalCode.Trim(); // حذف space و invisible chars

                    if (nationalCode.Length > 10)
                    {
                        failedRows.Add($"ردیف {r}: طول کد ملی بیشتر از 10 کاراکتر است ({nationalCode})");
                        continue;
                    }
                }


                // 6.3. شروع تراکنش برای این کاربر خاص
                using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    // 6.3.1. ایجاد Entities (همانند قبل)
                    var userId = Guid.NewGuid();

                    // Entity User
                    Entities.User.User user = new Entities.User.User()
                    {
                        Id = userId,
                        UserName = userName,
                        NormalizedUserName = userName.FixTextUpper(),
                        EmailAddress = emailAddress,
                        NormalizedEmailAddress = emailAddress?.FixTextUpper(),
                        EmailAddressConfirmed = false,
                        PasswordHash = PasswordHelper.HashPasswordBCrypt(password),
                        ConcurrencyStamp = Guid.NewGuid(),
                        PhoneNumber = phoneNumber,
                        PhoneNumberConfirmed = false,
                        TwoFactorEnabled = false,
                        LockoutEnd = null,
                        LockoutEnabled = false,
                        AccessFailedCount = 0,

                        CreatedDate = createdDate,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedBy = createdByUserId,
                        HostIp = hostIp
                    };

                    // Entity UserProfile
                    UserProfile userProfile = new UserProfile()
                    {
                        UserId = userId,
                        FirstName = firstName,
                        LastName = lastName,
                        NationalCode = nationalCode,

                        CreatedDate = createdDate,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedBy = createdByUserId,
                        HostIp = hostIp
                    };

                    // Entity UserRole
                    UserRole userRole = new UserRole()
                    {
                        UserId = userId,
                        RoleId = model.RoleId,

                        CreatedDate = createdDate,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedBy = createdByUserId,
                        HostIp = hostIp
                    };

                    // 6.3.2. افزودن به Repository (بدون ذخیره نهایی)
                    await _userRepository.AddAsync(user, cancellationToken, false);
                    await _userProfilesRepository.AddAsync(userProfile, cancellationToken, false);
                    await _userRoleReository.AddAsync(userRole, cancellationToken, false);


                    // 6.3.3. تخصیص رشته‌ها (UserField)
                    if (!string.IsNullOrWhiteSpace(fieldCodesString))
                    {
                        var codes = fieldCodesString
                            .Split(new[] { '-', ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(x =>
                            {
                                //bool ok = int.TryParse(x.Trim(), out int val);
                                var cleaned = new string(x.Where(char.IsDigit).ToArray());
                                bool ok = int.TryParse(cleaned, out int val);
                                return new { ok, val };
                            })
                            .Where(x => x.ok)
                            .Select(x => x.val)
                            .Distinct()
                            .ToList();


                        // 🔑 تبدیل Code → Id واقعی از DB
                        var examFieldMap = examFields.Where(x => codes.Contains(x.StandardCode)).ToDictionary(x => x.StandardCode, x => x.Id);
                        var fieldsToSave = new List<UserField>();

                        foreach (var code in codes)
                        {
                            if (!examFieldMap.TryGetValue(code, out var examFieldId))
                            {
                                // ❗ کد استاندارد در دیتابیس وجود ندارد
                                // این رشته را رد می‌کنیم (یا لاگ می‌گیریم)
                                continue;
                            }

                            fieldsToSave.Add(new UserField
                            {
                                UserID = userId,
                                ExamFieldID = examFieldId, // ✅ Id واقعی

                                CreatedDate = createdDate,
                                IsActive = true,
                                IsDeleted = false,
                                CreatedBy = createdByUserId,
                                HostIp = hostIp
                            });
                        }

                        if (fieldsToSave.Any())
                        {
                            await _userFieldRepository.AddRangeAsync(fieldsToSave, cancellationToken, false);
                        }
                    }


                    // 6.3.4. ذخیره کردن (یک بار برای تمام تغییرات) و Commit کردن
                    await _userRepository.SaveChangesAsync(cancellationToken);
                    await _userProfilesRepository.SaveChangesAsync(cancellationToken);
                    await _userRoleReository.SaveChangesAsync(cancellationToken);
                    await _userFieldRepository.SaveChangesAsync(cancellationToken);
                    //await _dbContext.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    if (!string.IsNullOrWhiteSpace(user.EmailAddress))
                        existEmails.Add(user.EmailAddress.FixTextUpper());

                    if (!string.IsNullOrWhiteSpace(user.UserName))
                        existUserNames.Add(user.UserName.FixTextUpper());

                    if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
                        existPhones.Add(user.PhoneNumber.FixTextUpper());

                    if (!string.IsNullOrWhiteSpace(userProfile.NationalCode))
                        existNationalCodes.Add(userProfile.NationalCode.FixTextUpper());

                    successCount++;
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                {
                    // 6.4. در صورت بروز خطا: Rollback و ثبت خطا
                    await transaction.RollbackAsync(cancellationToken);

                    string errorDetail = "خطای دیتابیس (تکراری بودن نام کاربری/کد ملی/ایمیل)";
                    if (ex.InnerException?.Message != null && ex.InnerException.Message.ToLower().Contains("duplicate key"))
                    {
                        errorDetail = $"تکراری بودن کلید یکتا (نام کاربری/کد ملی/ایمیل)";
                    }

                    failedRows.Add($"ردیف {r} ({firstName} {lastName}): {errorDetail}");

                    // 💡 در اینجا می‌توانید جزئیات خطا را لاگ کنید.
                }
                catch (Exception ex)
                {
                    // 6.4. خطاهای دیگر
                    await transaction.RollbackAsync(cancellationToken);
                    failedRows.Add($"ردیف {r} ({firstName} {lastName}): خطای نامشخص ({ex.Message})");
                }
            }

            // 7. حذف فایل از Session
            HttpContext.Session.Remove(model.TempKey);

            // 8. بازگشت نتیجه نهایی
            if (failedRows.Any())
            {
                string finalMessage = $"تعداد {successCount} کاربر با موفقیت ثبت شدند. اما ثبت {failedRows.Count} کاربر با خطا مواجه شد:<br>" +
                                      string.Join("<br>", failedRows.Take(10)); // نمایش ۱۰ خطای اول

                // اگر هیچ کاربر موفقی نداشتیم، BadRequest می‌دهیم.
                if (successCount == 0)
                {
                    return BadRequest(new { isSuccess = false, message = "هیچ کاربری با موفقیت ثبت نشد. جزئیات خطا در پیام موجود است." });
                }

                // اگر برخی موفق و برخی ناموفق بودند، یک پیام موفقیت‌آمیز با اخطار برمی‌گردانیم.
                return Ok(new { isSuccess = true, message = finalMessage });
            }

            return Ok(new { isSuccess = true, message = $"تعداد {successCount} کاربر با موفقیت ثبت شدند." });
        }

        // 💡 نکته مهم: اگر از Dapper یا متدهای EF Core SaveChangesAsync استفاده می‌کنید،
        // استفاده از یک Transaction برای تضمین اتمیک بودن عملیات (همه یا هیچ) بسیار توصیه می‌شود.
        #endregion
        private ImportValidationResult ValidateUserForImport(
                                                        UserCreateViewModel model,
                                                        HashSet<string> existEmails,
                                                        HashSet<string> existUserNames,
                                                        HashSet<string> existPhones,
                                                        HashSet<string> existNationalCodes)
        {
            var result = new ImportValidationResult();

            if (model.RoleId == 0)
                result.Errors.Add("نوع کاربری مشخص نشده");

            if (!string.IsNullOrWhiteSpace(model.EmailAddress) &&
                existEmails.Contains(model.EmailAddress.FixTextUpper()))
                result.Errors.Add("ایمیل تکراری است");

            if (existUserNames.Contains(model.UserName.FixTextUpper()))
                result.Errors.Add("نام کاربری تکراری است");

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber) &&
                existPhones.Contains(model.PhoneNumber.FixTextUpper()))
                result.Errors.Add("شماره همراه تکراری است");

            if (!string.IsNullOrWhiteSpace(model.NationalCode) &&
                existNationalCodes.Contains(model.NationalCode.FixTextUpper()))
                result.Errors.Add("کد ملی تکراری است");

            return result;
        }

        public class ImportValidationResult
        {
            public List<string> Errors { get; set; } = new();

            public bool IsValid => !Errors.Any();
        }
    }
}