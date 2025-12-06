using SanjeshP.Mosahebe.Entities.Common;
using SanjeshP.Mosahebe.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SanjeshP.Mosahebe.Entities.Student
{
    public class StudentToken : BaseEntity
    {
        [Required(ErrorMessage = "شناسه کاربر الزامی است")]
        [Display(Name = "شناسه کاربر")]
        [ForeignKey("User")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "شناسه جلسه الزامی است")]
        [MaxLength(128, ErrorMessage = "شناسه جلسه نباید بیش از 128 کاراکتر باشد")]
        [Display(Name = "شناسه جلسه")]
        public string SessionId { get; set; }

        [MaxLength(256, ErrorMessage = "Agent کاربر نباید بیش از 256 کاراکتر باشد")]
        [Display(Name = "Agent کاربر")]
        public string UserAgent { get; set; }

        [Required(ErrorMessage = "تاریخ انقضاء الزامی است")]
        [Display(Name = "تاریخ انقضاء")]
        public DateTime ExpirationDate { get; set; }

        [Display(Name = "کاربر")]
        public virtual Student Student { get; set; }

        public virtual ICollection<StudentOtp> StudentOtps { get; set; } = new List<StudentOtp>();


    }
}
