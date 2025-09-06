using SanjeshP.Mosahebe.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SanjeshP.Mosahebe.Entities.User
{
    public class Role : BaseEntity<int>
    {
        [Required(ErrorMessage = "عنوان نقش به انگلیسی الزامی است")]
        [MaxLength(50, ErrorMessage = "عنوان نقش به انگلیسی نباید بیش از 50 کاراکتر باشد")]
        [Display(Name = "عنوان نقش (انگلیسی)")]
        public string RoleNameEn { get; set; }

        [MaxLength(50, ErrorMessage = "عنوان نقش نرمال به انگلیسی نباید بیش از 50 کاراکتر باشد")]
        [Display(Name = "عنوان نقش نرمال (انگلیسی)")]
        public string NormalizedRoleNameEn { get; set; }

        [Required(ErrorMessage = "عنوان نقش به فارسی الزامی است")]
        [MaxLength(50, ErrorMessage = "عنوان نقش به فارسی نباید بیش از 50 کاراکتر باشد")]
        [Display(Name = "عنوان نقش (فارسی)")]
        public string RoleNameFa { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
