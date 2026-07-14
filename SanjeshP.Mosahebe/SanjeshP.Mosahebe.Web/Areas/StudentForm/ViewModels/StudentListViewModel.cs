using SanjeshP.Mosahebe.Entities.Student;
using SanjeshP.Mosahebe.WebFramework.Api;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.StudentForm.ViewModels
{
    public enum InterviewProcessStatusView
    {
        [Display(Name = "هنوز شروع نشده")]
        NotStarted = 0,

        [Display(Name = "فعال شده برای مصاحبه")]
        Active = 1,

        [Display(Name = "مصاحبه تمام شده")]
        Completed = 2
    }
    enum Gender { Male = 10, Female = 11 }
    public class StudentListViewModel : BaseDto<StudentListViewModel, Entities.Student.Student, int>
    {

        [Display(Name = "کد ثبت‌نام")]
        public int RegId { get; set; }

        [Display(Name = "نام")]
        [StringLength(50, ErrorMessage = "نام نمی‌تواند بیشتر از 50 کاراکتر باشد.")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [StringLength(50, ErrorMessage = "نام خانوادگی نمی‌تواند بیشتر از 50 کاراکتر باشد.")]
        public string LastName { get; set; }

        [Display(Name = "نام پدر")]
        [StringLength(50, ErrorMessage = "نام پدر نمی‌تواند بیشتر از 50 کاراکتر باشد.")]
        public string FatherName { get; set; }

        [Display(Name = "کد ملی")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "کد ملی باید 10 رقم باشد.")]
        public string NationalCode { get; set; }

        [Display(Name = "کد رهگیری")]
        [StringLength(20)]
        public string CodeRahgiri { get; set; }

        [Display(Name = "شماره تماس")]
        [Phone(ErrorMessage = "شماره تماس نامعتبر است.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "ایمیل")]
        [EmailAddress(ErrorMessage = "ایمیل نامعتبر است.")]
        public string Email { get; set; }

        [Display(Name = "جنسیت")]
        public string Gender { get; set; }  // می‌توان از Enum هم استفاده کرد و به متن نمایش تبدیل شود

        [Display(Name = "پژوهشی")]
        public bool IsPajoheshi { get; set; }
        [Display(Name = "وضعیت فرآیند مصاحبه")]
        public InterviewProcessStatusView InterviewProcessStatus { get; set; }

        [Display(Name = "نام آزمون")]
        public string ExamName { get; set; }

        //[Display(Name = "نوع دوره آزمون")]
        //public string ExamCourseType { get; set; }

        [Display(Name = "رشته‌ها")]
        public string Fields { get; set; }  // رشته‌ها به صورت کاما جداشده نمایش داده شوند

        public string ProfileImageUrl { get; set; }

        [Display(Name = "تعداد ایستگاه رشته")]
        public int CountStationField { get; set; }

        [Display(Name = "ایستگاه گذرانده شده")]
        public int PassedStation { get; set; }

    }
}
