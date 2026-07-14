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
using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Data.Repositories.Students;
using System.Security.Claims;
using SanjeshP.Mosahebe.Data.Contracts.Exam;
using SanjeshP.Mosahebe.Data.Repositories.Exam;

namespace SanjeshP.Mosahebe.Web.Areas.DocumentVerification.Controllers
{
    [Area("DocumentVerification")]
    public class EducationDocumentController : Controller
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
        private readonly IStudentFormSubmissionScoreRepository _studentFormSubmissionScoreRepository;
        private readonly IUserFieldRepository _userFieldRepository;


        public EducationDocumentController(IMapper mapper
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
                                        , IStudentFormSubmissionScoreRepository studentFormSubmissionScoreRepository
            , IUserFieldRepository userFieldRepository)
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
            this._educationDocumentScoreRepository = educationDocumentScoreRepository;
            this._examFieldRepository = examFieldRepository;
            this._examRepository = examRepository;
            this._studentFormSubmissionScoreRepository = studentFormSubmissionScoreRepository;
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
            //var fields = await _examFieldRepository.GetAllFieldsByExamIdAsync(examId, cancellationToken);
            //return PartialView("FieldDropdown", fields);
            var currentUserToken = User.FindFirstValue("Token");
            var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(currentUserToken), cancellationToken);

            var fields = await _examFieldRepository.GetAllFieldsByExamIdAsync(examId, cancellationToken);
            var userFields = await _userFieldRepository.GetExamFieldIdsByUserIdAsync(token.UserId, cancellationToken);

            var result = fields.Where(f => userFields.Contains(f.Id)).ToList();

            return PartialView("FieldDropdown", result);
        }

        [HttpGet]
        public async Task<IActionResult> StudentsList(int fieldId, CancellationToken cancellationToken)
        {
            try
            {
                var currentUserToken = User.FindFirstValue("Token");
                var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(currentUserToken), cancellationToken);
                // دریافت همه دانشجویان فعال
                var students = await _studentRepository.GetAllStudentsByFieldIdUserIdAsync(fieldId, token.UserId, cancellationToken);

                if (students == null || !students.Any())
                    return PartialView("StudentsList", new List<StudentListViewModel>());

                // جمع‌آوری شناسه‌های دانشجویان
                var studentIds = students.Select(s => s.Id).ToList();

                // دریافت نمرات نهایی همه دانشجویان به صورت Dictionary<int,double>
                var scores = await _educationDocumentScoreRepository
                    .GetScoresByStudentIdsAsync(studentIds, token.UserId, cancellationToken);


                // نگاشت دانشجویان به ViewModel
                var result = students.Select(e =>
                {
                    var activeScore = e.EducationDocumentScores?
                        .Where(ed => ed.IsActive && !ed.IsDeleted && ed.CreatedBy == token.UserId)
                        .OrderByDescending(ed => ed.CreatedDate)   // یا by Id اگر CreatedDate ندارید
                        .FirstOrDefault();

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

                        // نوع دوره آزمون به متن
                        //ExamCourseType = e.ExamCourseType == 1 ? "آموزشی" : "پژوهشی",

                        // رشته‌ها به صورت رشته کاما جدا شده
                        Fields = e.StudentFields != null && e.StudentFields.Any()
                                    ? string.Join(", ", e.StudentFields.Select(f => f.ExamField.Title))
                                    : "",
                        EducationDocumentScore = activeScore, // اگر VM فیلد نوع collection لازم نیست
                        CountScoreByCurentUser = e.EducationDocumentScores.Count(),

                        FinalTotalScore = scores.ContainsKey(e.Id) ? scores[e.Id] : (activeScore?.FinalTotalScore ?? 0)
                    };
                })
                // مرتب سازی: آن‌هایی که activeScore دارند بروند ته لیست
                .OrderBy(s => (s.EducationDocumentScore != null && !s.EducationDocumentScore.IsDeleted) ? 1 : 0)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveScores([FromForm] List<FormDataInputViewModel> Forms, CancellationToken cancellationToken)
        {
            if (Forms == null || !Forms.Any())
            {
                ModelState.AddModelError(string.Empty, "داده‌ای برای ذخیره ارسال نشده است");
                return BadRequest(ModelState);
            }

            if (!User.Identity.IsAuthenticated) return Forbid();

            var currentUserToken = User.FindFirstValue("Token");
            var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(currentUserToken), cancellationToken);

            try
            {
                int? studentId = null;
                foreach (var formInput in Forms)
                {
                    if (cancellationToken.IsCancellationRequested) break; // توقف در صورت کنسل شدن عملیات
                    if (formInput == null) continue;

                    var submissionScore = await _studentFormSubmissionRepository
                        .GetDetailsForEditAsync(formInput.FormSubmissionScoreId, cancellationToken);

                    if (submissionScore == null)
                    {
                        _logger.LogWarning("submissionScore not found id={Id}", formInput.FormSubmissionScoreId);
                        continue;
                    }

                    studentId ??= submissionScore.StudentId;


                    var oneMaxScore = submissionScore.FormItem?.OneMaxScore ?? 0; // حداکثر امتیاز فرم

                    StudentFormSubmissionScore scoreToSave = new StudentFormSubmissionScore();

                    // اگر فرم تایید شده باشد
                    if (formInput.IsApproved == true)
                    {
                        scoreToSave.IsApproved = true; // علامت تایید
                        scoreToSave.RejectReason = null; // هیچ دلیل ردی وجود ندارد

                        var coefficientId = submissionScore.FormItem?.FormItemCoefficient?.CoefficientId ?? 0;
                        var coeffs = await _coefficientRepository.GetByCoefficientIdAsync(coefficientId, cancellationToken);

                        // حالت ۱: فرم‌های بدون ورودی دستی (مثل معدل، کتاب، زبان و ...)
                        if (coefficientId == 0)
                        {
                            scoreToSave.CalculatedScore = oneMaxScore;
                        }
                        else if (coefficientId == 1 || coefficientId == 4)
                        {
                            scoreToSave.CalculatedScore = CalculateScore(submissionScore, coeffs);
                        }
                        else if (coefficientId == 2)
                        {
                            if (double.TryParse(formInput.ScoreValue, out var score))
                            {
                                double coefficientValue = CalculateScore(submissionScore, coeffs);

                                // بررسی حداقل و حداکثر
                                if (score < 0) score = 0;
                                if (oneMaxScore > 0 && score > oneMaxScore) score = oneMaxScore;
                                switch (coefficientValue)
                                {
                                    case 2: // ضریب
                                        scoreToSave.CalculatedScore = score / 5;
                                        break;
                                    case 1: // ضریب
                                        scoreToSave.CalculatedScore = score / 10;
                                        break;
                                    case 0.5: // ضریب
                                        scoreToSave.CalculatedScore = score / 20;
                                        break;
                                }
                            }
                            else
                            {
                                scoreToSave.CalculatedScore = 0; // اگر نمره نامعتبر باشد، صفر در نظر گرفته می‌شود
                            }
                        }
                        else if (coefficientId == 3)
                        {
                            // حالت ۲: فرم دارای ورودی دستی (CoefficientId = 3)
                            // ذخیره نمره وارد شده
                            if (double.TryParse(formInput.ScoreValue, out var score))
                            {
                                var coefficientValue = CalculateScore(submissionScore, coeffs);

                                // بررسی حداقل و حداکثر
                                if (score < 0) score = 0;
                                if (oneMaxScore > 0 && score > oneMaxScore) score = oneMaxScore;

                                scoreToSave.CalculatedScore = score * coefficientValue;
                            }
                            else
                            {
                                scoreToSave.CalculatedScore = 0; // اگر نمره نامعتبر باشد، صفر در نظر گرفته می‌شود
                            }
                        }
                        else if (coefficientId == 5)  // محاسبه نمراتی که میخواند توی بازه 1 تا 10 استاد نمره بدهد
                        {
                            if (double.TryParse(formInput.ScoreValue, out var score))
                            {
                                // بررسی حداقل و حداکثر
                                if (score < 0) score = 0;
                                if (oneMaxScore > 0 && score > oneMaxScore) score = oneMaxScore;
                                scoreToSave.CalculatedScore = score / 5;
                            }
                            else
                            {
                                scoreToSave.CalculatedScore = 0; // اگر نمره نامعتبر باشد، صفر در نظر گرفته می‌شود
                            }
                        }
                        else if (coefficientId == 6)
                        {

                            var coefficientValue = CalculateScore(submissionScore, coeffs);

                            if (coefficientValue != 0)
                            {
                                scoreToSave.CalculatedScore = coefficientValue;

                            }
                            else
                            {
                                scoreToSave.CalculatedScore = 0;
                            }

                        }
                        else if (coefficientId == 7)
                        {
                            var coefficientValue = CalculateScore(submissionScore, coeffs);
                            if (coefficientValue != 0)
                            {
                                scoreToSave.CalculatedScore = coefficientValue * oneMaxScore;

                            }
                            else
                            {
                                scoreToSave.CalculatedScore = 0;
                            }
                        }
                    }
                    else // اگر فرم تایید نشده باشد
                    {
                        scoreToSave.IsApproved = false;
                        scoreToSave.CalculatedScore = 0; // نمره صفر برای فرم رد شده
                        scoreToSave.RejectReason = formInput.RejectReason?.Trim(); // ذخیره دلیل رد
                    }

                    // بررسی وجود رکورد قبلی
                    var existingScore = await _studentFormSubmissionScoreRepository
                        .GetExistingScoreAsync(submissionScore.StudentId, submissionScore.Id, token.UserId, cancellationToken);
                    if (existingScore != null)
                    {
                        // رکورد قبلی وجود دارد → Update
                        existingScore.IsApproved = scoreToSave.IsApproved;
                        existingScore.CalculatedScore = scoreToSave.CalculatedScore;
                        existingScore.RejectReason = scoreToSave.RejectReason;
                        existingScore.UpdatedDate = DateTime.Now;
                        existingScore.UserId = token.UserId;
                        existingScore.HostIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                        await _studentFormSubmissionScoreRepository.UpdateStudentFormSubmissionScoreAsync(existingScore, cancellationToken);
                    }
                    else
                    {
                        // رکورد جدید اضافه می‌شود
                        scoreToSave.StudentFormSubmissionId = submissionScore.Id;
                        scoreToSave.StudentId = submissionScore.StudentId;
                        scoreToSave.UpdatedDate = DateTime.Now; // تاریخ بروزرسانی
                        scoreToSave.CreatedBy = token.UserId;
                        scoreToSave.UserId = token.UserId;
                        scoreToSave.HostIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                        await _studentFormSubmissionScoreRepository.AddStudentFormSubmissionScoreAsync(scoreToSave, cancellationToken);
                    }
                }

                await _studentFormSubmissionScoreRepository.SaveChangesAsync(cancellationToken);

                if (studentId > 0)
                {
                    await _educationDocumentScoreRepository.CalculateAndSaveFinalScoreAsync(studentId.Value, token.UserId, cancellationToken);
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ذخیره نمرات فرم‌ها");
                return StatusCode(500, new { success = false, message = "خطا در سرور" });
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
        private double CalculateScore(StudentFormSubmission submission, IEnumerable<Coefficient> coeffs)
        {
            var coeffId = submission.FormItem?.FormItemCoefficient?.CoefficientId ?? 0;

            switch (coeffId)
            {
                case 1: // معدل
                    // از فیلد های پیش فرض جدول FormItemCoefficientDefinition میگرده ببینه فیلدی با عنوان Grade پیدا میکنه که کد ضریب 1 داره یا نه
                    var gradeField = submission.StudentFormSubmissionDatas
                        .FirstOrDefault(f => f.FormItemCoefficientDefinition?.Name == "Grade");
                    if (gradeField != null && double.TryParse(gradeField.Value, out var grade))
                    {
                        var c = coeffs.FirstOrDefault(x => grade >= x.RangeStart && grade <= x.RangeEnd);
                        // حتما بررسی شود نمره
                        c.Value = c.Value / submission.FormItem.OneMaxScore;  // ضریب بایستی با  حداکثر نمره تکی تناسب بسته شود
                        return c?.Value ?? 0;
                    }
                    break;
                case 2: // کتاب
                        // از فیلد های پیش فرض جدول FormItemCoefficientDefinition میگرده ببینه فیلدی با عنوان BookType پیدا میکنه که کد ضریب 2 داره یا نه
                    var authField_Book = submission.StudentFormSubmissionDatas
                        .FirstOrDefault(f => f.FormItemCoefficientDefinition?.Name == "BookType");

                    // فرض: authField از نوع StudentFormSubmissionData است
                    if (authField_Book == null || string.IsNullOrWhiteSpace(authField_Book.Value))
                    {
                        throw new InvalidOperationException("مقدار فیلد همکاری در نگارش خالی است.");
                    }

                    // تلاش برای تبدیل رشته به عدد
                    if (int.TryParse(authField_Book.Value, out var valueId_Book))
                    {
                        var c = coeffs.FirstOrDefault(x => x.Id == valueId_Book);
                        if (c == null)
                            throw new InvalidOperationException($"ضریب معتبر برای مقدار انتخابی ({valueId_Book}) یافت نشد.");

                        // مقدار معتبر پیدا شد
                        return c.Value;
                    }
                    else
                    {
                        throw new InvalidOperationException($"مقدار فیلد همکاری '{authField_Book.Value}' عددی نیست.");
                    }
                case 3: // دارای همکار
                        // از فیلد های پیش فرض جدول FormItemCoefficientDefinition میگرده ببینه فیلدی با عنوان AuthPo پیدا میکنه که کد ضریب 3 داره یا نه
                    var authField = submission.StudentFormSubmissionDatas
                        .FirstOrDefault(f => f.FormItemCoefficientDefinition?.Name == "AuthPo");
                    // فرض: authField از نوع StudentFormSubmissionData است
                    if (authField == null || string.IsNullOrWhiteSpace(authField.Value))
                    {
                        throw new InvalidOperationException("مقدار فیلد همکاری در نگارش خالی است.");
                    }

                    // تلاش برای تبدیل رشته به عدد
                    if (int.TryParse(authField.Value, out var valueId))
                    {
                        var c = coeffs.FirstOrDefault(x => x.Id == valueId);
                        if (c == null)
                            throw new InvalidOperationException($"ضریب معتبر برای مقدار انتخابی ({valueId}) یافت نشد.");

                        // مقدار معتبر پیدا شد
                        return c.Value;
                    }
                    else
                    {
                        throw new InvalidOperationException($"مقدار فیلد همکاری '{authField.Value}' عددی نیست.");
                    }
                case 4: // زبان
                        // از فیلد های پیش فرض جدول FormItemCoefficientDefinition میگرده ببینه فیلدی با عنوان Number پیدا میکنه که کد ضریب 4 داره یا نه
                    var langField = submission.StudentFormSubmissionDatas
                        .FirstOrDefault(f => f.FormItemCoefficientDefinition?.Name == "Number");

                    var drop = submission.StudentFormSubmissionDatas
                        .Where(c => c.InputType == InputType.Dropdown)
                        .FirstOrDefault();
                    if (langField != null && float.TryParse(langField.Value, out var langScore))
                    {
                        var c = coeffs.FirstOrDefault(x => (langScore >= x.RangeStart && langScore <= x.RangeEnd) && drop.Value == x.Id.ToString());
                        return c?.Value ?? 0;
                    }
                    break;
                case 6: // تدریس
                        // از فیلد های پیش فرض جدول FormItemCoefficientDefinition میگرده ببینه فیلدی با عنوان EDU پیدا میکنه که کد ضریب 6 داره یا نه
                    var EDUField = submission.StudentFormSubmissionDatas
                        .FirstOrDefault(f => f.FormItemCoefficientDefinition?.Name == "EDU");

                    var EDUdrop = submission.StudentFormSubmissionDatas
                                           .Where(c => c.InputType == InputType.Dropdown)
                                           .FirstOrDefault();


                    if (EDUField != null && double.TryParse(EDUField.Value, out var EDUgrade))
                    {
                        var c = coeffs.FirstOrDefault(x => x.Id.ToString() == EDUdrop.Value);

                        return c?.Value ?? 0;
                    }
                    break;
                case 7: // دارای همکار
                        // از فیلد های پیش فرض جدول FormItemCoefficientDefinition میگرده ببینه فیلدی با عنوان AuthPo پیدا میکنه که کد ضریب 3 داره یا نه
                    var ReviewField = submission.StudentFormSubmissionDatas
                        .FirstOrDefault(f => f.FormItemCoefficientDefinition?.Name == "Review");
                    // فرض: authField از نوع StudentFormSubmissionData است
                    if (ReviewField == null || string.IsNullOrWhiteSpace(ReviewField.Value))
                    {
                        throw new InvalidOperationException("مقدار فیلد همکاری در نگارش خالی است.");
                    }

                    // تلاش برای تبدیل رشته به عدد
                    if (int.TryParse(ReviewField.Value, out var ReviewvalueId))
                    {
                        var c = coeffs.FirstOrDefault(x => x.Id == ReviewvalueId);
                        if (c == null)
                            throw new InvalidOperationException($"ضریب معتبر برای مقدار انتخابی ({ReviewvalueId}) یافت نشد.");

                        // مقدار معتبر پیدا شد
                        return c.Value;
                    }
                    else
                    {
                        throw new InvalidOperationException($"مقدار فیلد همکاری '{ReviewField.Value}' عددی نیست.");
                    }
            }
            return 0;
        }

        private async Task<StudentFormSubmissionViewModel> FillStudentFormSubmissionViewModel(int studentId, IEnumerable<StudentFormSubmission> studentFormSubmissions, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetStudentDetailsOnReviewDocument(studentId, cancellationToken);

            // رشته‌ها رو از student.StudentFields بخون
            var fieldTitles = student.StudentFields?
                .Select(sf => sf.ExamField?.Title)        // یا .Name بسته به مدل
                .Where(title => !string.IsNullOrEmpty(title) )
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
