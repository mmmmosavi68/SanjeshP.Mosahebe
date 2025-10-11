using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Threading;
using System;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SanjeshP.Mosahebe.Data.Contracts.Students;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using SanjeshP.Mosahebe.Web.Areas.StudentForm.Controllers;
using System.Linq;
using SanjeshP.Mosahebe.Web.Areas.DocumentVerification.ViewModels;
using SanjeshP.Mosahebe.Data.Contracts.StudentForm;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using SanjeshP.Mosahebe.Data;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using SanjeshP.Mosahebe.Entities.Student;
using SanjeshP.Mosahebe.Data.Repositories.StudentForm;
using SanjeshP.Mosahebe.Web.Areas.Student.ViewModels;
using SanjeshP.Mosahebe.Entities.StudentForm;

namespace SanjeshP.Mosahebe.Web.Areas.DocumentVerification.Controllers
{
    [AllowAnonymous]
    [Area("DocumentVerification")]
    public class EducationDocumentController : Controller
    {

        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<EducationDocumentController> _logger;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ICoefficientRepository _coefficientRepository;
        private readonly IStudentFormSubmissionScoreRepository _studentFormSubmissionRepository;
        private readonly IFormItemCoefficientDefinitionRepository _formItemCoefficientDefinitionRepository;
        private readonly IStudentFormSubmissionDataRepository _studentFormSubmissionValueRepository;

