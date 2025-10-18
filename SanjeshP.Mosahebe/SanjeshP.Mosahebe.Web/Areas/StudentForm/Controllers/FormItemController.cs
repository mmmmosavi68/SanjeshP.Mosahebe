using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SanjeshP.Mosahebe.Data.Contracts.StudentForm;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using SanjeshP.Mosahebe.Entities.StudentForm;
using SanjeshP.Mosahebe.Presentation.ViewModels;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Presentation.Controllers
{
    [Area("StudentForm")]
    public class FormItemController : Controller
    {
        private readonly ILogger<FormItemController> _logger;
        private readonly IFormItemRepository _formItemRepository;
        private readonly IFormItemCoefficientRepository _coefficientRepository;
        private readonly IFormItemCategoryRepository _categoryRepository;
        private readonly IUserTokenRepository _userTokenRepository;

        public FormItemController(
            ILogger<FormItemController> logger,
            IFormItemRepository formItemRepository,
            IFormItemCoefficientRepository coefficientRepository,
            IFormItemCategoryRepository categoryRepository,
            IUserTokenRepository userTokenRepository)
        {
            this._logger = logger;
            _formItemRepository = formItemRepository;
            _coefficientRepository = coefficientRepository;
            _categoryRepository = categoryRepository;
            this._userTokenRepository = userTokenRepository;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            return View();
        }
        public async Task<IActionResult> FormItemList(CancellationToken cancellationToken)
        {
            //همه FormItemها را می‌گیرد و به ViewModel تبدیل می‌کند
            var entities = await _formItemRepository.GetFormItemsWithIncludesAsync(cancellationToken);

            var model = entities.Select(f => new FormItemViewModel
            {
                Id = f.Id,
                Title = f.Title,
                Description = f.Description,
                OneMaxScore = f.OneMaxScore,
                AllMaxScore = f.AllMaxScore,
                IsActive = f.IsActive,
                CoefficientTitle = f.FormItemCoefficient?.Title,
                CategoryTitle = f.FormItemCategory?.Title
            });

            return PartialView("FormItemList", model);
        }
        // صفحه ایجاد
        //فقط فرم خالی را نمایش می‌دهد و Dropdown‌ ها را آماده می‌کند
        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var model = new FormItemViewModel
            {
                Coefficients = (await _coefficientRepository.GetAllAsync(cancellationToken))
                                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Title }),
                Categories = (await _categoryRepository.GetAllAsync(cancellationToken))
                                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Title })
            };

            return PartialView("Create",model);
        }

        //ثبت فرم جدید
        [HttpPost]
        public async Task<IActionResult> Create(FormItemViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(model);

            var currentUserToken = User.FindFirstValue("Token");
            var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(currentUserToken), cancellationToken);

            //اطلاعات از View گرفته می‌شود
            //تبدیل به Entity
            //ذخیره در دیتابیس
            var entity = new FormItem
            {
                Title = model.Title,
                Description = model.Description,
                OneMaxScore = model.OneMaxScore,
                AllMaxScore = model.AllMaxScore,
                IsActive = model.IsActive,
                CreatedBy = token.UserId,
                FormItemCoefficientId = model.FormItemCoefficientId,
                FormItemCategoryId = model.FormItemCategoryId,
                HostIp = Request.HttpContext.Connection.RemoteIpAddress?.ToString()

            };

            await _formItemRepository.AddFormItemAsync(entity, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        // نمایش فرم ویرایش
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var entity = await _formItemRepository.GetByFormItemIdAsync(id, cancellationToken);
            if (entity == null) return NotFound();

            var model = new FormItemViewModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                OneMaxScore = entity.OneMaxScore,
                AllMaxScore = entity.AllMaxScore,
                IsActive= entity.IsActive,
                FormItemCoefficientId = entity.FormItemCoefficientId,
                FormItemCategoryId = entity.FormItemCategoryId,
                Coefficients = (await _coefficientRepository.GetAllAsync(cancellationToken))
                                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Title }),
                Categories = (await _categoryRepository.GetAllAsync(cancellationToken))
                                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Title })
            };

            return PartialView("Edit",model);
        }

        //نمایش فرم ویرایش
        [HttpPost]
        public async Task<IActionResult> Edit(FormItemViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(model);

            var entity = await _formItemRepository.GetByFormItemIdAsync(model.Id, cancellationToken);
            if (entity == null) return NotFound();

            var currentUserToken = User.FindFirstValue("Token");
            var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(currentUserToken), cancellationToken);

            entity.Title = model.Title;
            entity.Description = model.Description;
            entity.OneMaxScore = model.OneMaxScore;
            entity.AllMaxScore = model.AllMaxScore;
            entity.IsActive=model.IsActive;
            entity.FormItemCoefficientId = model.FormItemCoefficientId;
            entity.FormItemCategoryId = model.FormItemCategoryId;
            entity.CreatedBy = token.UserId;
            entity.UpdatedDate = DateTime.Now;
            entity.HostIp = Request.HttpContext.Connection.RemoteIpAddress?.ToString();

            await _formItemRepository.UpdateFormItemAsync(entity, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        // حذف
        [HttpPost]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            try
            {
                if (id == 0)
                    return Json(new { isSuccess = false, message = "فرم جهت حذف ارسال نشده است یا خطایی رخ داده." });

                var formItem = await _formItemRepository.GetByFormItemIdAsync(id, cancellationToken);
                if (formItem == null)
                    return Json(new { isSuccess = false, message = "فرم یافت نشد." });

                formItem.IsDeleted = true; // اگر موجود است soft delete دارید
                await _formItemRepository.UpdateFormItemAsync(formItem, cancellationToken);

                return Json(new { isSuccess = true, message = "فرم با موفقیت حذف شد." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در حذف فرم");
                return Json(new { isSuccess = false, message = "خطا در سرور" });
            }
        }
    }
}
