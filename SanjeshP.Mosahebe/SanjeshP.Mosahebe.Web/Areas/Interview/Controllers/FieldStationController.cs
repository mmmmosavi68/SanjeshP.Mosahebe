using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SanjeshP.Mosahebe.Data.Contracts.Exam;
using SanjeshP.Mosahebe.Data.Contracts.Interview;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using SanjeshP.Mosahebe.Entities.Interview;
using System.Threading.Tasks;
using System.Threading;
using System;
using SanjeshP.Mosahebe.Web.Areas.Interview.ViewModels.FieldStation;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SanjeshP.Mosahebe.Data.Repositories.Interview;

namespace SanjeshP.Mosahebe.Web.Areas.Interview.Controllers
{
    [AllowAnonymous]
    [Area("Interview")]
    public class FieldStationController : Controller
    {
        private readonly IFieldStationRepository _fieldStationRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<FieldStationController> _logger;
        private readonly IExamFieldRepository _examFieldRepository;
        private readonly IStationRepository _stationRepository;

        public FieldStationController(IFieldStationRepository fieldStationRepository
            , IMapper mapper
            , ILogger<FieldStationController> logger
            , IExamFieldRepository examFieldRepository
            , IStationRepository stationRepository)
        {
            _fieldStationRepository = fieldStationRepository;
            _mapper = mapper;
            _logger = logger;
            _examFieldRepository = examFieldRepository;
            _stationRepository = stationRepository;
        }

        // نمایش مودال رشته‌ها برای ایستگاه
        public IActionResult Index(int stationId)
        {
            ViewData["StationId"] = stationId;
            return PartialView();
        }

        // لیست رشته‌های ایستگاه
        public async Task<IActionResult> FieldStationList(int stationId, CancellationToken cancellationToken)
        {
            var fields = await _fieldStationRepository.GetFieldsByStationIdAsync(stationId, cancellationToken);
            var model = fields.Select(fs => new FieldStationViewModel
            {
                Id = fs.Id,
                FieldId = fs.ExamFieldId,
                FieldTitle = fs.ExamField != null ? fs.ExamField.Title : "نامشخص",
                StationId = fs.StationId,
                StationTitle = fs.Station != null ? fs.Station.Name : "نامشخص"
            }).ToList();
            ViewData["StationId"] = stationId;  
            return PartialView("FieldStationList", model);
        }

        // لیست رشته‌های موجود برای اضافه کردن
        public async Task<IActionResult> FieldsNotInStation(int stationId, CancellationToken cancellationToken)
        {
            // گرفتن ایستگاه مورد نظر
            var station = await _stationRepository.GetByStationIDAsync(stationId, cancellationToken);
            if (station == null)
                return NotFound("ایستگاه یافت نشد");

            // گرفتن همه رشته‌ها
            var allFields = await _examFieldRepository.GetAllFieldsByExamIdAsync(cancellationToken);

            // گرفتن رشته‌های موجود در ایستگاه
            var existingFields = await _fieldStationRepository.GetFieldsByStationIdAsync(stationId, cancellationToken);

            // فیلتر رشته‌هایی که هنوز به ایستگاه اضافه نشده‌اند
            var notInStation = allFields
                .Where(f => !existingFields.Any(ef => ef.ExamFieldId == f.Id))
                .Select(f => new FieldViewModel
                {
                    Id = f.Id,
                    Title = f.Title,
                    StandardCode = f.StandardCode,
                    Station = station
                })
                .ToList();

            return PartialView("FieldsNotInStation", notInStation);
        }

        [HttpPost]
        public async Task<IActionResult> AddFieldToStation(int stationId, int fieldId, CancellationToken cancellationToken)
        {
            var exists = await _fieldStationRepository.TableNoTracking
                .AnyAsync(fs => fs.StationId == stationId && fs.ExamFieldId == fieldId, cancellationToken);

            if (exists)
                return BadRequest("رشته قبلا به ایستگاه اضافه شده است.");

            var entity = new FieldStation
            {
                StationId = stationId,
                ExamFieldId = fieldId
            };

            await _fieldStationRepository.AddFieldStationAsync(entity, cancellationToken);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFieldFromStation(int id, CancellationToken cancellationToken)
        {
            await _fieldStationRepository.DeleteFieldStationAsync(id, cancellationToken);
            return Ok();
        }
    }
}
