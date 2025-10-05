using Microsoft.AspNetCore.Mvc;
using SanjeshP.Mosahebe.Data.Contracts.StudentForm;
using SanjeshP.Mosahebe.Entities.StudentForm;
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
        private readonly IFormItemRepository _formItemRepository;
        private readonly IFormItemDefinitionRepository _formItemDefinitionRepository;
        private readonly IFormItemCoefficientDefinitionRepository _coefficientDefinitionRepository;
        private readonly IFormItemCoefficientRepository _coefficientRepository;
        private readonly IStudentFormSubmissionRepository _submissionRepository;
        private readonly IStudentFormSubmissionValueRepository _submissionValueRepository;

        public StudentHomeController(
            IFormItemRepository formItemRepository,
            IFormItemDefinitionRepository formItemDefinitionRepository,
            IFormItemCoefficientDefinitionRepository coefficientDefinitionRepository,
            IFormItemCoefficientRepository coefficientRepository,
            IStudentFormSubmissionRepository submissionRepository,
            IStudentFormSubmissionValueRepository submissionValueRepository)
        {
            _formItemRepository = formItemRepository;
            _formItemDefinitionRepository = formItemDefinitionRepository;
            _coefficientDefinitionRepository = coefficientDefinitionRepository;
            _coefficientRepository = coefficientRepository;
            _submissionRepository = submissionRepository;
            _submissionValueRepository = submissionValueRepository;
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
                Fields = await _formItemDefinitionRepository.GetByFormItemIdAsync(form.Id, cancellationToken)
            };

            if (form.FormItemCoefficientId > 0)
            {
                viewModel.CoefficientFields = await _coefficientDefinitionRepository.GetByCoefficientIdAsync(form.FormItemCoefficientId, cancellationToken);
                var coef = await _coefficientRepository.GetByCoefficientIdAsync(form.FormItemCoefficientId, cancellationToken);
                viewModel.Coefficients = coef.ToList();
            }

            return PartialView("_SubmitFormPartial", viewModel);
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
            var previousSubmission = await _submissionRepository
                .GetByStudentAndFormAsync(studentId, model.FormId, cancellationToken);
            if (previousSubmission != null)
            {
                previousSubmission.IsDeleted = true;
                await _submissionRepository.UpdateAsync(previousSubmission, cancellationToken);
            }

            // ایجاد رکورد جدید برای Submission
            var submission = new StudentFormSubmissionScore
            {
                FormItemId = model.FormId,
                StudentId = studentId,
                CreatedDate = DateTime.UtcNow
            };
            await _submissionRepository.AddStudentFormSubmissionAsync(submission, cancellationToken);

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
                    await _submissionValueRepository.AddStudentFormSubmissionValueAsync(studentFormSubmissionData, cancellationToken);
                }
            }

            // ذخیره فیلدهای Coefficient
            var formItem = await _formItemRepository.GetByFormItemIdAsync(model.FormId, cancellationToken);
            if (formItem.FormItemCoefficientId > 0)
            {
                var coefFields = await _coefficientDefinitionRepository.GetByCoefficientIdAsync(formItem.FormItemCoefficientId, cancellationToken);
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
                        await _submissionValueRepository.AddStudentFormSubmissionValueAsync(studentFormSubmissionData, cancellationToken);
                    }
                }
            }

            return Json(new { success = true, message = "فرم با موفقیت ثبت شد" });
        }
    }
}
