using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Convertor;
using SanjeshP.Mosahebe.Data.Contracts.File;
using SanjeshP.Mosahebe.Data.Contracts.StudentForm;
using SanjeshP.Mosahebe.Data.Contracts.Students;
using SanjeshP.Mosahebe.Entities.File;
using SanjeshP.Mosahebe.Entities.StudentForm;
using SanjeshP.Mosahebe.Web.Areas.Exam.ViewModels.Exam;
using SanjeshP.Mosahebe.Web.Areas.Student.ViewModels;
using System;
using System.Collections.Generic;
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
        private readonly IUploadedFileRepository _uploadedFileRepository;
        private readonly SiteSettings _siteSettings;
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
            IWebHostEnvironment env,
            IOptions<SiteSettings> siteSettings,
            IUploadedFileRepository uploadedFileRepository)
        {
            _mapper = mapper;

            _formItemRepository = formItemRepository;
            _formItemDefinitionRepository = formItemDefinitionRepository;
            _coefficientDefinitionRepository = coefficientDefinitionRepository;
            _formItemCoefficientRepository = formItemCoefficientRepository;
            _submissionScoreRepository = submissionScoreRepository;
            _submissionDataRepository = submissionDataRepository;
            _coefficientRepository = coefficientRepository;
            _env = env;
            _uploadedFileRepository = uploadedFileRepository;
            _siteSettings = siteSettings.Value;
        }

        // GET: /Student/StudentHome
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var forms = await _formItemRepository.GetFormItemsWithIncludesAsync(cancellationToken);
            return View(forms.OrderBy(f => f.FormItemCategory.Order));
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
                FormDescription = form.Description,
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
                viewModel.CoefficientList = coefficients.OrderBy(c => c.Name).ThenBy(c => c.RangeStart).ToList();
            }

            return View("LoadForm", viewModel);
        }

        // POST: /Student/StudentHome/SubmitForm
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> SubmitForm(FormSubmissionViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest("اطلاعات فرم ناقص است");

            var studentIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(studentIdClaim) || !int.TryParse(studentIdClaim, out var studentId))
                return BadRequest("کاربر نامعتبر است");

            var studentNationalCode = User.FindFirst("NationalCode")?.Value;
            if (string.IsNullOrEmpty(studentNationalCode))
                return BadRequest("کد ملی داوطلب نامعتبر است.");

            var fileSettings = _siteSettings.FileSettings ?? new FileSettings();
            var allowedExt = new HashSet<string>(fileSettings.AllowedUploadExtensions ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase);
            var maxFileSizeBytes = (long)fileSettings.MaxUploadFileSizeInMB * 1024 * 1024;

            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", studentNationalCode);
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            var errors = new List<string>();

            // نگهداری فایل‌های آماده شده: Key نام فیلد در فرم (field_{id} یا coef_{id}), فایل، storedName، و آیدی definition مربوط
            var preparedFiles = new List<(string Key, IFormFile File, string StoredFileName, int? FormItemDefinitionId, int? FormItemCoefficientDefinitionId)>();

            // لیست داده‌هایی که باید ذخیره شوند (شامل فایلها به عنوان مقدار StoredFileName)
            var preparedData = new List<StudentFormSubmissionData>();

            // 1) بررسی و آماده‌سازی فیلدهای Definition
            var fields = await _formItemDefinitionRepository.GetByFormItemIdAsync(model.FormId, cancellationToken);

            var formItem = await _formItemRepository.GetByFormItemIdAsync(model.FormId, cancellationToken);
            foreach (var field in fields)
            {
                string value = null;
                var key = $"field_{field.Id}";

                var MaxCountCategory = formItem.FormItemCategory?.MaxCount;
                var MaxCountFormItem = formItem.MaxCount;

                var FormItemCount = await _submissionScoreRepository.GetByStudentAndFormsAsync(studentId, field.FormItemId, cancellationToken);

                if (field.inputType == InputType.File)
                {
                    var ddd = FormItemCount.Where(f => f.FormItem.FormItemCategory.Id == formItem.FormItemCategory.Id).Count();
                    var ggg = FormItemCount.Where(f => f.FormItemId == field.FormItemId).Count();
                    var file = Request.Form.Files[key];
                    if (FormItemCount.Where(f => f.FormItem.FormItemCategory.Id == formItem.FormItemCategory.Id).Count() >= MaxCountCategory && MaxCountCategory != 0)
                    {
                        errors.Add($"شما حداکثر تعداد فرم برای این دسته بندی را قبلا ثبت کرده اید.");
                        continue;
                    }
                    else if (FormItemCount.Where(f => f.FormItemId == field.FormItemId).Count() >= MaxCountFormItem && MaxCountFormItem != 0)
                    {
                        errors.Add($"شما حداکثر تعداد را برای این فرم قبلا ثبت کرده اید.");
                        continue;
                    }
                    else if (file != null && file.Length > 0)
                    {
                        var UploadedFiles = await _uploadedFileRepository.GetByStudentIdAsync(studentId, cancellationToken);
                        var exist = UploadedFiles.Any(ss => ss.OriginalFileName == file.FileName && ss.Size == file.Length);
                        if (exist == true)
                        {
                            errors.Add($"قبلا فایلی با این نام و حجم ثبت شده است.");
                            continue;
                        }
                        else if (file.Length > maxFileSizeBytes)
                        {
                            errors.Add($"فایل فیلد '{field.Label}' بزرگتر از حد مجاز ({fileSettings.MaxUploadFileSizeInMB} MB) است");
                            continue;
                        }

                        var originalFileName = Path.GetFileName(file.FileName);
                        var ext = Path.GetExtension(originalFileName).ToLowerInvariant();
                        if (!allowedExt.Contains(ext))
                        {
                            errors.Add($"فرمت فایل فیلد '{field.Label}' مجاز نیست");
                            continue;
                        }

                        var storedFileName = $"{Guid.NewGuid():N}{ext}";
                        preparedFiles.Add((Key: key, File: file, StoredFileName: storedFileName, FormItemDefinitionId: field.Id, FormItemCoefficientDefinitionId: null));
                        value = storedFileName;
                    }
                    else
                    {
                        if (field.IsRequired)
                            errors.Add($"فیلد '{field.Label}' اجباری است");
                    }
                }
                else
                {
                    value = Request.Form[key];
                    if (field.IsRequired && string.IsNullOrWhiteSpace(value))
                    {
                        errors.Add($"فیلد '{field.Label}' اجباری است");
                    }
                }

                if (!string.IsNullOrEmpty(value))
                {
                    preparedData.Add(new StudentFormSubmissionData
                    {
                        FormItemDefinitionId = field.Id,
                        InputType = field.inputType,
                        Value = value
                    });
                }
            }

            // 2) بررسی و آماده‌سازی فیلدهای Coefficient

            if (formItem?.FormItemCoefficientId > 0)
            {
                var formItemCoefficient = await _formItemCoefficientRepository.GetByIdAsync(formItem.FormItemCoefficientId, cancellationToken);
                var coefFields = await _coefficientDefinitionRepository.GetByCoefficientIdAsync(formItemCoefficient.CoefficientId, cancellationToken);

                foreach (var coefField in coefFields)
                {
                    string value = null;
                    var key = $"coef_{coefField.Id}";

                    if (coefField.inputType == InputType.File)
                    {
                        var file = Request.Form.Files[key];
                        if (file != null && file.Length > 0)
                        {
                            if (file.Length > maxFileSizeBytes)
                            {
                                errors.Add($"فایل فیلد '{coefField.Label}' بزرگتر از حد مجاز ({fileSettings.MaxUploadFileSizeInMB} MB) است");
                                continue;
                            }

                            var originalFileName = Path.GetFileName(file.FileName);
                            var ext = Path.GetExtension(originalFileName).ToLowerInvariant();
                            if (!allowedExt.Contains(ext))
                            {
                                errors.Add($"فرمت فایل فیلد '{coefField.Label}' مجاز نیست");
                                continue;
                            }

                            var storedFileName = $"{Guid.NewGuid():N}{ext}";
                            preparedFiles.Add((Key: key, File: file, StoredFileName: storedFileName, FormItemDefinitionId: null, FormItemCoefficientDefinitionId: coefField.Id));
                            value = storedFileName;
                        }
                        else
                        {
                            if (coefField.IsRequired)
                                errors.Add($"فیلد '{coefField.Label}' اجباری است");
                        }
                    }
                    else
                    {
                        value = Request.Form[key];
                        if (coefField.IsRequired && string.IsNullOrWhiteSpace(value))
                        {
                            errors.Add($"فیلد '{coefField.Label}' اجباری است");
                        }
                    }

                    if (!string.IsNullOrEmpty(value))
                    {
                        preparedData.Add(new StudentFormSubmissionData
                        {
                            FormItemCoefficientDefinitionId = coefField.Id,
                            InputType = coefField.inputType,
                            Value = value
                        });
                    }
                }
            }

            // اگر خطا داریم، برگردانیم و هیچ چیزی ذخیره نشود
            if (errors.Any())
                return BadRequest(new { success = false, errors });

            // همه چی اوکی است؛ رکورد Submission را ایجاد می‌کنیم
            var submission = new StudentFormSubmissionScore
            {
                FormItemId = model.FormId,
                StudentId = studentId,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,

            };
            await _submissionScoreRepository.AddStudentFormSubmissionAsync(submission, cancellationToken);

            // ذخیره studentFormSubmissionDataها یکی‌یکی؛ اگر داده مربوط به فایل است، بعد از ذخیره data.Id فایل را روی دیسک بنویس و UploadedFile را بساز
            // توجه: فرض بر این است که AddStudentFormSubmissionValueAsync پس از ذخیره، entity.Id را مقداردهی می‌کند (EF Core رفتار پیش‌فرض بعد از SaveChanges)
            foreach (var data in preparedData)
            {
                data.StudentFormSubmissionScoreId = submission.Id;
                await _submissionDataRepository.AddStudentFormSubmissionValueAsync(data, cancellationToken);

                // اگر این data مربوط به فایلی است، پیدا کردن فایل متناظر در preparedFiles بر اساس storedFileName و definition id
                if (data.InputType == InputType.File)
                {
                    // پیدا کردن preparedFile متناظر
                    var match = preparedFiles.FirstOrDefault(p =>
                        (p.FormItemDefinitionId.HasValue && p.FormItemDefinitionId.Value == data.FormItemDefinitionId) ||
                        (p.FormItemCoefficientDefinitionId.HasValue && p.FormItemCoefficientDefinitionId.Value == data.FormItemCoefficientDefinitionId)
                    );

                    if (match.File != null)
                    {
                        var savePath = Path.Combine(basePath, match.StoredFileName);
                        using (var stream = new FileStream(savePath, FileMode.Create))
                        {
                            await match.File.CopyToAsync(stream, cancellationToken);
                        }

                        var uploaded = new UploadedFile
                        {
                            StudentId = studentId,
                            StudentFormSubmissionScoreId = submission.Id,
                            StudentFormSubmissionDataId = data.Id,
                            StoredFileName = match.StoredFileName,
                            OriginalFileName = Path.GetFileName(match.File.FileName),
                            ContentType = match.File.ContentType,
                            Size = match.File.Length,
                            UploadedAt = DateTime.Now
                        };
                        await _uploadedFileRepository.AddUploadedFileAsync(uploaded, cancellationToken);

                        // از لیست آماده حذف کن تا در صورت موارد مشابه تداخل نشود
                        preparedFiles.Remove(match);
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
                        .Where(d => d.IsActive && !d.IsDeleted && d.InputType != InputType.File)
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
                    Order = f.FormItem.FormItemCategory.Order,
                    CreateDatePersian = DateConvertor.ToShamsiDateTime(f.CreatedDate), // فرض بر این که این پراپرتی توی entity هست یا خودت تبدیلش می‌کنی
                    SubmissionValues = f.StudentFormSubmissionDatas.Select(d =>
                    {
                        var label = d.FormItemDefinition?.Label
                                    ?? d.FormItemCoefficientDefinition?.Label
                                    ?? "بدون عنوان";
                        return $"{label}: {d.Value}";
                    }).ToList()
                });



                return PartialView("SubmissionDataList", result.OrderBy(f => f.Order));
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