        public EducationDocumentController(IMapper mapper
                                        , IWebHostEnvironment env
                                        , ILogger<EducationDocumentController> logger
                                        , IUserTokenRepository userTokenRepository
                                        , IStudentRepository studentRepository
                                        , ICoefficientRepository coefficientRepository
                                        , IStudentFormSubmissionScoreRepository studentFormSubmissionRepository
                                        , IFormItemCoefficientDefinitionRepository formItemCoefficientDefinitionRepository
                                        , IStudentFormSubmissionDataRepository studentFormSubmissionValueRepository)
        {
            _mapper = mapper;
            _env = env;
            _logger = logger;
            _userTokenRepository = userTokenRepository;
            _studentRepository = studentRepository;
            this._coefficientRepository = coefficientRepository;
            _studentFormSubmissionRepository = studentFormSubmissionRepository;
            _formItemCoefficientDefinitionRepository = formItemCoefficientDefinitionRepository;
            _studentFormSubmissionValueRepository = studentFormSubmissionValueRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> StudentsList(CancellationToken cancellationToken)
        {
            try
            {
                var students = await _studentRepository.GetAllStudentsAsync(cancellationToken);

                var result = students.Select(e => new StudentListViewModel
                {
                    Id = e.Id,
                    RegId = e.RegId,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    FatherName = e.FatherName,
                    NationalCode = e.NationalCode,
                    CodeRahgiri = e.CodeRahgiri,
                    PhoneNumber = e.PhoneNumber,
                    Email = e.Email,

                    // تبدیل جنسیت عددی به متن
                    Gender = e.Gender == 1 ? "مرد" : "زن",

                    // تبدیل IsPajoheshi به بولین یا متن
                    IsPajoheshi = e.IsPajoheshi == 1,

                    // نام آزمون از navigation property
                    ExamName = "نام آزمون",//e.Exam?.Name ?? "",

                    // نوع دوره آزمون به متن
                    ExamCourseType = e.ExamCourseType == 1 ? "آموزشی" : "پژوهشی",

                    // رشته‌ها به صورت رشته کاما جدا شده
                    Fields = "رشته ها" //e.StudentFields != null && e.StudentFields.Any()? string.Join(", ", e.StudentFields.Select(f => f.Field.Name)): ""
                }).ToList();


                return PartialView("StudentsList", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت لیست آزمون‌ها");
                return StatusCode(500, "خطا در سرور");
            }
        }

        //[HttpGet]
        //public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        //{
        //    var forms = await _studentFormSubmissionRepository.GetAllByStudentIdAsync(id, cancellationToken);
        //    return PartialView("Details", forms);

        //}

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            if (id <= 0) return BadRequest();
            try
            {
                // گرفتن تمام فرم‌های داوطلب
                var forms = await _studentFormSubmissionRepository.GetAllByStudentIdAsync(id, cancellationToken);
                if (forms == null || !forms.Any())
                {
                    ViewBag.Message = "هیچ فرمی برای این داوطلب ثبت نشده است.";
                    forms = new List<StudentFormSubmissionScore>(); // لیست خالی برای جلوگیری از خطا در foreach
                }

                // 🧩 فیلتر رکوردهای معتبر و جایگزینی مقادیر Coefficient
                foreach (var form in forms)
                {
                    form.StudentFormSubmissionDatas = form.StudentFormSubmissionDatas?
                        .Where(d => d.IsActive && !d.IsDeleted)
                        .ToList() ?? new();

                    // گرفتن آیدی‌هایی که در Value ذخیره شده‌اند
                    var valueIds = form.StudentFormSubmissionDatas
                        .Select(d => int.TryParse(d.Value, out var idValue) ? idValue : (int?)null)
                        .Where(idValue => idValue.HasValue)
                        .Select(idValue => idValue.Value)
                        .Distinct()
                        .ToList();

                    // 🧮 اگر آیدی داریم، آن‌ها را از repository دریافت و جایگزین می‌کنیم
                    if (valueIds.Any())
                    {
                        foreach (var data in form.StudentFormSubmissionDatas)
                        {
                            if (data.InputType == InputType.Dropdown)
                            {
                                if (int.TryParse(data.Value, out var coefId))
                                {
                                    var coef = await _coefficientRepository.GetByIdAsync(cancellationToken, coefId);

                                    if (coef != null)
                                    {
                                        // جایگزینی مقدار با نام ضریب
                                        data.Value = coef.Name;
                                    }
                                }
                            }
                        }
                    }

                    // مرتب‌سازی
                    form.StudentFormSubmissionDatas = form.StudentFormSubmissionDatas
                        .OrderBy(d => d.CreatedDate)
                        .ToList();
                }

                // 🧭 مپ کردن به ViewModel
                var model = _mapper.Map<IEnumerable<SubmissionDataDetailViewModel>>(forms);

                return PartialView("Details", forms);
            }
            catch (Exception ex)
            {
                // لاگ دقیق‌تر می‌تونه اضافه بشه در آینده
                return StatusCode(500, "خطا در سرور");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveScores([FromForm] List<FormDataInputViewModel> Forms, CancellationToken cancellationToken)
        {
            // بررسی اینکه فرم‌ها خالی نباشند
            if (Forms == null || !Forms.Any())
            {
                ModelState.AddModelError(string.Empty, "داده‌ای برای ذخیره ارسال نشده است");
                return BadRequest(ModelState);
            }

            // بررسی اینکه کاربر لاگین کرده باشد
            if (!User.Identity.IsAuthenticated) return Forbid();

            try
            {
                // پیمایش بر روی تمام فرم‌های ارسال شده
                foreach (var formInput in Forms)
                {
                    if (cancellationToken.IsCancellationRequested) break; // توقف در صورت کنسل شدن عملیات
                    if (formInput == null) continue;

                    // دریافت رکورد اصلی فرم از دیتابیس
                    var submissionScore = await _studentFormSubmissionRepository
                        .GetDetailsForEditAsync(formInput.FormSubmissionScoreId, cancellationToken);

                    if (submissionScore == null)
                    {
                        _logger.LogWarning("submissionScore not found id={Id}", formInput.FormSubmissionScoreId);
                        continue;
                    }

                    var oneMaxScore = submissionScore.FormItem?.OneMaxScore ?? 0; // حداکثر امتیاز فرم

                    // اگر فرم تایید شده باشد
                    if (formInput.IsApproved == true)
                    {
                        submissionScore.IsApproved = true; // علامت تایید
                        submissionScore.RejectReason = null; // هیچ دلیل ردی وجود ندارد

                        // ذخیره نمره وارد شده
                        if (int.TryParse(formInput.ScoreValue, out var score))
                        {
                            // بررسی حداقل و حداکثر
                            if (score < 0) score = 0;
                            if (oneMaxScore > 0 && score > oneMaxScore) score = oneMaxScore;

                            submissionScore.Score = score;
                        }
                        else
                        {
                            submissionScore.Score = 0; // اگر نمره نامعتبر باشد، صفر در نظر گرفته می‌شود
                        }
                    }
                    else // اگر فرم تایید نشده باشد
                    {
                        submissionScore.IsApproved = false;
                        submissionScore.Score = 0; // نمره صفر برای فرم رد شده
                        submissionScore.RejectReason = formInput.RejectReason?.Trim(); // ذخیره دلیل رد
                    }

                    submissionScore.UpdatedDate = DateTime.UtcNow; // تاریخ بروزرسانی
                    await _studentFormSubmissionRepository.UpdateAsync(submissionScore, cancellationToken);
                }

                // ذخیره تغییرات در دیتابیس
                await _studentFormSubmissionRepository.SaveChangesAsync(cancellationToken);

                // بازگشت به صفحه اصلی یا لیست فرم‌ها
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ذخیره نمرات فرم‌ها");
                return StatusCode(500, "خطا در سرور");
            }
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DownloadSubmissionFile(int submissionDataId, int studentId, CancellationToken cancellationToken)
        {
            // دریافت داده‌ی دقیق submission-data (نام فایل در data.Value)
            var data = await _studentFormSubmissionValueRepository.GetSubmissionDataByIdAsync(submissionDataId, cancellationToken);
            if (data == null) return NotFound();

            // بررسی دسترسی به فایل (اجباری)
            var student = await _studentRepository.GetStudentByIdAsync(studentId, cancellationToken);
            // مثال: اگر بخواهید مطمئن شوید فایل متعلق به کاربر است:
            // var parent = data.StudentFormSubmissionScore; // اگر Include کرده‌اید
            // if (parent == null || parent.StudentId != currentUserStudentId) return Forbid();

            // امن‌سازی مقدار Value: اغلب data.Value فقط نام فایل است، نه مسیر کامل
            var fileName = Path.GetFileName(data.Value ?? string.Empty);
            if (string.IsNullOrEmpty(fileName)) return NotFound();

            // مسیر فیزیکی امن داخل wwwroot/uploads/{national}/{fileName}
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", student.NationalCode ?? string.Empty);
            var filePath = Path.Combine(uploadsFolder, fileName);

            // جلوگیری از مسیرهای فراری (path traversal)
            var normalized = Path.GetFullPath(filePath);
            if (!normalized.StartsWith(Path.GetFullPath(uploadsFolder), StringComparison.OrdinalIgnoreCase))
                return Forbid();

            if (!System.IO.File.Exists(normalized)) return NotFound();

            // تعیین mime type ایمن
            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(normalized, out var contentType))
                contentType = "application/octet-stream";

            var stream = System.IO.File.OpenRead(normalized);
            return File(stream, contentType, fileName);
        }
    }
}
