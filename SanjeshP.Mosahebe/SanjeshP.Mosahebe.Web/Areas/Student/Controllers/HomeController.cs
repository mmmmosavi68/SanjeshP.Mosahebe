using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanjeshP.Mosahebe.Entities.Student;
using SanjeshP.Mosahebe.Data.Repositories.Students;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SanjeshP.Mosahebe.Data.Contracts.Students;

namespace SanjeshP.Mosahebe.Web.Areas.Student.Controllers
{
    [Area("Student")]
    public class HomeController : Controller
    {
        private readonly IFieldDefinitionRepository _fieldModelRepository;

        public HomeController(IFieldDefinitionRepository fieldModelRepository)
        {
            _fieldModelRepository = fieldModelRepository;
        }

        // صفحه لیست فرم‌ها (انتخاب فرم)
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var formIds = await _fieldModelRepository.GetFormIds(cancellationToken);
            return View(formIds);
        }
         
        // GET: نمایش فرم
        [HttpGet]
        public async Task<IActionResult> Form(int formId, CancellationToken cancellationToken)
        {
            var fields = await _fieldModelRepository.GetFields(formId, cancellationToken);

            ViewBag.Fields = fields;
            ViewBag.Selected = formId;

            return View();
        }

        // POST: دریافت و ذخیره داده‌های فرم
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitForm(int formId, CancellationToken cancellationToken)
        {
            var defs = await _fieldModelRepository.GetFields(formId, cancellationToken);

            var values = new Dictionary<string, string>();

            foreach (var f in defs)
            {
                string key = f.Name;
                string value = "";

                if (f.Type == FieldType.File)
                {
                    var file = Request.Form.Files[f.Name];
                    value = file != null && file.Length > 0
                        ? file.FileName
                        : "";
                }
                else if (f.Type == FieldType.Checkbox)
                {
                    value = Request.Form.ContainsKey(key) ? "true" : "false";
                }
                else
                {
                    value = Request.Form[key];
                }

                values[key] = value;
            }

            // ذخیره‌سازی

            ViewBag.Fields = defs;
            ViewBag.Selected = formId;
            ViewBag.Message = "اطلاعات فرم با موفقیت ذخیره شد.";

            return View("Form");
        }

    }
}
