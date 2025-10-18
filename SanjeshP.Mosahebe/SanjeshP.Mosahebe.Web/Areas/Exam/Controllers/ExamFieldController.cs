using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SanjeshP.Mosahebe.Data.Contracts.Exam;
using SanjeshP.Mosahebe.Entities.Exam;
using SanjeshP.Mosahebe.WebFramework.Api;
using System.Threading.Tasks;
using System.Threading;
using System;
using SanjeshP.Mosahebe.Web.Areas.Exam.ViewModels.ExamField;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace SanjeshP.Mosahebe.Web.Areas.Exam.Controllers
{
    [Area("Exam")]
    public class ExamFieldController : Controller
    {
        private readonly IExamFieldRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<ExamFieldController> _logger;

        public ExamFieldController(IExamFieldRepository repository, IMapper mapper, ILogger<ExamFieldController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index(int examId)
        {
            if (examId == 0)
                return BadRequest("آزمون معتبر نیست");

            ViewBag.ExamId = examId;
            return PartialView("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ExamFieldList(int examId, CancellationToken cancellationToken)
        {
            try
            {
                var fields = await _repository.GetAllFieldsByExamIdAsync(examId, cancellationToken);
                var model = fields.Select(f => _mapper.Map<ExamFieldViewModel>(f)).ToList();

                ViewBag.ExamId = examId;
                return PartialView("ExamFieldList", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت لیست رشته‌ها");
                return StatusCode(500, "خطا در سرور");
            }
        }

        #region Create Field
        [HttpGet]
        public IActionResult CreateExamField(int examId)
        {
            var model = new ExamFieldCreateViewModel
            {
                ExamId = examId
            };
            return PartialView(model);
        }

        [HttpPost]
        public async Task<ApiResult> CreateExamField(ExamFieldCreateViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            try
            {
                var entity = new ExamField
                {
                    Title = model.Title,
                    StandardCode = model.StandardCode,
                    ExamId = model.ExamId,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                await _repository.AddFieldAsync(entity, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ایجاد رشته آزمون");
                return (ApiResult)StatusCode(500, "خطا در سرور");
            }
        }
        #endregion

        #region Edit Field
        [HttpGet]
        public async Task<IActionResult> EditExamField(int id, CancellationToken cancellationToken)
        {
            var field = await _repository.GetByFieldIdAsync(id, cancellationToken);
            if (field == null)
                return NotFound();

            var model = _mapper.Map<ExamFieldEditViewModel>(field);
            return PartialView("EditExamField", model);
        }

        [HttpPost]
        public async Task<ApiResult> EditExamField(ExamFieldEditViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var existing = await _repository.GetByFieldIdAsync(model.Id, cancellationToken);
            if (existing == null)
                return NotFound();

            existing.Title = model.Title;
            existing.StandardCode = model.StandardCode;
            existing.UpdatedDate = DateTime.Now;

            try
            {
                await _repository.UpdateFieldAsync(existing, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ویرایش رشته آزمون");
                return (ApiResult)StatusCode(500, "خطا در سرور");
            }
        }
        #endregion

        #region Delete Field
        [HttpPost]
        public async Task<IActionResult> DeleteExamField(int fieldId, CancellationToken cancellationToken)
        {
            try
            {
                if (fieldId == 0)
                    return Json(new { isSuccess = false, message = "رشته معتبر نیست" });

                await _repository.DeleteFieldAsync(fieldId, cancellationToken);
                return Json(new { isSuccess = true, message = "رشته با موفقیت حذف شد" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در حذف رشته آزمون");
                return Json(new { isSuccess = false, message = "خطا در سرور" });
            }
        }
        #endregion
    }
}
