using SanjeshP.Mosahebe.Entities.Student;
using SanjeshP.Mosahebe.WebFramework.Api;
using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.StudentForm.ViewModels
{
    public class StudentCreateViewModel : BaseDto<StudentEditViewModel, Entities.Student.Student, int>
    {
        [Display(Name = "شماره ثبت‌نام")]
        [Required(ErrorMessage = "شماره ثبت‌نام الزامی است")]
        public int RegId { get; set; }

        [Display(Name = "نام")]
        [Required(ErrorMessage = "نام الزامی است")]
        [MaxLength(200)]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "نام خانوادگی الزامی است")]
        [MaxLength(200)]
        public string LastName { get; set; }

        [Display(Name = "نام پدر")]
        [MaxLength(200)]
        public string FatherName { get; set; }

        [Display(Name = "کد ملی")]
        [MaxLength(10)]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "کد ملی باید 10 رقم باشد")]
        public string NationalCode { get; set; }

        [Display(Name = "کد رهگیری")]
        [MaxLength(20)]
        public string CodeRahgiri { get; set; }

        [Display(Name = "شماره تماس")]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [Display(Name = "ایمیل")]
        [EmailAddress(ErrorMessage = "ایمیل معتبر نیست")]
        [MaxLength(250)]
        public string Email { get; set; }

        [Display(Name = "جنسیت")]
        [Required(ErrorMessage = "انتخاب جنسیت الزامی است")]
        public int Gender { get; set; } // 1 = مرد, 2 = زن

        [Display(Name = "نوع دوره (پژوهشی)")]
        public bool IsPajoheshi { get; set; }

        [Display(Name = "آزمون مرتبط")]
        [Required(ErrorMessage = "انتخاب آزمون الزامی است")]
        public int ExamId { get; set; }

        [Display(Name = "نوع دوره آزمون")]
        [Required(ErrorMessage = "نوع دوره آزمون الزامی است")]
        public int ExamCourseType { get; set; } // مثلاً 1 = آموزشی، 2 = پژوهشی

        [Display(Name = "وضعیت فرآیند مصاحبه")]
        public InterviewProcessStatus InterviewProcessStatus { get; set; } = InterviewProcessStatus.NotStarted;
    }
}
