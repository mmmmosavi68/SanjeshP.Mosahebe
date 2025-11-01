using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SanjeshP.Mosahebe.Data.Contracts.Exam;
using SanjeshP.Mosahebe.Data.Contracts.StudentForm;
using SanjeshP.Mosahebe.Data.Contracts.Students;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using SanjeshP.Mosahebe.Web.Areas.DocumentVerification.ViewModels;
using SanjeshP.Mosahebe.Web.Areas.Student.ViewModels;
using SanjeshP.Mosahebe.Entities.StudentForm;
using System.IO;
using System.Security.Claims;
using SanjeshP.Mosahebe.Data.Repositories.StudentForm;
using SanjeshP.Mosahebe.Common;

namespace SanjeshP.Mosahebe.Web.Areas.DocumentVerification.Controllers
{
    [AllowAnonymous]
    [Area("DocumentVerification")]
    public class ResearchDocumentByMinistryController : Controller
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
        private readonly IEducationDocumentScoreRepository _educationDocumentScoreRepository;
        private readonly IExamFieldRepository _examFieldRepository;
        private readonly IExamRepository _examRepository;

        public ResearchDocumentByMinistryController(IMapper mapper
                                        , IWebHostEnvironment env
                                        , ILogger<EducationDocumentController> logger
                                        , IUserTokenRepository userTokenRepository
                                        , IStudentRepository studentRepository
                                        , ICoefficientRepository coefficientRepository
                                        , IStudentFormSubmissionScoreRepository studentFormSubmissionRepository
                                        , IFormItemCoefficientDefinitionRepository formItemCoefficientDefinitionRepository
                                        , IStudentFormSubmissionDataRepository studentFormSubmissionValueRepository
                                        , IEducationDocumentScoreRepository educationDocumentScoreRepository
                                        , IExamFieldRepository examFieldRepository
                                        , IExamRepository examRepository)
        {
            _mapper = mapper;
            _env = env;
            _logger = logger;
            _userTokenRepository = userTokenRepository;
            _studentRepository = studentRepository;
            _coefficientRepository = coefficientRepository;
            _studentFormSubmissionRepository = studentFormSubmissionRepository;
            _formItemCoefficientDefinitionRepository = formItemCoefficientDefinitionRepository;
            _studentFormSubmissionValueRepository = studentFormSubmissionValueRepository;
            _educationDocumentScoreRepository = educationDocumentScoreRepository;
            _examFieldRepository = examFieldRepository;
            _examRepository = examRepository;
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var exams = await _examRepository.GetAllExamsNoTrackingAsync(cancellationToken); // فرض کنیم متد داریم
            return View(exams);
        }

        // لیست رشته‌ها بر اساس آزمون
        public async Task<IActionResult> FieldDropdown(int examId, CancellationToken cancellationToken)
        {
            var fields = await _examFieldRepository.GetAllFieldsByExamIdAsync(examId, cancellationToken);
            return PartialView("FieldDropdown", fields.Where(f => f.IsPajoheshi == true));
        }


        [HttpGet]
        public async Task<IActionResult> StudentsList(int fieldId, CancellationToken cancellationToken)
        {
            try
            {
                // دریافت همه دانشجویان فعال
                var students = await _studentRepository.GetAllStudentsPajoheshiByFieldIdAsync(fieldId, cancellationToken);

                if (students == null || !students.Any())
                    return PartialView("StudentsList", new List<StudentListViewModel>());


                // نگاشت دانشجویان به ViewModel
                var result = students.Select(e =>
                {
                    return new StudentListViewModel
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

                        // تبدیل IsPajoheshi به بولین
                        IsPajoheshi = e.IsPajoheshi,

                        // نام آزمون از navigation property
                        ExamName = e.Exam?.Title ?? "",

                        // رشته‌ها به صورت رشته کاما جدا شده
                        Fields = e.StudentFields != null && e.StudentFields.Any()
                                    ? string.Join(", ", e.StudentFields.Select(f => f.ExamField.Title))
                                    : "",
                        IsApproved = e.IsApproved,
                        RejectReason= e.RejectReason,
                    };
                })
                .OrderBy(s => s.IsApproved)
                .ThenBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToList();

                return PartialView("StudentsList", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت لیست دانشجویان");
                return StatusCode(500, "خطا در سرور");
            }
        }


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

