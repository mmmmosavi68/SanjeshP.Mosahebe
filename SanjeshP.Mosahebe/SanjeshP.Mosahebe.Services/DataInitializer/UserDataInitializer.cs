using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Entities.Group;
using SanjeshP.Mosahebe.Entities.Menu;
using SanjeshP.Mosahebe.Entities.SiteSetting;
using SanjeshP.Mosahebe.Entities.User;
using Services.RDC.DataInitializer;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Services.DataInitializer
{
    public class UserDataInitializer : IDataInitializer
    {
        private readonly IEntityFrameworkRepository<Role> eFRoleRepository;
        private readonly IEntityFrameworkRepository<User> eFUserRepository;
        private readonly IEntityFrameworkRepository<UserProfile> eFUserProfileRepository;
        private readonly IEntityFrameworkRepository<Menu> eFMenuRepository;
        private readonly IEntityFrameworkRepository<Group> eFGroupRepository;
        private readonly IEntityFrameworkRepository<GroupUsers> eFUserGroupRepository;
        private readonly IEntityFrameworkRepository<GroupAccessMenus> eFAccessMenusGroupRepository;
        private readonly IEntityFrameworkRepository<UserAccessMenus> eFAccessMenusRepository;
        private readonly IEntityFrameworkRepository<UserRole> eFUserRoleRepository;
        private readonly IEntityFrameworkRepository<LoginSettings> eFLoginSettings;

        public UserDataInitializer(IEntityFrameworkRepository<Role> eFRoleRepository
            , IEntityFrameworkRepository<User> eFUserRepository
            , IEntityFrameworkRepository<UserProfile> eFUserProfileRepository
            , IEntityFrameworkRepository<Menu> eFMenuRepository
            , IEntityFrameworkRepository<Group> eFGroupRepository
            , IEntityFrameworkRepository<GroupUsers> eFUserGroupRepository
            , IEntityFrameworkRepository<GroupAccessMenus> eFAccessMenusGroupRepository
            , IEntityFrameworkRepository<UserAccessMenus> eFAccessMenusRepository
            , IEntityFrameworkRepository<UserRole> eFUserRoleRepository
            , IEntityFrameworkRepository<LoginSettings> eFLoginSettings)
        {
            this.eFRoleRepository = eFRoleRepository;
            this.eFUserRepository = eFUserRepository;
            this.eFUserProfileRepository = eFUserProfileRepository;
            this.eFMenuRepository = eFMenuRepository;
            this.eFGroupRepository = eFGroupRepository;
            this.eFUserGroupRepository = eFUserGroupRepository;
            this.eFAccessMenusGroupRepository = eFAccessMenusGroupRepository;
            this.eFAccessMenusRepository = eFAccessMenusRepository;
            this.eFUserRoleRepository = eFUserRoleRepository;
            this.eFLoginSettings = eFLoginSettings;
        }

        public void InitializeData()
        {
            string path = Path.Combine(Environment.CurrentDirectory, @"DefaultData\");

            var jsonUser = File.ReadAllText(path + "User.json");
            var jUser = JsonConvert.DeserializeObject<List<User>>(jsonUser);
            foreach (var menu in jUser)
            {
                var existMenu = eFUserRepository.TableNoTracking.Any(p => p.NormalizedUserName == menu.NormalizedUserName);
                if (!existMenu)
                {
                    menu.CreatedDate = DateTime.Now;
                    menu.ExpireDate = DateTime.Now.AddYears(20);
                    eFUserRepository.Add(menu);
                }
            }

            var user = eFUserRepository.TableNoTracking.Where(p => p.NormalizedUserName == "ADMINISTRATOR").SingleOrDefault();

            var jsonUserProfile = File.ReadAllText(path + "UserProfile.json");
            var jUserProfile = JsonConvert.DeserializeObject<List<UserProfile>>(jsonUserProfile);
            foreach (var menu in jUserProfile)
            {
                var existMenu = eFUserProfileRepository.TableNoTracking.Any(p => p.UserId == menu.UserId);
                if (!existMenu)
                {
                    menu.CreatedBy = user.Id;
                    menu.CreatedDate = DateTime.Now;
                    eFUserProfileRepository.Add(menu);
                }
            }

            var jsonMenu = File.ReadAllText(path + "Menu.json");
            var jMenu = JsonConvert.DeserializeObject<List<Menu>>(jsonMenu).OrderBy(u => u.PageCode);
            foreach (var menu in jMenu)
            {
                var existMenu = eFMenuRepository.TableNoTracking.Any(p => p.Id == menu.Id);
                if (!existMenu)
                {
                    menu.CreatedBy = user.Id;
                    menu.CreatedDate = DateTime.Now;
                    eFMenuRepository.Add(menu);
                }
            }

            var jsonGroup = File.ReadAllText(path + "Group.json");
            var jGroup = JsonConvert.DeserializeObject<List<Group>>(jsonGroup);
            foreach (var menu in jGroup)
            {
                var existMenu = eFGroupRepository.TableNoTracking.Any(p => p.GroupName == menu.GroupName);
                if (!existMenu)
                {
                    menu.CreatedBy = user.Id;
                    menu.CreatedDate = DateTime.Now;
                    eFGroupRepository.Add(menu);
                }
            }

            var jsonUserGroup = File.ReadAllText(path + "UserGroup.json");
            var jUserGroup = JsonConvert.DeserializeObject<List<GroupUsers>>(jsonUserGroup);
            foreach (var menu in jUserGroup)
            {
                var existMenu = eFUserGroupRepository.TableNoTracking.Any(p => p.Id == menu.Id);
                if (!existMenu)
                {
                    menu.CreatedBy = user.Id;
                    menu.CreatedDate = DateTime.Now;
                    eFUserGroupRepository.Add(menu);
                }
            }

            //var jsonAccessMenusGroup = File.ReadAllText(path + "AccessMenusGroup.json");
            //var jAccessMenusGroup = JsonConvert.DeserializeObject<List<GroupAccessMenus>>(jsonAccessMenusGroup);
            //foreach (var menu in jAccessMenusGroup)
            //{
            //    var existMenu = eFAccessMenusGroupRepository.TableNoTracking.Any(p => p.GroupId == menu.GroupId && p.MenuId == menu.MenuId);
            //    if (!existMenu)
            //    {
            //        menu.CreatedBy = user.Id;
            //        menu.CreatedDate = DateTime.Now;
            //        eFAccessMenusGroupRepository.Add(menu);
            //    }
            //}

            //var jsonAccessMenu = File.ReadAllText(path + "AccessMenus.json");
            //var jAccessMenu = JsonConvert.DeserializeObject<List<UserAccessMenus>>(jsonAccessMenu);
            //foreach (var menu in jAccessMenu)
            //{
            //    var existMenu = eFAccessMenusRepository.TableNoTracking.Any(p => p.UserId == menu.UserId && p.MenuId == menu.MenuId);
            //    if (!existMenu)
            //    {
            //        menu.CreatedBy = user.Id;
            //        menu.CreatedDate = DateTime.Now;
            //        eFAccessMenusRepository.Add(menu);
            //    }
            //}

            var jsonRole = File.ReadAllText(path + "Role.json");
            var jRole = JsonConvert.DeserializeObject<List<Role>>(jsonRole);
            foreach (var menu in jRole)
            {
                var existMenu = eFRoleRepository.TableNoTracking.Any(p => p.RoleNameEn == menu.RoleNameEn && p.RoleNameFa == menu.RoleNameFa);
                if (!existMenu)
                {
                    menu.CreatedBy = user.Id;
                    menu.CreatedDate = DateTime.Now;
                    eFRoleRepository.Add(menu);
                }
            }

            var jsonLoginSetting = File.ReadAllText(path + "LoginSetting.json");
            var jLoginSetting = JsonConvert.DeserializeObject<List<LoginSettings>>(jsonLoginSetting);
            foreach (var loginSetting in jLoginSetting)
            {
                var existLloginSetting = eFLoginSettings.TableNoTracking.Any(p => p.Id == loginSetting.Id);
                if (!existLloginSetting)
                {
                    loginSetting.CreatedBy = user.Id;
                    loginSetting.CreatedDate = DateTime.Now;
                    eFLoginSettings.Add(loginSetting);
                }
            }

            var role = eFRoleRepository.TableNoTracking.Where(p => p.NormalizedRoleNameEn == "ADMIN").SingleOrDefault();
            var existuserRole = eFUserRoleRepository.TableNoTracking.Any(p => p.UserId == user.Id && p.RoleId == role.Id);
            if (!existuserRole)
            {
                UserRole userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedDate = DateTime.Now,
                    CreatedBy = user.Id,
                    HostIp = "::1"
                };
                eFUserRoleRepository.Add(userRole);
            }
        }
    }
}