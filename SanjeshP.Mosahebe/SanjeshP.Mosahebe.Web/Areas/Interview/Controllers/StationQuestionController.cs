using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SanjeshP.Mosahebe.Web.Areas.Interview.ViewModels.StationQuestion;
using SanjeshP.Mosahebe.Entities.Interview;
using Microsoft.Extensions.Logging;
using SanjeshP.Mosahebe.Data.Contracts.Interview;
using SanjeshP.Mosahebe.WebFramework.Api;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace SanjeshP.Mosahebe.Web.Areas.Interview.Controllers
{
    [Area("Interview")]
    public class StationQuestionController : Controller
    {
        private readonly IStationQuestionRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<StationQuestionController> _logger;

        public StationQuestionController(IStationQuestionRepository repository,
                                         IMapper mapper,
                                         ILogger<StationQuestionController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        #region List Questions
        [HttpGet]
        public IActionResult Index(int stationId)
        {
            if (stationId == 0)
                return BadRequest("ایستگاه معتبر نیست");

            ViewBag.StationId = stationId;
            return PartialView("Index");
        }

        [HttpGet]
        public async Task<IActionResult> StationQuestionList(int stationId, CancellationToken cancellationToken)
        {
            try
            {
                var questions = await _repository.GetAllQuestionsByStationIdAsync(stationId, cancellationToken);
                var model = questions.Select(q => _mapper.Map<StationQuestionViewModel>(q)).ToList();

                ViewBag.StationId = stationId;
                return PartialView("StationQuestionList", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت لیست سوالات ایستگاه");
                return StatusCode(500, "خطا در سرور");
            }
        }
        #endregion

        #region Create Question
        [HttpGet]
        public IActionResult CreateQuestion(int stationId)
        {
            var model = new StationQuestionCreateViewModel
            {
                StationId = stationId
            };
            return PartialView("CreateQuestion", model);
        }

        [HttpPost]
        public async Task<ApiResult> CreateQuestion(StationQuestionCreateViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            try
            {
                var entity = new StationQuestion
                {
                    Text = model.Text,
                    MaxValue = model.MaxValue,
                    StationId = model.StationId,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                await _repository.AddQuestionAsync(entity, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ایجاد سوال ایستگاه");
                return (ApiResult)StatusCode(500, "خطا در سرور");
            }
        }
        #endregion

        #region Edit Question
        [HttpGet]
        public async Task<IActionResult> EditQuestion(int id, CancellationToken cancellationToken)
        {
            var question = await _repository.GetByQuestionIdAsync(id, cancellationToken);
            if (question == null)
                return NotFound();
            ViewBag.StationId = question.StationId;
            var model = _mapper.Map<StationQuestionEditViewModel>(question);
            return PartialView("EditQuestion", model);
        }

        [HttpPost]
        public async Task<ApiResult> EditQuestion(StationQuestionEditViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var existing = await _repository.GetByQuestionIdAsync(model.Id, cancellationToken);
            if (existing == null)
                return NotFound();

            existing.Text = model.Text;
            existing.MaxValue = model.MaxValue;
            existing.UpdatedDate = DateTime.Now;

            try
            {
                await _repository.UpdateQuestionAsync(existing, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ویرایش سوال ایستگاه");
                return (ApiResult)StatusCode(500, "خطا در سرور");
            }
        }
        #endregion

        #region Delete Question
        [HttpPost]
        public async Task<IActionResult> DeleteQuestion(int questionId, CancellationToken cancellationToken)
        {
            try
            {
                if (questionId == 0)
                    return Json(new { isSuccess = false, message = "سؤال معتبر نیست" });

                await _repository.DeleteQuestionAsync(questionId, cancellationToken);
                return Json(new { isSuccess = true, message = "سؤال با موفقیت حذف شد" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در حذف سوال ایستگاه");
                return Json(new { isSuccess = false, message = "خطا در سرور" });
            }
        }
        #endregion
    }
}
