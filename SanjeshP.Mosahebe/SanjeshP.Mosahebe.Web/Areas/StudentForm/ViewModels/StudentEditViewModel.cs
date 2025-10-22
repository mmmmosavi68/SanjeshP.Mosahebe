using SanjeshP.Mosahebe.Entities.Student;
using SanjeshP.Mosahebe.WebFramework.Api;
using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.StudentForm.ViewModels
{
    public class StudentEditViewModel : BaseDto<StudentEditViewModel, Entities.Student.Student, int>
    {

        [Display(Name = "شماره ثبت‌نام")]
        public int RegId { get; set; }

        [Display(Name = "نام")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; }

        [Display(Name = "نام پدر")]
        public string FatherName { get; set; }

        [Display(Name = "کد ملی")]
        public string NationalCode { get; set; }

        [Display(Name = "کد رهگیری")]
        public string CodeRahgiri { get; set; }

        [Display(Name = "شماره تماس")]
        public string PhoneNumber { get; set; }

        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Display(Name = "جنسیت")]
        public int Gender { get; set; } // 1 = مرد, 2 = زن

        [Display(Name = "نوع دوره (پژوهشی)")]
        public bool IsPajoheshi { get; set; }

        [Display(Name = "آزمون مرتبط")]
        public int ExamId { get; set; }

        [Display(Name = "نوع دوره آزمون")]
        public int ExamCourseType { get; set; }

        [Display(Name = "وضعیت فرآیند مصاحبه")]
        [Required(ErrorMessage = "وضعیت مصاحبه را انتخاب کنید")]
        public InterviewProcessStatus InterviewProcessStatus { get; set; }
    }
}
