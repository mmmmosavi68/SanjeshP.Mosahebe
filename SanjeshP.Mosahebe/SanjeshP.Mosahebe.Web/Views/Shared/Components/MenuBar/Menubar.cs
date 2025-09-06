using Microsoft.AspNetCore.Mvc;
using SanjeshP.Mosahebe.Web.Areas.User.ViewModels.Menu;
using SanjeshP.Mosahebe.Data.Contracts.Menus;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Web.Views.Shared.Components.MenuBar
{
    [ViewComponent(Name = "MenuBar")]
    public class MenuBarViewComponent : ViewComponent
    {
        private readonly IUserTokenRepository _userTokenRepository;

        public readonly IViewUserMenubarRepository _view_UserMenubarRepository;

        public MenuBarViewComponent(IViewUserMenubarRepository view_UserMenubarRepository, IUserTokenRepository userTokenRepository)
        {
            _view_UserMenubarRepository = view_UserMenubarRepository;
            _userTokenRepository = userTokenRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var token = new Guid(HttpContext.User.FindFirst("Token").Value.ToString());
            var userToken = await _userTokenRepository.GetUserTokenByIdAsync(token, HttpContext.RequestAborted);
            var CurentView_UserMenubar = _view_UserMenubarRepository.GetUserAccessMenus(userToken.UserId, HttpContext.RequestAborted);
            if (CurentView_UserMenubar == null)
            {
                // Handle the case where _view_UserMenubar is null
                return View(new List<MenuTreeViewModel>());
            }

            List<MenuTreeViewModel> FinalList = new List<MenuTreeViewModel>();
            foreach (var item in CurentView_UserMenubar)
            {
                if (item.ParentId == null)
                {
                    FinalList.Add(new MenuTreeViewModel
                    {
                        CssClass = item.CssClass,
                        Icon = item.Icon,
                        Id = item.Id,
                        Link = item.Link,
                        PageCode = item.PageCode,
                        ParentId = item.ParentId,
                        ShowMenu = item.ShowMenu,
                        Title = item.MenuTitle,
                        IsDelete = item.Person_Checkecd,
                        Area = item.Area,
                        ActionName = item.ActionName,
                        ControllerName = item.ControllerName,
                        children = GetChildMenu(item.Id, CurentView_UserMenubar)
                    });
                }
            }

            return View(FinalList);
        }
        private List<MenuTreeViewModel> GetChildMenu(Guid _id, List<Entities.Menu.View_UserMenubar> _List)
        {
            List<MenuTreeViewModel> FinalList = new List<MenuTreeViewModel>();
            foreach (var item in _List)
            {
                if (item.ParentId == _id && (item.Person_Checkecd == true || item.Group_Checkecd == true))
                {
                    FinalList.Add(new MenuTreeViewModel
                    {
                        CssClass = item.CssClass,
                        Icon = item.Icon,
                        Id = item.Id,
                        Link = item.Link,
                        PageCode = item.PageCode,
                        ParentId = item.ParentId,
                        ShowMenu = item.ShowMenu,
                        Title = item.MenuTitle,
                        Area = item.Area,
                        ActionName = item.ActionName,
                        ControllerName = item.ControllerName,
                        children = GetChildMenu(item.Id, _List)
                    });
                }
            }
            return FinalList;
        }
    }
}
