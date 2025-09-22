using Microsoft.AspNetCore.Mvc;
using SanjeshP.Mosahebe.Web.Areas.Student.ViewModels;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using SanjeshP.Mosahebe.Data.Contracts.StudentForm;
using SanjeshP.Mosahebe.Entities.StudentForm;

namespace SanjeshP.Mosahebe.Web.Areas.Student.Controllers
{
    [Area("Student")]
    public class StudentHomeController : Controller
    {
        
        private readonly IStudentFormRepository _studentFormRepository;
        private readonly IStudentFieldDefinitionRepository _studentFieldDefinitionRepository;
        private readonly IStudentFormSubmissionRepository _studentFormSubmissionRepository;
        private readonly IStudentFormSubmissionValueRepository _studentFormSubmissionValueRepository;

        public StudentHomeController(
            IStudentFormRepository studentFormRepository,
            IStudentFieldDefinitionRepository studentFieldDefinitionRepository,
            IStudentFormSubmissionRepository studentFormSubmissionRepository,
            IStudentFormSubmissionValueRepository studentFormSubmissionValueRepository)
        {
            _studentFormRepository = studentFormRepository;
            _studentFieldDefinitionRepository = studentFieldDefinitionRepository;
            _studentFormSubmissionRepository = studentFormSubmissionRepository;
            _studentFormSubmissionValueRepository = studentFormSubmissionValueRepository;
        }

        // صفحه لیست فرم‌ها
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var forms = await _studentFormRepository.GetAllStudentFormsNoTrackingAsync(cancellationToken);
            return View(forms);
        }

        // وقتی کاربر یک فرم رو انتخاب می‌کنه
        // فرم به صورت Ajax نمایش داده میشه و View ندارد
        [HttpGet]
        public async Task<IActionResult> LoadForm(int formId, CancellationToken cancellationToken)
        {
            var fields = await _studentFieldDefinitionRepository.GetByStudentFormIdAsync(formId, cancellationToken);
            ViewBag.Fields = fields;
            ViewBag.Selected = formId;
            return PartialView("SubmitForm");
        }

        // وقتی کاربر فرم رو ارسال می‌کنه
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitForm(int formId, CancellationToken cancellationToken)
        {
            var fields = await _studentFieldDefinitionRepository.GetByStudentFormIdAsync(formId, cancellationToken);

            // ثبت رکورد اصلی FormSubmission
            var submission = new StudentFormSubmission
            {
                FormId = formId,
                // می‌تونی StudentId یا CandidateId رو اینجا ست کنی
            };

            await _studentFormSubmissionRepository.AddStudentFormSubmissionAsync(submission, cancellationToken);

            // ذخیره مقادیر هر فیلد
            foreach (var field in fields)
            {
                string value = "";

                if (field.Type == Entities.StudentForm.FieldType.File)
                {
                    var file = Request.Form.Files[field.Name];
                    if (file != null && file.Length > 0)
                    {
                        var filePath = Path.Combine("uploads", file.FileName);
                        using (var stream = System.IO.File.Create(filePath))
                            await file.CopyToAsync(stream, cancellationToken);

                        value = file.FileName;
                    }
                }
                else if (field.Type == FieldType.Checkbox)
                {
                    value = Request.Form.ContainsKey(field.Name) ? "true" : "false";
                }
                else
                {
                    value = Request.Form[field.Name];
                }

                var submissionValue = new StudentFormSubmissionValue
                {
                    FormSubmissionId = submission.Id,
                    FieldDefinitionId = field.Id,
                    Type = field.Type,
                    Value = value
                };

                await _studentFormSubmissionValueRepository.AddStudentFormSubmissionValueAsync(submissionValue, cancellationToken);
            }

            return RedirectToAction("Index");
        }

        // گرفتن ارسال‌های قبلی یک فرم
        // فرم به صورت Ajax نمایش داده میشه و View ندارد
        [HttpGet]
        public async Task<IActionResult> GetFormSubmissions(int formId, CancellationToken cancellationToken)
        {
            var fields = await _studentFieldDefinitionRepository.GetByStudentFormIdAsync(formId, cancellationToken);
            var submissions = await _studentFormSubmissionRepository.GetByStudentFormIdAsync(formId, cancellationToken);

            var result = new List<FormSubmissionViewModel>();

            foreach (var submission in submissions.OrderByDescending(s => s.Id))
            {
                var values = await _studentFormSubmissionValueRepository.GetByStudentFormSubmissionValueIdAsync(submission.Id, cancellationToken);

                foreach (var value in values)
                {
                    result.Add(new FormSubmissionViewModel
                    {
                        FieldLabel = fields.FirstOrDefault(f => f.Id == value.FieldDefinitionId)?.Label,
                        Value = value.Value,
                        SubmittedAt = submission.CreatedDate // فرض کردم CreatedDate تو مدل‌هات هست
                    });
                }
            }

            return PartialView("_FormSubmissionsTable", result);
        }
    }
}
