using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Contracts.Exam;
using SanjeshP.Mosahebe.Data.Contracts.StudentForm;
using SanjeshP.Mosahebe.Data.Contracts.Students;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using SanjeshP.Mosahebe.Entities.StudentForm;
using SanjeshP.Mosahebe.Web.Areas.DocumentVerification.ViewModels;
using SanjeshP.Mosahebe.Web.Areas.Student.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Entities.Student;
using System.Security.Claims;

namespace SanjeshP.Mosahebe.Web.Areas.DocumentVerification.Controllers
{
    [AllowAnonymous]
    [Area("DocumentVerification")]
    public class ResearchDocumentByUnivController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<EducationDocumentController> _logger;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ICoefficientRepository _coefficientRepository;
        private readonly IStudentFormSubmissionRepository _studentFormSubmissionRepository;
        private readonly IFormItemCoefficientDefinitionRepository _formItemCoefficientDefinitionRepository;
        private readonly IStudentFormSubmissionDataRepository _studentFormSubmissionValueRepository;
        private readonly IEducationDocumentScoreRepository _educationDocumentScoreRepository;
        private readonly IExamFieldRepository _examFieldRepository;
        private readonly IExamRepository _examRepository;
        private readonly IResearchDocumentScoreRepository _researchDocumentScoreRepository;
        private readonly IUserFieldRepository _userFieldRepository;

        public ResearchDocumentByUnivController(IMapper mapper
                                        , IWebHostEnvironment env
                                        , ILogger<EducationDocumentController> logger
                                        , IUserTokenRepository userTokenRepository
                                        , IStudentRepository studentRepository
                                        , ICoefficientRepository coefficientRepository
                                        , IStudentFormSubmissionRepository studentFormSubmissionRepository
                                        , IFormItemCoefficientDefinitionRepository formItemCoefficientDefinitionRepository
                                        , IStudentFormSubmissionDataRepository studentFormSubmissionValueRepository
                                        , IEducationDocumentScoreRepository educationDocumentScoreRepository
                                        , IExamFieldRepository examFieldRepository
                                        , IExamRepository examRepository
                                        , IResearchDocumentScoreRepository researchDocumentScoreRepository
                                        , IUserFieldRepository userFieldRepository)
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
            _researchDocumentScoreRepository = researchDocumentScoreRepository;
            _userFieldRepository = userFieldRepository;
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var exams = await _examRepository.GetAllExamsNoTrackingAsync(cancellationToken); // فرض کنیم متد داریم
            return View(exams);
        }

        // لیست رشته‌ها بر اساس آزمون
        public async Task<IActionResult> FieldDropdown(int examId, CancellationToken cancellationToken)
        {
            var curentUserToken = User.Identity.FindFirstValue("Token");
            var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(curentUserToken), cancellationToken);

            // رشته‌هایی که کاربر به آنها دسترسی دارد
            var userFields = await _userFieldRepository.GetByUserIdAsync(token.UserId, cancellationToken);

            // تمام رشته‌های آزمون
            var fields = await _examFieldRepository.GetAllFieldsByExamIdAsync(examId, cancellationToken);

            // فیلتر بر اساس دسترسی کاربر و فقط پژوهشی
            var accessibleFields = fields
                .Where(f => f.IsPajoheshi && userFields.Any(uf => uf.ExamFieldID == f.Id))
                .ToList();

            return PartialView("FieldDropdown", accessibleFields);
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
                var result = students.Where(s => s.IsApproved == true).Select(e =>
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
                        ResearchDocumentScore = e.ResearchDocumentScore,
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
                        IsApproved = e.IsApproved ?? false,
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

                    var studentFormSubmissionViewModelNull = await FillStudentFormSubmissionViewModel(id, forms, cancellationToken);
                    return PartialView("Details", studentFormSubmissionViewModelNull);
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
                var studentFormSubmissionViewModel = await FillStudentFormSubmissionViewModel(id, forms, cancellationToken);


                return PartialView("Details", studentFormSubmissionViewModel);
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
        public async Task<IActionResult> SaveResearchDocumentScore(int StudentId, StudentStatus Status, double FinalScore, string RejectReason, CancellationToken cancellationToken)
        {
            if (StudentId == 0)
                return BadRequest("شناسه دانشجو معتبر نیست.");

            // دریافت UserId فعلی (در صورت نیاز)
            var curentUserToken = User.Identity.FindFirstValue("Token");
            var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(curentUserToken), cancellationToken);

            // کنترل وضعیت‌ها
            switch (Status)
            {
                case StudentStatus.Absent:
                    FinalScore = 0;
                    RejectReason = null;
                    break;

                case StudentStatus.Rejected:
                    FinalScore = 0;
                    if (string.IsNullOrWhiteSpace(RejectReason))
                        return Json(new { isSuccess = false, message = "دلیل عدم تأیید الزامی است." });
                    break;

                case StudentStatus.Approved:
                    if (FinalScore < 0 || FinalScore > 100)
                        return Json(new { isSuccess = false, message = "نمره باید بین 0 تا 100 باشد." });
                    RejectReason = null;
                    break;
            }


            // بررسی رکورد موجود
            var record = await _researchDocumentScoreRepository.GetResearchDocumentScoreByStudentIdUserId(StudentId, token.UserId, cancellationToken);
            if (record != null)
            {
                // ویرایش رکورد موجود
                record.FinalTotalScore = FinalScore;
                record.RejectReason = RejectReason;
                record.IsApproved = Status;
                record.CreatedBy = token.UserId;
                record.HostIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                record.UpdatedDate = DateTime.Now; // اگر BaseEntity این فیلد را دارد
                _researchDocumentScoreRepository.Update(record);
            }
            else
            {
                // ایجاد رکورد جدید
                record = new ResearchDocumentScore
                {
                    StudentId = StudentId,
                    FinalTotalScore = FinalScore,
                    RejectReason = RejectReason,
                    IsApproved = Status,
                    CreatedBy = token.UserId,
                    HostIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    CreatedDate = DateTime.Now // اگر BaseEntity این فیلد را دارد
                };
                await _researchDocumentScoreRepository.AddAsync(record, cancellationToken);
            }

            await _researchDocumentScoreRepository.SaveChangesAsync(cancellationToken);

            return Json(new { isSuccess = true, message = "امتیاز با موفقیت ذخیره شد." });
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
            // var parent = data.StudentFormSubmission; // اگر Include کرده‌اید
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

        private async Task<StudentFormSubmissionViewModel> FillStudentFormSubmissionViewModel(int studentId, IEnumerable<StudentFormSubmission> studentFormSubmissions, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetStudentByIdAsync(studentId, cancellationToken);

            // رشته‌ها رو از student.StudentFields بخون
            var fieldTitles = student.StudentFields?
                .Select(sf => sf.ExamField?.Title)        // یا .Name بسته به مدل
                .Where(title => !string.IsNullOrEmpty(title))
                .ToList() ?? new List<string>();

            // رشته‌ها رو با "-" جدا کن
            var joinedFields = string.Join(" - ", fieldTitles);

            var StudentFormSubmissionViewModel = new StudentFormSubmissionViewModel
            {
                StudentName = student.FirstName + " " + student.LastName,
                NationalCode = student.NationalCode,
                StudentId = student.Id,
                FielsTitle = joinedFields,
                Forms = studentFormSubmissions.ToList()
            };
            return StudentFormSubmissionViewModel;
        }
    }
}
