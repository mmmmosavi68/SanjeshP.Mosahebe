using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SanjeshP.Mosahebe.Data.Contracts.Interview;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using SanjeshP.Mosahebe.Entities.Interview;
using SanjeshP.Mosahebe.WebFramework.Api;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using SanjeshP.Mosahebe.Web.Areas.Interview.ViewModels.Station;
using Microsoft.AspNetCore.Authorization;


namespace SanjeshP.Mosahebe.Web.Areas.Interview.Controllers
{
    [Area("Interview")]
    public class StationController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILogger<StationController> _logger;
        private readonly IStationRepository _stationRepository;
        private readonly IUserTokenRepository _userTokenRepository;

        public StationController(IMapper mapper,
                                 ILogger<StationController> logger,
                                 IStationRepository stationRepository,
                                 IUserTokenRepository userTokenRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _stationRepository = stationRepository;
            _userTokenRepository = userTokenRepository;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> StationList(CancellationToken cancellationToken)
        {
            try
            {
                var stations = await _stationRepository.GetAllStationsNoTrackingAsync(cancellationToken);
                var model = stations.Select(s => new StationViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    IsActive = s.IsActive
                }).ToList();

                return PartialView("StationList", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت فهرست ایستگاه‌ها");
                return StatusCode(500, "خطا در سرور");
            }
        }

        #region Create
        [HttpGet]
        public IActionResult CreateStation() => PartialView("CreateStation");

        [HttpPost]
        public async Task<ApiResult> CreateStation(StationCreateViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            try
            {
                var currentUserToken = User.FindFirstValue("Token");
                var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(currentUserToken), cancellationToken);

                var station = new Station
                {
                    Name = model.Name,
                    CreatedBy = token.UserId,
                    HostIp = Request.HttpContext.Connection.RemoteIpAddress?.ToString(),
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                await _stationRepository.AddStationAsync(station, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ایجاد ایستگاه");
                return (ApiResult)StatusCode(500, "خطا در سرور");
            }
        }
        #endregion

        #region Edit
        [HttpGet]
        public async Task<IActionResult> EditStation(int id, CancellationToken cancellationToken)
        {
            var station = await _stationRepository.GetByStationIDAsync(id, cancellationToken);
            if (station == null)
                return NotFound();

            var model = new StationEditViewModel
            {
                Id = station.Id,
                Name = station.Name,
                IsActive = station.IsActive
            };

            return PartialView("EditStation", model);
        }

        [HttpPost]
        public async Task<ApiResult> EditStation(StationEditViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var existing = await _stationRepository.GetByStationIDAsync(model.Id, cancellationToken);
            if (existing == null)
                return NotFound();

            existing.Name = model.Name;
            existing.IsActive = model.IsActive;
            existing.UpdatedDate = DateTime.Now;

            try
            {
                await _stationRepository.UpdateStationAsync(existing, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ویرایش ایستگاه");
                return (ApiResult)StatusCode(500, "خطا در سرور");
            }
        }
        #endregion

        #region Delete
        [HttpPost]
        public async Task<IActionResult> DeleteStation(int stationId, CancellationToken cancellationToken)
        {
            try
            {
                if (stationId == 0)
                    return Json(new { isSuccess = false, message = "ایستگاه انتخاب نشده است." });

                var station = await _stationRepository.GetByStationIDAsync(stationId, cancellationToken);
                if (station == null)
                    return Json(new { isSuccess = false, message = "ایستگاه یافت نشد." });

                station.IsDeleted = true;
                await _stationRepository.UpdateStationAsync(station, cancellationToken);

                return Json(new { isSuccess = true, message = "ایستگاه با موفقیت حذف شد." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در حذف ایستگاه");
                return Json(new { isSuccess = false, message = "خطا در سرور" });
            }
        }
        #endregion
    }
}