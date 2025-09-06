using AutoMapper;
using SanjeshP.Mosahebe.Entities.Menu;
using SanjeshP.Mosahebe.WebFramework.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SanjeshP.Mosahebe.Web.Areas.Group.ViewModels.GroupUsers
{
    public class GroupUserViewModel : BaseDto<GroupUserViewModel, Entities.Group.GroupUsers, Guid>
    {
        public Guid GroupUserId { get; set; }
        [Display(Name = "نام")]
        public string FirstName { get; set; }
        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; }
        [Display(Name = "کد ملی")]
        public string NationalCode { get; set; }
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }
        [Display(Name = "نام گروه")]
        public string GroupName { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? UserId { get; set; }
        public virtual ICollection<GroupAccessMenus> MenusGroupAccesses { get; set; } = new List<GroupAccessMenus>();
        public virtual Entities.User.User User { get; set; }
        public virtual Entities.Group.Group Group { get; set; }

        public override void CustomMappings(IMappingExpression<Entities.Group.GroupUsers, GroupUserViewModel> mappingExpression)
        {
            mappingExpression.ForMember(
               dest => dest.FirstName,
               config => config.MapFrom(src => $"{User.UserProfiles.Select(U => U.FirstName).FirstOrDefault()}")
               );
            mappingExpression.ForMember(
               dest => dest.LastName,
               config => config.MapFrom(src => $"{User.UserProfiles.Select(U => U.LastName).FirstOrDefault()}")
               );
            mappingExpression.ForMember(
                dest => dest.UserName,
                config => config.MapFrom(src => $"{User.UserName}")
                );

            mappingExpression.ForMember(
               dest => dest.NationalCode,
               config => config.MapFrom(src => $"{User.UserProfiles.Select(U => U.NationalCode).FirstOrDefault()}")
               );

            mappingExpression.ForMember(
              dest => dest.GroupName,
              config => config.MapFrom(src => $"{src.Group.GroupName}")
              );
        }
    }
}
