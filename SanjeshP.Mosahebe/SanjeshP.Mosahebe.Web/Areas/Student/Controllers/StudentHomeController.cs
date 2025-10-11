using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SanjeshP.Mosahebe.Convertor;
using SanjeshP.Mosahebe.Data.Contracts.StudentForm;
using SanjeshP.Mosahebe.Data.Contracts.Students;
using SanjeshP.Mosahebe.Entities.StudentForm;
using SanjeshP.Mosahebe.Web.Areas.Exam.ViewModels.Exam;
using SanjeshP.Mosahebe.Web.Areas.Student.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Web.Areas.Student.Controllers
{
    [Area("Student")]
    public class StudentHomeController : Controller
    {
        private readonly IMapper _mapper;

        private readonly IFormItemRepository _formItemRepository;
        private readonly IFormItemDefinitionRepository _formItemDefinitionRepository;
        private readonly IFormItemCoefficientDefinitionRepository _coefficientDefinitionRepository;
        private readonly IFormItemCoefficientRepository _formItemCoefficientRepository;
        private readonly IStudentFormSubmissionScoreRepository _submissionScoreRepository;
        private readonly IStudentFormSubmissionDataRepository _submissionDataRepository;
        private readonly ICoefficientRepository _coefficientRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IStudentRepository _studentRepository;


        public StudentHomeController(IMapper mapper,
            IFormItemRepository formItemRepository,
            IFormItemDefinitionRepository formItemDefinitionRepository,
            IFormItemCoefficientDefinitionRepository coefficientDefinitionRepository,
            IFormItemCoefficientRepository formItemCoefficientRepository,
            IStudentFormSubmissionScoreRepository submissionScoreRepository,
            IStudentFormSubmissionDataRepository submissionDataRepository,
            IStudentRepository studentRepository,
            ICoefficientRepository coefficientRepository,
            IWebHostEnvironment env)
        {
            _mapper = mapper;

            _formItemRepository = formItemRepository;
            _formItemDefinitionRepository = formItemDefinitionRepository;
            _coefficientDefinitionRepository = coefficientDefinitionRepository;
            _formItemCoefficientRepository = formItemCoefficientRepository;
            _submissionScoreRepository = submissionScoreRepository;
            _submissionDataRepository = submissionDataRepository;
            this._coefficientRepository = coefficientRepository;
            this._env = env;
        }

        // GET: /Student/StudentHome
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var forms = await _formItemRepository.GetFormItemsWithIncludesAsync(cancellationToken);
            return View(forms);
        }

        // GET: /Student/StudentHome/LoadForm?formId=1
        public async Task<IActionResult> LoadForm(int formId, CancellationToken cancellationToken)
        {
            var form = await _formItemRepository.GetByFormItemIdAsync(formId, cancellationToken);
            if (form == null)
                return NotFound();

            //var FormItemDefinitions = await _formItemDefinitionRepository.GetByFormItemIdAsync(form.Id, cancellationToken);
            //var FormItemCoefficientDefinitions = await _coefficientDefinitionRepository.GetByCoefficientIdAsync(form.FormItemCoefficientId, cancellationToken);

            var viewModel = new FormSubmissionViewModel
            {
                FormId = form.Id,
                FormTitle = form.Title,
                Fields = await _formItemDefinitionRepository.GetByFormItemIdAsync(form.Id, cancellationToken)
            };

            if (form.FormItemCoefficientId > 0)
            {

                var formItemcoefficient = await _formItemCoefficientRepository.GetByIdAsync(form.FormItemCoefficientId, cancellationToken);
                var coefficients = await _coefficientRepository.GetByCoefficientIdAsync(formItemcoefficient.CoefficientId, cancellationToken);
                viewModel.CoefficientFields = await _coefficientDefinitionRepository.GetByCoefficientIdAsync(formItemcoefficient.CoefficientId, cancellationToken);
                var coef = await _formItemCoefficientRepository.GetByCoefficientIdAsync(form.FormItemCoefficientId, cancellationToken);
                viewModel.Coefficients = coef.ToList();
                // اضافه کردن coefficients از _coefficientRepository
                viewModel.CoefficientList = coefficients.ToList();
            }

            return View("LoadForm", viewModel);
        }

        // POST: /Student/StudentHome/SubmitForm
        [HttpPost]
        public async Task<IActionResult> SubmitForm(FormSubmissionViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest("اطلاعات فرم ناقص است");

            var studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var studentNationalCode = User.FindFirst("NationalCode")?.Value; // فرض بر این است که کد ملی تو Claims هست

            if (string.IsNullOrEmpty(studentNationalCode))
                return BadRequest("کد ملی داوطلب نامعتبر است.");

            // حذف ارسال قبلی اگر وجود دارد
            //var previousSubmission = await _submissionScoreRepository
            //    .GetByStudentAndFormAsync(studentId, model.FormId, cancellationToken);
            //if (previousSubmission != null)
            //{
            //    previousSubmission.IsDeleted = true;
            //    await _submissionScoreRepository.UpdateAsync(previousSubmission, cancellationToken);
            //}

            // ایجاد رکورد جدید برای Submission
            var submission = new StudentFormSubmissionScore
            {
                FormItemId = model.FormId,
                StudentId = studentId,
                CreatedDate = DateTime.UtcNow
            };
            await _submissionScoreRepository.AddStudentFormSubmissionAsync(submission, cancellationToken);

            // مسیر پایه برای ذخیره فایل‌ها
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", studentNationalCode);
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            // ذخیره فیلدهای Definition
            var fields = await _formItemDefinitionRepository.GetByFormItemIdAsync(model.FormId, cancellationToken);
            foreach (var field in fields)
            {
                string value = null;

                if (field.inputType == InputType.File)
                {
                    var file = Request.Form.Files[field.Name];
                    if (file != null && file.Length > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var savePath = Path.Combine(basePath, fileName);

                        using (var stream = new FileStream(savePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        value = fileName;
                    }
                }
                else
                {
                    value = Request.Form[field.Name];
                }

                if (!string.IsNullOrEmpty(value))
                {
                    var studentFormSubmissionData = new StudentFormSubmissionData
                    {
                        StudentFormSubmissionScoreId = submission.Id,
                        FormItemDefinitionId = field.Id,
                        InputType = field.inputType,
                        Value = value
                    };
                    await _submissionDataRepository.AddStudentFormSubmissionValueAsync(studentFormSubmissionData, cancellationToken);
                }
            }

            // ذخیره فیلدهای Coefficient
            var formItem = await _formItemRepository.GetByFormItemIdAsync(model.FormId, cancellationToken);
            if (formItem.FormItemCoefficientId > 0)
            {
                var formItemCoefficient = await _formItemCoefficientRepository.GetByIdAsync(formItem.FormItemCoefficientId, cancellationToken);
                var coefFields = await _coefficientDefinitionRepository.GetByCoefficientIdAsync(formItemCoefficient.CoefficientId, cancellationToken);
                foreach (var coefField in coefFields)
                {
                    string value = null;

                    if (coefField.inputType == InputType.File)
                    {
                        var file = Request.Form.Files[coefField.Name];
                        if (file != null && file.Length > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var savePath = Path.Combine(basePath, fileName);

                            using (var stream = new FileStream(savePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            value = fileName;
                        }
                    }
                    else
                    {
                        value = Request.Form[coefField.Name];
                    }

                    if (!string.IsNullOrEmpty(value))
                    {
                        var studentFormSubmissionData = new StudentFormSubmissionData
                        {
                            StudentFormSubmissionScoreId = submission.Id,
                            FormItemCoefficientDefinitionId = coefField.Id,
                            InputType = coefField.inputType,
                            Value = value
                        };
                        await _submissionDataRepository.AddStudentFormSubmissionValueAsync(studentFormSubmissionData, cancellationToken);
                    }
                }
            }

            return Json(new
            {
                success = true,
                message = "فرم با موفقیت ثبت شد",
                redirectUrl = Url.Action("LoadForm", "StudentHome", new { area = "Student", formId = model.FormId })
            });
        }
        public async Task<IActionResult> SubmissionDataList(CancellationToken cancellationToken)
        {
            try
            {
                var studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var forms = await _submissionScoreRepository.GetByStudentAsync(studentId, cancellationToken);

               

                foreach (var form in forms)
                {
                    // 🧩 فیلتر رکوردهای معتبر
                    form.StudentFormSubmissionDatas = form.StudentFormSubmissionDatas
                        .Where(d => d.IsActive && !d.IsDeleted)
                        .ToList();

                    // 🧮 گرفتن آیدی‌هایی که در Value ذخیره شده‌اند
                    var valueIds = form.StudentFormSubmissionDatas
                        .Select(d => int.TryParse(d.Value, out var idValue) ? idValue : (int?)null)
                        .Where(idValue => idValue.HasValue)
                        .Select(idValue => idValue.Value)
                        .ToList();

                    if (valueIds.Any())
                    {
                        foreach (var data in form.StudentFormSubmissionDatas)
                        {
                            if (data.InputType == InputType.Dropdown)
                            {
                                if (int.TryParse(data.Value, out var coefId))
                                {
                                    // دریافت از Repository
                                    var coef = await _coefficientRepository.GetByIdAsync(cancellationToken, coefId);

                                    if (coef != null)
                                    {
                                        // جایگزینی مقدار اصلی با مقدار توضیحی از جدول Coefficient
                                        //data.Value = $"{coef.Name} ({coef.Value})";
                                        data.Value = $"{coef.Name}";
                                    }
                                }
                            }
                        }
                    }
                }



                var result = forms.Select(f => new AllSubmissionDataViewModel
                {
                    SubmissionId = f.Id,
                    FormItemId = f.FormItemId,
                    FormTitle = f.FormItem.Title,
                    CreateDatePersian = DateConvertor.ToShamsiDateTime(f.CreatedDate), // فرض بر این که این پراپرتی توی entity هست یا خودت تبدیلش می‌کنی
                    SubmissionValues = f.StudentFormSubmissionDatas.Select(d =>
                    {
                        var label = d.FormItemDefinition?.Label
                                    ?? d.FormItemCoefficientDefinition?.Label
                                    ?? "بدون عنوان";
                        return $"{label}: {d.Value}";
                    }).ToList()
                });



                return PartialView("SubmissionDataList", result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "خطا در سرور");
            }
        }
        [HttpGet]
        public async Task<IActionResult> SubmissionDataDetail(int id, CancellationToken cancellationToken)
        {
            try
            {
                var studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var datadetail = await _submissionScoreRepository.GetFormSubmissionDataByStudentAndSubmissionScoresIDAsync(studentId, id, cancellationToken);
                if (datadetail == null)
                {
                    return NotFound();
                }

                // 🧩 فیلتر رکوردهای معتبر
                datadetail.StudentFormSubmissionDatas = datadetail.StudentFormSubmissionDatas
                    .Where(d => d.IsActive && !d.IsDeleted)
                    .ToList();

                // 🧮 گرفتن آیدی‌هایی که در Value ذخیره شده‌اند
                var valueIds = datadetail.StudentFormSubmissionDatas
                    .Select(d => int.TryParse(d.Value, out var idValue) ? idValue : (int?)null)
                    .Where(idValue => idValue.HasValue)
                    .Select(idValue => idValue.Value)
                    .ToList();

                if (valueIds.Any())
                {
                    foreach (var data in datadetail.StudentFormSubmissionDatas)
                    {
                        if (data.InputType == InputType.Dropdown)
                        {
                            if (int.TryParse(data.Value, out var coefId))
                            {
                                // دریافت از Repository
                                var coef = await _coefficientRepository.GetByIdAsync(cancellationToken, coefId);

                                if (coef != null)
                                {
                                    // جایگزینی مقدار اصلی با مقدار توضیحی از جدول Coefficient
                                    //data.Value = $"{coef.Name} ({coef.Value})";
                                    data.Value = $"{coef.Name}";
                                }
                            }
                        }
                    }
                }

                var model = _mapper.Map<SubmissionDataDetailViewModel>(datadetail);

                return PartialView("SubmissionDataDetail", model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "خطا در سرور");
            }
        }

        public async Task<IActionResult> DeleteSubmissionData(int submissionId, CancellationToken cancellationToken)
        {
            try
            {
                if (submissionId == 0)
                {
                    return Json(new { isSuccess = false, message = "فرمی جهت حذف ارسال نشده است یا خطایی رخ داده." });
                }

                await _submissionScoreRepository.DeleteStudentFormSubmissionAsync(submissionId, cancellationToken);
                await _submissionDataRepository.DeleteStudentFormSubmissionValueAsync(submissionId, cancellationToken);

                return Json(new { isSuccess = true, message = "فرم با موفقیت حذف شد." });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = "خطا در سرور" });
            }
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DownloadSubmissionFile(int submissionDataId, CancellationToken cancellationToken)
        {
            // دریافت داده‌ی دقیق submission-data (نام فایل در data.Value)
            var data = await _submissionDataRepository.GetSubmissionDataByIdAsync(submissionDataId, cancellationToken);
            if (data == null) return NotFound();

            // بررسی دسترسی به فایل (اجباری)
            //var student = await _studentRepository.GetStudentByIdAsync(studentId, cancellationToken);
            // مثال: اگر بخواهید مطمئن شوید فایل متعلق به کاربر است:
            // var parent = data.StudentFormSubmissionScore; // اگر Include کرده‌اید
            // if (parent == null || parent.StudentId != currentUserStudentId) return Forbid();

            // امن‌سازی مقدار Value: اغلب data.Value فقط نام فایل است، نه مسیر کامل
            var fileName = Path.GetFileName(data.Value ?? string.Empty);
            if (string.IsNullOrEmpty(fileName)) return NotFound();

            // مسیر فیزیکی امن داخل wwwroot/uploads/{national}/{fileName}
            var studentNationalCode = User.FindFirst("NationalCode")?.Value; // فرض بر این است که کد ملی تو Claims هست

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", studentNationalCode ?? string.Empty);
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
