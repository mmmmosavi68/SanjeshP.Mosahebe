using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanjeshP.Mosahebe.Data.Contracts.StudentForm;
using SanjeshP.Mosahebe.Entities.StudentForm;
using SanjeshP.Mosahebe.Presentation.ViewModels;
using System.Security.Claims;
using System;
using System.Threading;
using System.Threading.Tasks;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using Microsoft.Extensions.Logging;

namespace SanjeshP.Mosahebe.Web.Areas.StudentForm.Controllers
{
    [AllowAnonymous]
    [Area("StudentForm")]
    public class FormItemDefinitionController : Controller
    {
        private readonly IFormItemDefinitionRepository _repository;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly ILogger<FormItemDefinitionController> _logger;

        public FormItemDefinitionController(IFormItemDefinitionRepository repository, IUserTokenRepository userTokenRepository, ILogger<FormItemDefinitionController> logger)
        {
            _repository = repository;
            _userTokenRepository = userTokenRepository;
            this._logger = logger;
        }

        public async Task<IActionResult> Index(int formItemId, CancellationToken cancellationToken)
        {
            ViewBag.FormItemId = formItemId;
            return PartialView("Index");
        }
        public async Task<IActionResult> FormItemDefinitionList(int formItemId, CancellationToken cancellationToken)
        {
            var defs = await _repository.GetByFormItemIdAsync(formItemId, cancellationToken);
            ViewBag.FormItemId = formItemId;
            return PartialView("FormItemDefinitionList", defs);
        }

        [HttpGet]
        public IActionResult Create(int formItemId)
        {
            var model = new FormItemDefinitionCreateViewModel { FormItemId = formItemId };
            return PartialView("Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FormItemDefinitionCreateViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return PartialView("Create", model);

            var currentUserToken = User.FindFirstValue("Token");
            var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(currentUserToken), cancellationToken);

            var entity = new FormItemDefinition
            {
                FormItemId = model.FormItemId,
                Name = model.Name,
                Label = model.Label,
                CreatedBy = token.UserId,
                inputType = model.InputType,
                // اینجا مقدار متن قابل نمایش رو خودمون میسازیم
                InputTypeText = model.InputType.ToString(),
                IsRequired = model.IsRequired,
                Description = model.Description,
                ValidationPattern = model.ValidationPattern,
                ErrorMessage = model.ErrorMessage,
                DropdownOptions = model.DropdownOptions,
                HostIp = Request.HttpContext.Connection.RemoteIpAddress?.ToString()

            };

            await _repository.AddDefinitionAsync(entity, cancellationToken);
            return Ok();
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
                InputTypeText = entity.inputType.ToString(), // ← اینجا تغییر دادیم
                IsRequired = entity.IsRequired,
                Description = entity.Description,
                ValidationPattern = entity.ValidationPattern,
                ErrorMessage = entity.ErrorMessage,
                DropdownOptions = entity.DropdownOptions
            };

            return PartialView("Edit", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FormItemDefinitionCreateViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return PartialView("Edit", model);

            var entity = await _repository.GetByDefinitionIdAsync(model.Id, cancellationToken);
            if (entity == null) return NotFound();

            entity.Name = model.Name;
            entity.Label = model.Label;
            entity.inputType = model.InputType;
            entity.InputTypeText = model.InputType.ToString();

            entity.IsRequired = model.IsRequired;
            entity.Description = model.Description;
            entity.ValidationPattern = model.ValidationPattern;
            entity.ErrorMessage = model.ErrorMessage;
            entity.DropdownOptions = model.DropdownOptions;
            entity.UpdatedDate = DateTime.Now;
            entity.HostIp = Request.HttpContext.Connection.RemoteIpAddress?.ToString();

            await _repository.UpdateDefinitionAsync(entity, cancellationToken);
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            try
            {
                if (id == 0)
                {
                    return Json(new { isSuccess = false, message = "آیتم جهت حذف ارسال نشده است یا خطایی رخ داده." });
                }

                await _repository.DeleteDefinitionAsync(id, cancellationToken);

                return Json(new { isSuccess = true, message = "آیتم با موفقیت حذف شد." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در حذف آیتم فرم");
                return Json(new { isSuccess = false, message = "خطا در سرور" });
            }
        }

    }
}