                    var studentFormSubmissionScoreViewModelNull = await FillStudentFormSubmissionScoreViewModel(id, forms, cancellationToken);
                    return PartialView("Details", studentFormSubmissionScoreViewModelNull);
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
                var studentFormSubmissionScoreViewModel = await FillStudentFormSubmissionScoreViewModel(id, forms, cancellationToken);


                return PartialView("Details", studentFormSubmissionScoreViewModel);
            }
            catch (Exception ex)
            {
                // لاگ دقیق‌تر می‌تونه اضافه بشه در آینده
                return StatusCode(500, "خطا در سرور");
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> PreviewSubmissionFile(int submissionDataId, int studentId, CancellationToken cancellationToken)
        {
            var data = await _studentFormSubmissionValueRepository.GetSubmissionDataByIdAsync(submissionDataId, cancellationToken);
            if (data == null) return NotFound();

            var student = await _studentRepository.GetStudentByIdAsync(studentId, cancellationToken);
            var fileName = Path.GetFileName(data.Value ?? string.Empty);
            if (string.IsNullOrEmpty(fileName)) return NotFound();

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", student.NationalCode ?? string.Empty);
            var filePath = Path.Combine(uploadsFolder, fileName);

            var normalized = Path.GetFullPath(filePath);
            if (!normalized.StartsWith(Path.GetFullPath(uploadsFolder), StringComparison.OrdinalIgnoreCase))
                return Forbid();

            if (!System.IO.File.Exists(normalized)) return NotFound();

            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(normalized, out var contentType))
                contentType = "application/octet-stream";

            // تشخیص نوع فایل
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            bool isImage = ext is ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp";
            bool isPdf = ext == ".pdf";

            //if (!isImage && !isPdf)
            //    return PartialView("PreviewSubmissionFile", model);

            // مسیر مجازی برای نمایش در مرورگر
            var relativePath = $"/uploads/{student.NationalCode}/{fileName}";

            var model = new FilePreviewViewModel
            {
                FileName = fileName,
                FileUrl = relativePath,
                IsPdf = isPdf,
                IsImage = isImage
            };

            return PartialView("PreviewSubmissionFile", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDocumentReview(int studentId, bool isApproved, string rejectReason, CancellationToken cancellationToken)
        {
            if (studentId == 0)
                return Json(new { isSuccess = false, message = "شناسه داوطلب معتبر نیست." });

            if (!User.Identity.IsAuthenticated)
                return Json(new { isSuccess = false, message = "دسترسی غیرمجاز." });

            try
            {
                var curentUserToken = User.Identity.FindFirstValue("Token");
                var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(curentUserToken), cancellationToken);
                var student = await _studentRepository.GetStudentByIdAsync(studentId, cancellationToken);

                student.IsApproved = isApproved;
                student.RejectReason = isApproved ? null : rejectReason;
                student.CreatedBy = token.UserId;
                student.CreatedDate = DateTime.Now;


                await _studentRepository.UpdateStudentAsync(student, cancellationToken);
                await _studentRepository.SaveChangesAsync(cancellationToken);

                return Json(new { isSuccess = true, message = "نتیجه بررسی مدارک داوطلب با موفقیت ذخیره شد." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ذخیره نتیجه بررسی مدارک داوطلب {studentId}", studentId);
                return Json(new { isSuccess = false, message = "خطا در سرور. لطفاً مجدداً تلاش کنید." });
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

        private async Task<StudentFormSubmissionScoreViewModel> FillStudentFormSubmissionScoreViewModel(int studentId, IEnumerable<StudentFormSubmissionScore> studentFormSubmissionScores, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetStudentByIdAsync(studentId, cancellationToken);

            // رشته‌ها رو از student.StudentFields بخون
            var fieldTitles = student.StudentFields?
                .Select(sf => sf.ExamField?.Title)        // یا .Name بسته به مدل
                .Where(title => !string.IsNullOrEmpty(title))
                .ToList() ?? new List<string>();

            // رشته‌ها رو با "-" جدا کن
            var joinedFields = string.Join(" - ", fieldTitles);

            var StudentFormSubmissionScoreViewModel = new StudentFormSubmissionScoreViewModel
            {
                StudentName = student.FirstName + " " + student.LastName,
                NationalCode = student.NationalCode,
                StudentId = student.Id,
                FielsTitle = joinedFields,
                Forms = studentFormSubmissionScores.ToList()
            };
            return StudentFormSubmissionScoreViewModel;
        }
    }
}
