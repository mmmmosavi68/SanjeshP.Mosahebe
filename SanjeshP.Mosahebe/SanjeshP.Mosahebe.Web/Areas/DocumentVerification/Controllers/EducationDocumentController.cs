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
        private readonly IStudentFormSubmissionRepository _studentFormSubmissionRepository;
        private readonly IFormItemCoefficientDefinitionRepository _formItemCoefficientDefinitionRepository;
        private readonly IStudentFormSubmissionValueRepository _studentFormSubmissionValueRepository;

        public EducationDocumentController(IMapper mapper
                                        , IWebHostEnvironment env
                                        , ILogger<EducationDocumentController> logger
                                        , IUserTokenRepository userTokenRepository
                                        , IStudentRepository studentRepository
                                        , IStudentFormSubmissionRepository studentFormSubmissionRepository
                                        , IFormItemCoefficientDefinitionRepository formItemCoefficientDefinitionRepository
                                        , IStudentFormSubmissionValueRepository studentFormSubmissionValueRepository)
        {
            _mapper = mapper;
            _env = env;
            _logger = logger;
            _userTokenRepository = userTokenRepository;
            _studentRepository = studentRepository;
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

            var forms = await _studentFormSubmissionRepository.GetAllByStudentIdAsync(id, cancellationToken);
            if (forms == null) return NotFound();

            return PartialView("Details", forms);
        }

        [HttpPost]
        public async Task<IActionResult> SaveScores([FromForm] List<FormDataInputViewModel> Forms, CancellationToken cancellationToken)
        {
            if (Forms == null || !Forms.Any())
            {
                ModelState.AddModelError(string.Empty, "داده‌ای برای ذخیره ارسال نشده است");
                return BadRequest(ModelState);
            }

            if (!User.Identity.IsAuthenticated) return Forbid();

            try
            {
                foreach (var formInput in Forms)
                {
                    if (cancellationToken.IsCancellationRequested) break;
                    if (formInput == null) continue;

                    var submissionScore = await _studentFormSubmissionRepository.GetDetailsForEditAsync(formInput.FormSubmissionScoreId, cancellationToken);
                    if (submissionScore == null)
                    {
                        _logger.LogWarning("submissionScore not found id={Id}", formInput.FormSubmissionScoreId);
                        continue;
                    }

                    var formItem = submissionScore.FormItem;
                    var oneMaxScore = formItem?.OneMaxScore ?? 0;
                    int totalScoreForForm = 0;
                    bool anyRejected = false;

                    foreach (var dataInput in formInput.Datas ?? Enumerable.Empty<SubmissionDataInputViewModel>())
                    {
                        if (cancellationToken.IsCancellationRequested) break;
                        if (dataInput == null) continue;

                        var submissionData = submissionScore.StudentFormSubmissionDatas
                            .FirstOrDefault(d => d.Id == dataInput.SubmissionDataId);

                        if (submissionData == null)
                        {
                            _logger.LogWarning("submissionData not found id={Id}", dataInput.SubmissionDataId);
                            continue;
                        }

                        if (dataInput.FormItemCoefficientDefinitionId.HasValue)
                        {
                            submissionData.FormItemCoefficientDefinitionId = dataInput.FormItemCoefficientDefinitionId;
                        }

                        if (dataInput.IsApproved == true)
                        {
                            if (dataInput.ManualScore.HasValue)
                            {
                                var manual = dataInput.ManualScore.Value;
                                if (manual < 0) manual = 0;
                                if (oneMaxScore > 0 && manual > oneMaxScore) manual = oneMaxScore;

                                submissionData.Value = manual.ToString(CultureInfo.InvariantCulture);
                                totalScoreForForm += manual;
                            }
                            else if (dataInput.FormItemCoefficientDefinitionId.HasValue)
                            {
                                var coeffDef = await _formItemCoefficientDefinitionRepository.GetFormItemCoefficientDefinitionWithCoefficientsAsync(dataInput.FormItemCoefficientDefinitionId.Value, cancellationToken);
                                if (coeffDef?.Coefficients != null && coeffDef.Coefficients.Any())
                                {
                                    if (float.TryParse(submissionData.Value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var numericValue))
                                    {
                                        var matched = coeffDef.Coefficients
                                            .FirstOrDefault(x => numericValue >= x.RangeStart && numericValue <= x.RangeEnd);
                                        if (matched != null)
                                        {
                                            var score = (int)Math.Round(matched.Value);
                                            if (oneMaxScore > 0 && score > oneMaxScore) score = oneMaxScore;
                                            totalScoreForForm += score;
                                        }
                                        else
                                        {
                                            _logger.LogInformation("No coefficient match for submissionData {Id} value {Value}", submissionData.Id, submissionData.Value);
                                        }
                                    }
                                    else
                                    {
                                        _logger.LogInformation("Unable to parse numeric value for submissionData {Id} value {Value}", submissionData.Id, submissionData.Value);
                                    }
                                }
                            }
                        }
                        else
                        {
                            anyRejected = true;
                            if (!string.IsNullOrWhiteSpace(dataInput.RejectReason))
                            {
                                _logger.LogInformation("Reject reason for submissionData {Id}: {Reason}", submissionData.Id, dataInput.RejectReason);
                                // در صورت وجود فیلد RejectReason در مدل، اینجا مقداردهی شود
                            }
                        }
                    }

                    submissionScore.Score = totalScoreForForm;
                    submissionScore.IsApproved = !anyRejected;
                    submissionScore.UpdatedDate = DateTime.UtcNow;
                    await _studentFormSubmissionRepository.UpdateAsync(submissionScore, cancellationToken); // فرض وجود متد UpdateAsync در Base repository
                }

                await _studentFormSubmissionRepository.SaveChangesAsync(cancellationToken); // یا اگر SaveChanges در repo نیست، فراخوانی متد مناسب
                return RedirectToAction("Index");
            }
            //catch (OperationCanceledException)
            //{
            //    _logger.LogWarning("SaveScores cancelled");
            //    return StatusCode(StatusCodes.Status499ClientClosedRequest);
            //}
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while saving scores");
                return StatusCode(StatusCodes.Status500InternalServerError, "خطا در ذخیره‌سازی نمرات");
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DownloadSubmissionFile(int submissionDataId,int studentId, CancellationToken cancellationToken)
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
