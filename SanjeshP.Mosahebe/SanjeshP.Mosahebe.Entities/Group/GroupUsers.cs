using SanjeshP.Mosahebe.Entities.Common;
using SanjeshP.Mosahebe.Entities.Menu;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SanjeshP.Mosahebe.Entities.Group
{
    public class GroupUsers : BaseEntity
    {
        [Required(ErrorMessage = "شناسه کاربر الزامی است")]
        [Display(Name = "شناسه کاربر")]
        [ForeignKey("User")]
        public Guid? UserId { get; set; }

        [Required(ErrorMessage = "شناسه گروه الزامی است")]
        [Display(Name = "شناسه گروه")]
        [ForeignKey("Group")]
        public Guid? GroupId { get; set; }

        [Display(Name = "کاربر")]
        public virtual User.User User { get; set; }

        [Display(Name = "گروه")]
        public virtual Group Group { get; set; }

        public virtual ICollection<GroupAccessMenus> AccessMenusGroups { get; set; } = new List<GroupAccessMenus>();
    }
}
