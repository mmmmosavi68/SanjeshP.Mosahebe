using Microsoft.AspNetCore.Mvc;
using SanjeshP.Mosahebe.Data.Contracts.StudentForm;
using SanjeshP.Mosahebe.Entities.StudentForm;
using SanjeshP.Mosahebe.Web.Areas.Student.ViewModels;
using System;
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

            var previousSubmission = await _submissionRepository
                .GetByStudentAndFormAsync(studentId, model.FormId, cancellationToken);

            if (previousSubmission != null)
            {
                previousSubmission.IsDeleted = true;
                await _submissionRepository.UpdateAsync(previousSubmission, cancellationToken);
            }

            var submission = new StudentFormSubmissionScore
            {
                FormItemId = model.FormId,
                StudentId = studentId,
                CreatedDate = DateTime.UtcNow
            };

            await _submissionRepository.AddStudentFormSubmissionAsync(submission, cancellationToken);

            // ✅ دریافت فیلدهای Definition از دیتابیس (نه از model)
            var fields = await _formItemDefinitionRepository.GetByFormItemIdAsync(model.FormId, cancellationToken);

            foreach (var field in fields)
            {
                var value = Request.Form[field.Name];
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

            // ✅ دریافت فیلدهای Coefficient از دیتابیس
            var formItem = await  _formItemRepository.GetByFormItemIdAsync(model.FormId, cancellationToken);
            var formItemCoefficient = await _coefficientDefinitionRepository.GetByCoefficientIdAsync(formItem.FormItemCoefficientId, cancellationToken);

                foreach (var coefField in formItemCoefficient)
                {
                    var value = Request.Form[coefField.Name];
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
            

            return Json(new { success = true, message = "فرم با موفقیت ثبت شد" });
        }


        //public async Task<IActionResult> SubmitForm(FormSubmissionViewModel model, CancellationToken cancellationToken)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest("اطلاعات فرم ناقص است");

        //    var submission = new StudentFormSubmissionScore
        //    {
        //        FormItemId = model.FormId
        //    };

        //    await _submissionRepository.AddStudentFormSubmissionAsync(submission, cancellationToken);

        //    // ذخیره مقادیر فیلدهای Definition
        //    foreach (var field in model.Fields)
        //    {
        //        var value = Request.Form[field.Name].ToString();
        //        if (!string.IsNullOrEmpty(value))
        //        {
        //            await _submissionValueRepository.AddStudentFormSubmissionValueAsync(new StudentFormSubmissionData
        //            {
        //                StudentFormSubmissionScoreId = submission.Id,
        //                FormItemDefinitionId = field.Id,
        //                Value = value
        //            }, cancellationToken);
        //        }
        //    }

        //    // ذخیره مقادیر فیلدهای Coefficient
        //    foreach (var coefField in model.CoefficientFields)
        //    {
        //        var value = Request.Form[coefField.Name].ToString();
        //        if (!string.IsNullOrEmpty(value))
        //        {
        //            await _submissionValueRepository.AddStudentFormSubmissionValueAsync(new StudentFormSubmissionData
        //            {
        //                StudentFormSubmissionScoreId = submission.Id,
        //                FormItemCoefficientDefinitionId = coefField.Id,
        //                Value = value
        //            }, cancellationToken);
        //        }
        //    }

        //    return Json(new { success = true, message = "فرم با موفقیت ثبت شد" });
        //}
    }
}
