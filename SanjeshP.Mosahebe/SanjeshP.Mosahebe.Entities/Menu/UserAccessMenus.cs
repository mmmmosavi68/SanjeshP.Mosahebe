using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SanjeshP.Mosahebe.Entities.Menu
{
    public class UserAccessMenus : BaseEntity<int>
    {
        [Required(ErrorMessage = "شناسه کاربر الزامی است")]
        [Display(Name = "شناسه کاربر")]
        [ForeignKey("User")]
        public Guid? UserId { get; set; }

        [Required(ErrorMessage = "شناسه منو الزامی است")]
        [Display(Name = "شناسه منو")]
        [ForeignKey("Menu")]
        public Guid? MenuId { get; set; }

        [Display(Name = "کاربر")]
        public virtual User.User User { get; set; }

        [Display(Name = "منو")]
        public virtual Menu Menu { get; set; }
    }
}
