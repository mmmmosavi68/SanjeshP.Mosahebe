using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SanjeshP.Mosahebe.Web.Areas.Exam.ViewModels.ExamSchedule;
using SanjeshP.Mosahebe.Common.Enums.ExamSchedule;
using SanjeshP.Mosahebe.Common.Utilities;
using SanjeshP.Mosahebe.Data.Contracts.Exam;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using SanjeshP.Mosahebe.Entities.Exam;
using SanjeshP.Mosahebe.WebFramework.Api;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Web.Areas.Exam.Controllers
{
    [Area("Exam")]
    public class ExamScheduleController : Controller
    {

        private readonly IExamScheduleRepository _examScheduleRepository;
        private readonly IExamRepository _examRepository;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ExamScheduleController> _logger;

        public ExamScheduleController(
            IExamScheduleRepository examScheduleRepository,
            IExamRepository examRepository,
            IUserTokenRepository userTokenRepository,
            IMapper mapper,
            ILogger<ExamScheduleController> logger)
        {
            _examScheduleRepository = examScheduleRepository;
            _examRepository = examRepository;
            _userTokenRepository = userTokenRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index(int examId)
        {
            if (examId == 0)
                return BadRequest("آزمون معتبر نیست");

            try
            {
                ViewBag.ExamId = examId;
                return PartialView("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت لیست رویدادهای آزمون");
                return StatusCode(500, "خطا در سرور");
            }
        }

        // نمایش لیست زمان‌بندی‌ها برای آزمون انتخاب شده
        [HttpGet]
        public async Task<IActionResult> ExamScheduleList(int examId, CancellationToken cancellationToken)
        {
            if (examId == 0)
                return BadRequest("آزمون معتبر نیست");

            try
            {
                var schedules = await _examScheduleRepository.GetAllSchedulesByExamIdAsync(examId, cancellationToken);
                var model = schedules.Select(s => _mapper.Map<ExamScheduleViewModel>(s)).ToList();

                ViewBag.ExamId = examId;
                return PartialView("ExamScheduleList", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت لیست رویدادهای آزمون");
                return StatusCode(500, "خطا در سرور");
            }
        }

        #region Create Schedule

        [HttpGet]
        public async Task<IActionResult> CreateExamSchedule(int examId, CancellationToken cancellationToken)
        {
            var usedEventTypes = await _examScheduleRepository.GetUsedEventTypesByExamIdAsync(examId, cancellationToken);

            var allEventTypes = Enum.GetValues(typeof(ExamEventType)).Cast<ExamEventType>();

            var availableEventTypes = allEventTypes
                .Where(x => !usedEventTypes.Contains(x))
                .Select(x => new SelectListItem
                {
                    Value = x.ToString(),
                    Text = x.ToDisplay()
                })
                .ToList();

            var model = new ExamScheduleCreateViewModel
            {
                ExamId = examId,
                EventTypeSelectList = availableEventTypes
            };

            return PartialView(model);
        }


        [HttpPost]
        public async Task<ApiResult> CreateExamSchedule(ExamScheduleCreateViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var currentUserToken = User.FindFirstValue("Token");
                var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(currentUserToken), cancellationToken);
                var eventsType = ExamEventType.Registration;

                var entity = new ExamSchedule
                {
                 
                    ExamId = model.ExamId,
                    EventType = model.EventType,
                    EventTypeName = model.EventType.ToString(),
                    EventTypeDisplayName = model.EventType.ToDisplay(),
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    IsActive = true,
                    CreatedBy = token.UserId,
                    CreatedDate = DateTime.Now,
                    HostIp = HttpContext.Connection.RemoteIpAddress?.ToString()
                };

                await _examScheduleRepository.AddScheduleAsync(entity, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ایجاد رویداد زمان‌بندی");
                return (ApiResult)StatusCode(500, "خطا در سرور");
            }
        }

        #endregion

        #region Edit Schedule

        [HttpGet]
        public async Task<IActionResult> EditExamSchedule(int id, CancellationToken cancellationToken)
        {
            var schedule = await _examScheduleRepository.GetAllSchedulesByExamIdAsync(id, cancellationToken);
            if (schedule == null)
                return NotFound();

            var model = _mapper.Map<ExamScheduleEditViewModel>(schedule);
            return PartialView("EditExamSchedule", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditExamSchedule(ExamScheduleEditViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _examScheduleRepository.GetAllSchedulesByExamIdAsync(model.Id, cancellationToken);
            if (existing == null)
                return NotFound();

            try
            {
                //existing.EventType = model.EventType;
                //existing.StartTime = model.StartTime;
                //existing.EndTime = model.EndTime;
                //existing.IsActive = model.IsActive;
                //existing.UpdatedDate = DateTime.Now;

                //await _examScheduleRepository.UpdateScheduleAsync(existing, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ویرایش رویداد زمان‌بندی");
                return StatusCode(500, "خطا در سرور");
            }
        }

        #endregion

        #region Delete Schedule

        [HttpPost]
        public async Task<IActionResult> DeleteExamSchedule(Guid scheduleId, CancellationToken cancellationToken)
        {
            try
            {
                if (scheduleId == Guid.Empty)
                    return Json(new { isSuccess = false, message = "شناسه رویداد معتبر نیست" });

                //var schedule = await _examScheduleRepository.GetByIdAsync(scheduleId, cancellationToken);
                //if (schedule == null)
                //    return Json(new { isSuccess = false, message = "رویداد یافت نشد" });

                //schedule.IsDeleted = true;
                //await _examScheduleRepository.UpdateScheduleAsync(schedule, cancellationToken);

                return Json(new { isSuccess = true, message = "رویداد با موفقیت حذف شد" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در حذف رویداد زمان‌بندی");
                return Json(new { isSuccess = false, message = "خطا در سرور" });
            }
        }

        #endregion
    }
}