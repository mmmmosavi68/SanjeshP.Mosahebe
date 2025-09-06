using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SanjeshP.Mosahebe.Web.Areas.Exam.ViewModels.TypeOfExam;
using SanjeshP.Mosahebe.Data.Contracts.Exam;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using SanjeshP.Mosahebe.Entities.Exam;
using SanjeshP.Mosahebe.WebFramework.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Web.Areas.Exam.Controllers
{
    [Area("Exam")]
    public class TypeOfExamController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILogger<TypeOfExamController> _logger;
        private readonly ITypeOfExamRepository _typeOfExamRepository;
        private readonly IUserTokenRepository _userTokenRepository;

        public TypeOfExamController(IMapper mapper,
                                    ILogger<TypeOfExamController> logger,
                                    ITypeOfExamRepository typeOfExamRepository,
                                    IUserTokenRepository userTokenRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _typeOfExamRepository = typeOfExamRepository;
            _userTokenRepository = userTokenRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> TypeOfExamsList(CancellationToken cancellationToken)
        {
            try
            {
                var entities = await _typeOfExamRepository.GetAllTypeOfExamNoTrackingAsync(cancellationToken);
                var models = _mapper.Map<IEnumerable<TypeOfExamViewModel>>(entities);
                return PartialView("TypeOfExamsList", models);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در متد TypeOfExamsList");
                return StatusCode(500, "خطا در سرور");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailTypeOfExam(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _typeOfExamRepository.GetByTypeOfExamIDAsync(id, cancellationToken);
                if (entity == null) return NotFound();

                var model = _mapper.Map<TypeOfExamViewModel>(entity);
                return PartialView("DetailTypeOfExam", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"خطا در متد DetailTypeOfExam با id = {id}");
                return StatusCode(500, "خطا در سرور");
            }
        }

        #region Create TypeOfExam
        [HttpGet]
        public IActionResult CreateTypeOfExam()
        {
            return PartialView("CreateTypeOfExam");
        }

        [HttpPost]
        public async Task<ApiResult> CreateTypeOfExam(TypeOfExamCreateViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            try
            {
                var currentUserToken = User.FindFirstValue("Token");
                var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(currentUserToken), cancellationToken);

                var entity = _mapper.Map<TypeOfExam>(model);
                entity.Id = Guid.NewGuid();
                entity.CreatedBy = token.UserId;
                entity.HostIp = HttpContext.Connection.RemoteIpAddress?.ToString();

                await _typeOfExamRepository.AddTypeOfExamAsync(entity, cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در متد CreateTypeOfExam");
                return (ApiResult)StatusCode(500, "خطا در سرور");
            }
        }
        #endregion

        #region Edit TypeOfExam
        [HttpGet]
        public async Task<IActionResult> EditTypeOfExam(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _typeOfExamRepository.GetByTypeOfExamIDAsync(id, cancellationToken);
                if (entity == null) return NotFound();

                var model = _mapper.Map<TypeOfExamEditViewModel>(entity);
                return PartialView("EditTypeOfExam", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"خطا در متد EditTypeOfExam GET با id = {id}");
                return StatusCode(500, "خطا در سرور");
            }
        }

        [HttpPost]
        public async Task<ApiResult> EditTypeOfExam(TypeOfExamEditViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            try
            {
                var existing = await _typeOfExamRepository.GetByTypeOfExamIDAsync(model.Id, cancellationToken);
                if (existing == null)
                    return NotFound();

                _mapper.Map(model, existing); // نقشه‌برداری ViewModel به Entity برای ویرایش

                await _typeOfExamRepository.UpdateTypeOfExamAsync(existing, cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"خطا در متد EditTypeOfExam POST با id = {model.Id}");
                return (ApiResult)StatusCode(500, "خطا در سرور");
            }
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> DeleteTypeOfExam(Guid typeOfExamId, CancellationToken cancellationToken)
        {
            try
            {
                if (typeOfExamId == Guid.Empty)
                    return Json(new { isSuccess = false, message = "آزمونی جهت حذف ارسال نشده است یا خطایی رخ داده." });

                var entity = await _typeOfExamRepository.GetByTypeOfExamIDAsync(typeOfExamId, cancellationToken);
                if (entity == null)
                    return Json(new { isSuccess = false, message = "آزمونی یافت نشد." });

                entity.IsDeleted = true;
                await _typeOfExamRepository.UpdateTypeOfExamAsync(entity, cancellationToken);

                return Json(new { isSuccess = true, message = "آزمون با موفقیت حذف شد." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"خطا در متد DeleteTypeOfExam با id = {typeOfExamId}");
                return Json(new { isSuccess = false, message = "خطا در حذف آزمون رخ داده است." });
            }
        }
    }
}
