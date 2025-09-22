using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Data.Contracts.Students;
using SanjeshP.Mosahebe.Entities.Student;
using SanjeshP.Mosahebe.Web.Areas.Student.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Web.Areas.Student.Controllers
{
    [Area("Student")]
    public class StudentHomeController : Controller
    {
        private readonly IFieldDefinitionRepository _fieldModelRepository;

        public StudentHomeController(IFieldDefinitionRepository fieldModelRepository)
        {
            _fieldModelRepository = fieldModelRepository;
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}


        // صفحه لیست فرم‌ها (انتخاب فرم)
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var formIds = await _fieldModelRepository.GetFormIds(cancellationToken);
            return View(formIds);
        }
        //وقتی کاربر فرم جدید رو انتخاب می‌کنه و روی افزودن کلیک می‌کنه، این اکشن با Ajax فراخوانی می‌شه
        [HttpGet]
        public async Task<IActionResult> LoadForm(int formId, CancellationToken cancellationToken)
        {
            var fields = await _fieldModelRepository.GetFields(formId, cancellationToken);
            ViewBag.Fields = fields;
            ViewBag.Selected = formId;
            return PartialView("SubmitForm");
        }

        //وقتی کاربر فرم رو submit می‌کنه، اطلاعات فرم به این اکشن می‌ره
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitForm(int formId, CancellationToken cancellationToken)
        {
            var fields = await _fieldModelRepository.GetFields(formId, cancellationToken);

            foreach (var field in fields)
            {
                string value = "";

                if (field.Type == FieldType.File)
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

                await _fieldModelRepository.AddFormSubmissionAsync(new FormSubmission
                {
                    FormId = formId,
                    FieldDefinitionId = field.Id,
                    Value = value
                }, cancellationToken);
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> GetFormSubmissions(int formId, CancellationToken cancellationToken)
        {
            var fields = await _fieldModelRepository.GetFields(formId, cancellationToken);
            var submissions = _fieldModelRepository.GetAllSubmissions() // این متد جدید باید در ریپازیتوری اضافه شود
                .Where(s => s.FormId == formId)
                .OrderByDescending(s => s.SubmittedAt)
                .Select(s => new FormSubmissionViewModel
                {
                    FieldLabel = fields.FirstOrDefault(f => f.Id == s.FieldDefinitionId)?.Label,
                    Value = s.Value,
                    SubmittedAt = s.SubmittedAt
                })
                .ToList();

            return PartialView("_FormSubmissionsTable", submissions);
        }
}
}
