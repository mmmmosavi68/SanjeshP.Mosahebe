using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanjeshP.Mosahebe.Data.Contracts.StudentForm;
using SanjeshP.Mosahebe.Entities.StudentForm;
using SanjeshP.Mosahebe.Presentation.ViewModels;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Web.Areas.StudentForm.Controllers
{
    [AllowAnonymous]
    [Area("StudentForm")]
    public class FormItemDefinitionController : Controller
    {
        private readonly IFormItemDefinitionRepository _repository;

        public FormItemDefinitionController(IFormItemDefinitionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index(int formItemId, CancellationToken cancellationToken)
        {
            var defs = await _repository.GetByFormItemIdAsync(formItemId, cancellationToken);
            ViewBag.FormItemId = formItemId;
            return View(defs);
        }

        [HttpGet]
        public IActionResult Create(int formItemId)
        {
            var model = new FormItemDefinitionCreateViewModel { FormItemId = formItemId };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FormItemDefinitionCreateViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(model);

            var entity = new FormItemDefinition
            {
                FormItemId = model.FormItemId,
                Name = model.Name,
                Label = model.Label,
                inputType = model.InputType,
                InputTypeText = model.InputTypeText,
                IsRequired = model.IsRequired,
                Description = model.Description,
                ValidationPattern = model.ValidationPattern,
                ErrorMessage = model.ErrorMessage
            };

            await _repository.AddDefinitionAsync(entity, cancellationToken);
            return RedirectToAction("Index", new { formItemId = model.FormItemId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByDefinitionIdAsync(id, cancellationToken);
            if (entity == null) return NotFound();

            var model = new FormItemDefinitionCreateViewModel
            {
                Id = entity.Id,
                FormItemId = entity.FormItemId,
                Name = entity.Name,
                Label = entity.Label,
                InputType = entity.inputType,
                InputTypeText = entity.InputTypeText,
                IsRequired = entity.IsRequired,
                Description = entity.Description,
                ValidationPattern = entity.ValidationPattern,
                ErrorMessage = entity.ErrorMessage
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FormItemDefinitionCreateViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(model);

            var entity = await _repository.GetByDefinitionIdAsync(model.Id, cancellationToken);
            if (entity == null) return NotFound();

            entity.Name = model.Name;
            entity.Label = model.Label;
            entity.inputType = model.InputType;
            entity.InputTypeText = model.InputTypeText;
            entity.IsRequired = model.IsRequired;
            entity.Description = model.Description;
            entity.ValidationPattern = model.ValidationPattern;
            entity.ErrorMessage = model.ErrorMessage;

            await _repository.UpdateDefinitionAsync(entity, cancellationToken);
            return RedirectToAction("Index", new { formItemId = entity.FormItemId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int Id, CancellationToken cancellationToken)
        {
            await _repository.DeleteDefinitionAsync(Id, cancellationToken);
            return RedirectToAction(nameof(Index));

            //return RedirectToAction("Index", new { formItemId });
        }
    }
}
