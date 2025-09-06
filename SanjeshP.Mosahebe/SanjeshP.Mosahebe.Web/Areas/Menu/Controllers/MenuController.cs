using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SanjeshP.Mosahebe.Web.Areas.Menu.ViewModels.Menu;
using SanjeshP.Mosahebe.Web.Areas.User.ViewModels.Menu;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Common.Utilities;
using SanjeshP.Mosahebe.Data.Contracts.Menus;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using SanjeshP.Mosahebe.Entities.Menu;
using SanjeshP.Mosahebe.WebFramework.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Web.Areas.Menu.Controllers
{
    [Area("Menu")]
    public class MenuController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly ILogger<MenuController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IMenusRepository _menuRepository;
        private readonly IViewUserMenubarRepository _view_UserMenubarRepository;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly IAccessMenusRepository _accessMenuRepository;
        private readonly IMemoryCache _memoryCache;

        public MenuController(IMapper mapper
                                , ICompositeViewEngine viewEngine
                                , ILogger<MenuController> logger
                                , IUserRepository userRepository
                                , IMenusRepository menuRepository
                                , IViewUserMenubarRepository view_UserMenubarRepository
                                , IUserTokenRepository userTokenRepository
                                , IAccessMenusRepository accessMenuRepository
                                , IMemoryCache memoryCache)
        {
            _mapper = mapper;
            _viewEngine = viewEngine;
            _userRepository = userRepository;
            _logger = logger;
            _userTokenRepository = userTokenRepository;
            _menuRepository = menuRepository;
            _view_UserMenubarRepository = view_UserMenubarRepository;
            _userTokenRepository = userTokenRepository;
            _accessMenuRepository = accessMenuRepository;
            _memoryCache = memoryCache;
        }
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult CurrentUserMenu()
        {
            return PartialView("CurrentUserMenu");
        }

        #region دریافت فهرست منو بر اساس سطح دسترسی کاربر و نمای سطح دسترسی کاربر انتخابی
        public async Task<ActionResult<string>> GetCurrentUserMenus(CancellationToken cancellationToken)
        {
            var curentUserToken = User.Identity.FindFirstValue("Token");
            var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(curentUserToken), cancellationToken);
            var user = await _userRepository.GetUserByIdAsync(token.UserId, cancellationToken);
            if (user == null)
            {
                return NotFound();
            }
            var userid = token.UserId;

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
                        text = item.PageCode + " _ " + item.MenuTitle,
                    });
                }
                else
                {
                    userAccessMenusViewModel.Add(new UserAccessMenusViewModel
                    {
                        id = item.Id,
                        parent = item.ParentId.ToString(),
                        text = item.PageCode + " _ " + item.MenuTitle,
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
            return (json);
        }
        #endregion پایان

        #region Create Menu
        public async Task<IActionResult> CreateMenu(Guid parentId, CancellationToken cancellationToken)
        {
            var menu = await _menuRepository.GetByIdAsync(cancellationToken, parentId);
            if (menu == null)
            {
                return NotFound();
            }
            // ارسال اطلاعات به view
            ViewBag.ParentId = parentId;
            ViewBag.ParentTitle = menu.MenuTitle;


            var model = new MenuCreateViewModel
            {
                ParentId = parentId,
                CssClass = "nav-item",
                Icon = "fa fa-home",
                PageCode = menu.PageCode,
                Link = "No Link",

                ShowMenu = false // مقدار پیش‌فرض برای ShowMenu
            };
            return PartialView("CreateMenu", model);
        }

        [HttpPost]
        public async Task<ApiResult> CreateMenu(MenuCreateViewModel menuCreateViewModel, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            try
            {
                var currentUserToken = User.FindFirstValue("Token");
                var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(currentUserToken), cancellationToken);

                var menu = new Entities.Menu.Menu
                {
                    Id = Guid.NewGuid(),
                    ParentId = menuCreateViewModel.ParentId,
                    MenuTitle = menuCreateViewModel.MenuTitle,
                    ShowMenu = menuCreateViewModel.ShowMenu,
                    CssClass = menuCreateViewModel.CssClass,
                    Icon = menuCreateViewModel.Icon,
                    IsDeleted = false,
                    Link = menuCreateViewModel.Link,
                    PageCode = menuCreateViewModel.PageCode,
                    Path = menuCreateViewModel.Path,
                    ActionName = menuCreateViewModel.ActionName,
                    Area = menuCreateViewModel.Area,
                    ControllerName = menuCreateViewModel.ControllerName,

                    CreatedBy = token.UserId,
                    HostIp = Request.HttpContext.Connection.RemoteIpAddress?.ToString(),
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                await _menuRepository.AddAsync(menu, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ایجاد آزمون");
                return (ApiResult)StatusCode(500, "خطا در سرور");
            }
        }
        #endregion End Create Menu


        #region Edit Menu
        [HttpGet]
        public async Task<IActionResult> EditMenu(Guid id, CancellationToken cancellationToken)
        {
            // دریافت اطلاعات منو برای ویرایش
            var menu = await _menuRepository.GetByIdAsync(cancellationToken, id);
            if (menu == null)
            {
                return NotFound();
            }


            var model = new MenuEditViewModel
            {
                Id = menu.Id,
                MenuTitle = menu.MenuTitle,
                Parent_title = menu.MenuTitle, /* menu.ParentId == null ? "ندارد" : (await _menuRepository.GetByIdAsync(cancellationToken, menu.ParentId.Value)).MenuTitle,*/
                PageCode = menu.PageCode,
                Path = menu.Path,
                Area = menu.Area,
                ControllerName = menu.ControllerName,
                ActionName = menu.ActionName,
                ShowMenu = menu.ShowMenu ?? false,
                CssClass = menu.CssClass,
                Icon = menu.Icon,
                Link = menu.Link
            };
            return PartialView("EditMenu", model);
        }

        [HttpPost]
        public async Task<ApiResult> EditMenu(MenuEditViewModel menuEditViewModel, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            try
            {
                var currentUserToken = User.FindFirstValue("Token");
                var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(currentUserToken), cancellationToken);

                // بروزرسانی اطلاعات منو
                var menu = await _menuRepository.GetByIdAsync(cancellationToken, menuEditViewModel.Id);
                if (menu == null)
                {
                    return (ApiResult)NotFound();
                }

                menu.MenuTitle = menuEditViewModel.MenuTitle;
                menu.ParentId = menu.ParentId;
                menu.PageCode = menuEditViewModel.PageCode;
                menu.Path = menuEditViewModel.Path;
                menu.Area = menuEditViewModel.Area;
                menu.ControllerName = menuEditViewModel.ControllerName;
                menu.ActionName = menuEditViewModel.ActionName;
                menu.ShowMenu = menuEditViewModel.ShowMenu;
                menu.CssClass = menuEditViewModel.CssClass;
                menu.Icon = menuEditViewModel.Icon;
                menu.Link = menuEditViewModel.Link;

                //menu.ModifiedBy = token.UserId;
                menu.UpdatedDate = DateTime.Now;
                menu.HostIp = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
                menu.IsActive = true;

                await _menuRepository.UpdateAsync(menu, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ویرایش منو");
                return (ApiResult)StatusCode(500, "خطا در سرور");
            }
        }
        #endregion End Edit Menu


        #region Delete Menu
        [HttpGet]
        public async Task<IActionResult> DeleteMenu(Guid id, CancellationToken cancellationToken)
        {
            // دریافت اطلاعات منو برای ویرایش
            var menu = await _menuRepository.GetByIdAsync(cancellationToken, id);
            if (menu == null)
            {
                return NotFound();
            }


            var model = new DeleteMenuViewModel
            {
                Id = menu.Id,
                MenuTitle = menu.MenuTitle,
            };
            return PartialView("DeleteMenu", model);
        }

        [HttpPost]
        public async Task<ApiResult> DeleteMenu(DeleteMenuViewModel deleteMenuViewModel, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            try
            {
                var currentUserToken = User.FindFirstValue("Token");
                var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(currentUserToken), cancellationToken);

                // بروزرسانی اطلاعات منو
                var menu = await _menuRepository.GetByIdAsync(cancellationToken, deleteMenuViewModel.Id);
                if (menu == null)
                {
                    return (ApiResult)NotFound();
                }

                // نرمال‌سازی عناوین قبل از مقایسه
                var normalizedMenuTitle = menu.MenuTitle.FixPersianChars();
                var normalizedInputTitle = deleteMenuViewModel.MenuTitle.FixPersianChars();
                if (normalizedMenuTitle != normalizedInputTitle)
                {
                    return new BadRequestObjectResult("عنوان وارد شده صحیح نمی‌باشد.");
                }

                var allMenus = await _menuRepository.GetAllMenusAsync(cancellationToken);
                var hasChildren = allMenus.Any(menu => menu.ParentId == deleteMenuViewModel.Id);

                if (hasChildren)
                {
                    return new BadRequestObjectResult("این منو دارای زیرمجموعه است و نمی‌توان آن را حذف کرد.");
                }


                menu.IsActive = false;
                menu.IsDeleted = true;
                menu.UpdatedDate = DateTime.Now;
                menu.HostIp = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
                await _menuRepository.UpdateAsync(menu, cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ویرایش منو");
                return (ApiResult)StatusCode(500, "خطا در سرور");
            }
        }
        #endregion End Delete Menu

        public async Task<ApiResult> ResetMenu(CancellationToken cancellationToken)
        {
            try
            {
                string _jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "DefaultData", "Menu.json");

                // پاک کردن محتویات فایل JSON
                if (System.IO.File.Exists(_jsonFilePath))
                {
                    System.IO.File.Delete(_jsonFilePath); // حذف فایل
                }
                // دریافت داده‌ها از پایگاه داده
                List<Entities.Menu.Menu> menuData = await _menuRepository.GetAllMenusAsync(cancellationToken);
                // تبدیل داده‌های منو به ویو مدل
                var menuViewModels = menuData.Select(menu => new MenuAllFieldViewModel
                {
                    Id = menu.Id,
                    MenuTitle = menu.MenuTitle,
                    ParentId = menu.ParentId,
                    PageCode = menu.PageCode,
                    Path = menu.Path,
                    Area = menu.Area,
                    ControllerName = menu.ControllerName,
                    ActionName = menu.ActionName,
                    ShowMenu = menu.ShowMenu,
                    CssClass = menu.CssClass,
                    Icon = menu.Icon,
                    Link = menu.Link,
                    IsActive = menu.IsActive,
                    IsDeleted = menu.IsDeleted,
                    CreatedDate = menu.CreatedDate,
                    UpdatedDate = menu.UpdatedDate,
                    CreatedBy = menu.CreatedBy,
                    HostIp = menu.HostIp,
                }).ToList();
                // تبدیل داده‌ها به JSON
                var json = JsonConvert.SerializeObject(menuViewModels, Formatting.Indented);

                // نوشتن داده‌های جدید در فایل
                System.IO.File.WriteAllText(_jsonFilePath, json);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ویرایش منو");
                return (ApiResult)StatusCode(500, "خطا در سرور");
            }
        }

        public IActionResult DownloadJsonFile()
        {
            string _jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "DefaultData", "files", "Menu.json");

            // چک کردن اگر فایل وجود دارد
            if (System.IO.File.Exists(_jsonFilePath))
            {
                // خواندن فایل به صورت byte[] و ارسال به کلاینت
                var fileBytes = System.IO.File.ReadAllBytes(_jsonFilePath);
                return File(fileBytes, "application/json", "Menu.json"); // ارسال فایل با نوع Mime مناسب
            }
            else
            {
                return NotFound("فایل مورد نظر یافت نشد.");
            }
        }
    }
}
