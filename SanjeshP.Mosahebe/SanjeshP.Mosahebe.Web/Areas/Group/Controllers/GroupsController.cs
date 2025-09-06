using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SanjeshP.Mosahebe.Web.Areas.Group.ViewModels.Groups;
using SanjeshP.Mosahebe.Web.Areas.User.Controllers;
using SanjeshP.Mosahebe.Web.Areas.User.ViewModels.Menu;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Data.Contracts.Groups;
using SanjeshP.Mosahebe.Data.Contracts.Menu;
using SanjeshP.Mosahebe.Data.Contracts.Menus;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using SanjeshP.Mosahebe.Entities.Group;
using SanjeshP.Mosahebe.Entities.Menu;
using SanjeshP.Mosahebe.Entities.User;
using SanjeshP.Mosahebe.WebFramework.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Web.Areas.Group.Controllers
{
    [Area("Group")]
    public class GroupsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly ILogger<UsersController> _logger;
        private readonly IGroupRepository _groupsRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly IEntityFrameworkRepository<GroupUsers> _eFRepositoryUserGroup;
        private readonly IMenusRepository _menuRepository;
        private readonly IAccessMenusRepository _accessMenuRepository;
        private readonly IViewUserMenubarRepository _view_UserMenubarRepository;
        private readonly IAccessMenusGroupRepository _accessMenusGroupRepository;

        public GroupsController(IMapper mapper
                                , ICompositeViewEngine viewEngine
                                , ILogger<UsersController> logger
                                , IGroupRepository groupsRepository
                                , IUserRepository userRepository
                                , IUserTokenRepository userTokenRepository
                                , IEntityFrameworkRepository<GroupUsers> eFRepositoryUserGroup
                                , IMenusRepository menuRepository
                                , IAccessMenusRepository accessMenuRepository
                                , IViewUserMenubarRepository view_UserMenubarRepository
                                , IAccessMenusGroupRepository accessMenusGroupRepository
                                )
        {
            _mapper = mapper;
            _viewEngine = viewEngine;
            _logger = logger;
            _groupsRepository = groupsRepository;
            _userRepository = userRepository;
            _userTokenRepository = userTokenRepository;
            _eFRepositoryUserGroup = eFRepositoryUserGroup;
            _menuRepository = menuRepository;
            _accessMenuRepository = accessMenuRepository;
            _view_UserMenubarRepository = view_UserMenubarRepository;
            _accessMenusGroupRepository = accessMenusGroupRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GroupsList()
        {

            var groups = _groupsRepository.GetAllGroups();
            var groupSelectDtos = _mapper.Map<IEnumerable<GroupViewModel>>(groups);
            return PartialView("GroupsList", groupSelectDtos);
        }

        [HttpGet]
        public IActionResult CreateGroup()
        {

            return PartialView("CreateGroup");
        }

        [HttpPost]
        public async Task<ApiResult> CreateGroup([Bind("GroupName,IsActive")] GroupCreateViewModel groupCreateViewModel, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(groupCreateViewModel.GroupName))
            {
                ModelState.AddModelError("GroupName", "لطفا نامگروه را وارد کنید.");
                return new BadRequestObjectResult(ModelState);
            }
            var existingGroup = await _groupsRepository.Table.AsNoTracking().FirstOrDefaultAsync(up => up.GroupName == groupCreateViewModel.GroupName.Trim(), cancellationToken);
            if (existingGroup != null)
            {
                ModelState.AddModelError("GroupName", "نام گروه تکراری است.");
                return new BadRequestObjectResult(ModelState);
            }
            var curentUserToken = User.Identity.FindFirstValue("Token");
            UserToken token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(curentUserToken), HttpContext.RequestAborted);

            Entities.Group.Group group = new Entities.Group.Group()
            {
                Id = Guid.NewGuid(),
                GroupName = groupCreateViewModel.GroupName.Trim(),
                CreatedBy = token.UserId,
                HostIp = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                IsActive = groupCreateViewModel.IsActive,
                IsDeleted = false,
                CreatedDate = DateTime.Now,
            };
            _groupsRepository.Add(group);

            return Ok();
        }

        public IActionResult EditGroup(Guid groupId)
        {
            var group = _groupsRepository.GetById(groupId);
            var groupDto = _mapper.Map<GroupEditViewModel>(group);
            return PartialView("EditGroup", groupDto);
        }

        [HttpPost]
        public async Task<ApiResult> EditGroup([Bind("Id,GroupName,IsActive")] GroupEditViewModel groupEditViewModel, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(groupEditViewModel.GroupName))
            {
                ModelState.AddModelError("GroupName", "لطفا نام گروه را وارد کنید.");
                return new BadRequestObjectResult(ModelState);
            }
            var group = await _groupsRepository.Table.AsNoTracking().FirstOrDefaultAsync(up => up.Id == groupEditViewModel.Id, cancellationToken);

            if (group.GroupName != groupEditViewModel.GroupName.Trim())
            {
                var existingGroup = await _groupsRepository.Table.AsNoTracking().FirstOrDefaultAsync(up => up.GroupName == groupEditViewModel.GroupName.Trim(), cancellationToken);
                if (existingGroup != null)
                {
                    ModelState.AddModelError("GroupName", "نام گروه تکراری است.");
                    return new BadRequestObjectResult(ModelState);
                }
            }
            var curentUserToken = User.Identity.FindFirstValue("Token");
            UserToken token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(curentUserToken), cancellationToken);

            group.GroupName = groupEditViewModel.GroupName.Trim();
            group.CreatedBy = token.UserId;
            group.HostIp = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            group.IsActive = groupEditViewModel.IsActive;
            group.CreatedDate = DateTime.Now;
            _groupsRepository.Update(group);

            return Ok();
        }

        [HttpPost]
        public async Task<ApiResult> DeleteGroup(Guid groupId, CancellationToken cancellationToken)
        {
            var group = await _groupsRepository.Table.AsNoTracking().FirstOrDefaultAsync(up => up.Id == groupId, cancellationToken);
            if (group == null)
            {
                ModelState.AddModelError("GroupError", "گروه یافت نشد.");
                return new BadRequestObjectResult(ModelState);
            }
            var hasUsersInGroup = await _eFRepositoryUserGroup.Table.AsNoTracking().AnyAsync(up => up.GroupId == groupId, cancellationToken);
            if (hasUsersInGroup)
            {
                ModelState.AddModelError("UserGroupError", "در این گروه کاربر وجود دارد و امکان حذف وجود ندارد.");
                return new BadRequestObjectResult(ModelState);
            }

            group.IsDeleted = true;
            await _groupsRepository.UpdateAsync(group, cancellationToken);

            return Ok();
        }

        #region دریافت فهرست منو بر اساس سطح دسترسی کاربر و نمای سطح دسترسی گروه انتخابی
        public IActionResult GroupAccessMenu(Guid groupId)
        {
            ViewData["groupId"] = groupId;
            return PartialView("GroupAccessMenu");
        }
        public async Task<ActionResult<string>> GetGroupAccessMenuItem(Guid groupid, CancellationToken cancellationToken)
        {
            var curentUserToken = User.Identity.FindFirstValue("Token");
            UserToken token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(curentUserToken), cancellationToken);

            #region Convert ListMenu to UserAccessMenusViewModel for use jstree
            List<Entities.Menu.Menu> listMenus = await _menuRepository.GetAllMenusAsync(cancellationToken);
            List<UserAccessMenusViewModel> listMenuUserAccessDtos = new List<UserAccessMenusViewModel>();
            foreach (var item in listMenus)
            {
                if (item.ParentId == null)
                {
                    listMenuUserAccessDtos.Add(new UserAccessMenusViewModel
                    {
                        id = item.Id,
                        parent = "#",
                        text = item.MenuTitle,
                    });
                }
                else
                {
                    listMenuUserAccessDtos.Add(new UserAccessMenusViewModel
                    {
                        id = item.Id,
                        parent = item.ParentId.ToString(),
                        text = item.MenuTitle,
                    });
                }
            }
            #endregion

            #region Add Group Access item
            List<View_UserMenubar> view_GrpoupMenubars = await _view_UserMenubarRepository.GetGroupSelectedAccessMenusByGroupIdAsync(token.UserId, groupid, cancellationToken);
            foreach (var item in view_GrpoupMenubars)
            {
                foreach (var item2 in listMenuUserAccessDtos)
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
            foreach (var item in listMenuUserAccessDtos)
            {
                if (item.Group_Checkecd == true)
                {
                    var result = true;
                    foreach (var item2 in listMenuUserAccessDtos)
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
                        //listMenuUserAccessStateDto.disabled = item.Group_Checkecd;
                        item.state = userAccessMenusStateViewModel;
                    }
                    else
                    {
                        UserAccessMenusStateViewModel userAccessMenusStateViewModel = new UserAccessMenusStateViewModel();
                        userAccessMenusStateViewModel.selected = false;
                        userAccessMenusStateViewModel.opened = false;
                        //listMenuUserAccessStateDto.disabled = item.Group_Checkecd;
                        item.state = userAccessMenusStateViewModel;
                    }
                }
            }
            #endregion

            var json = JsonConvert.SerializeObject(listMenuUserAccessDtos);
            return json;
        }
        #endregion پایان

        #region افزودن و اصلاح سطح دسترسی گروه انتخابی
        //public async Task<ApiResult> Modify_SelectedNodes_SelectedGroup(string list, Guid groupId, CancellationToken cancellationToken)
        public async Task<ApiResult> Modify_SelectedNodes_SelectedGroup([FromBody] ModifyGroupAccessMenusViewModel request, CancellationToken cancellationToken)
        {
            List<GroupAccessMenus> userAccessMenusGroup = await _accessMenusGroupRepository.GetGroupAccessMenusByGroupIdAsync(request.GroupId, cancellationToken);
            //Remove all access
            if (request.List is null)
            {
                foreach (var item in userAccessMenusGroup)
                {
                    item.IsDeleted = true;
                    await _accessMenusGroupRepository.UpdateAsync(item, cancellationToken);
                }
            }
            else
            {

                Guid[] strArray = request.List.ToArray();
                foreach (var item in strArray)
                {
                    bool isExist = userAccessMenusGroup.Any(ua => ua.MenuId.Equals(item));
                    if (!isExist)
                    {
                        GroupAccessMenus NewAccess = new GroupAccessMenus();
                        NewAccess.MenuId = item;
                        NewAccess.GroupId = request.GroupId;
                        NewAccess.CreatedBy = new Guid(User.Identity.FindFirstValue(ClaimTypes.NameIdentifier));
                        NewAccess.IsDeleted = false;
                        NewAccess.IsActive = true;
                        NewAccess.HostIp = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                        await _accessMenusGroupRepository.AddAsync(NewAccess, cancellationToken);
                    }
                }
                foreach (var item in userAccessMenusGroup)
                {
                    bool isExist = strArray.Any(ua => ua.Equals(item.MenuId));
                    if (!isExist)
                    {
                        item.IsDeleted = true;
                        item.IsActive = false;
                        await _accessMenusGroupRepository.UpdateAsync(item, cancellationToken);
                    }
                    else if (isExist && item.IsDeleted == true)
                    {
                        item.IsDeleted = false;
                        item.IsActive = true;
                        await _accessMenusGroupRepository.UpdateAsync(item, cancellationToken);
                    }
                }
            }
            return Ok();
        }
        #endregion پایان
    }
}
