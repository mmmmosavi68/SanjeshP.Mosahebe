using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SanjeshP.Mosahebe.Web.Areas.Exam.ViewModels.Exam;
using SanjeshP.Mosahebe.Web.Areas.Exam.ViewModels.TypeOfExam;
using SanjeshP.Mosahebe.Common;
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
    public class ExamController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ExamController> _logger;
        private readonly IExamRepository _examRepository;
        private readonly ITypeOfExamRepository _typeOfExamRepository;
        private readonly IUserTokenRepository _userTokenRepository;

        public ExamController(IMapper mapper,
                              ILogger<ExamController> logger,
                              IExamRepository examRepository,
                              ITypeOfExamRepository typeOfExamRepository,
                              IUserTokenRepository userTokenRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _examRepository = examRepository;
            _typeOfExamRepository = typeOfExamRepository;
            _userTokenRepository = userTokenRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ExamsList(CancellationToken cancellationToken)
        {
            try
            {
                var exams = await _examRepository.GetAllExamsNoTrackingAsync(cancellationToken);
                var result = exams.Select(e => new ExamViewModel
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    TypeOfExamId = e.TypeOfExamId,
                    TypeOfExamTitle = e.TypeOfExam?.Title,
                    IsActive = e.IsActive
                }).ToList();

                return PartialView("ExamsList", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت لیست آزمون‌ها");
                return StatusCode(500, "خطا در سرور");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailExam(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var exam = await _examRepository.GetByExamsIDAsync(id, cancellationToken);
                if (exam == null)
                {
                    return NotFound();
                }

                var model = _mapper.Map<ExamViewModel>(exam);
                model.TypeOfExamTitle = exam.TypeOfExam?.Title;

                return PartialView("DetailExam", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت جزئیات آزمون");
                return StatusCode(500, "خطا در سرور");
            }
        }

        #region Create Exam
        [HttpGet]
        public async Task<IActionResult> CreateExam(CancellationToken cancellationToken)
        {
            var typeOfExams = await _typeOfExamRepository.GetAllTypeOfExamNoTrackingAsync(cancellationToken);
            var model = new ExamCreateViewModel
            {
                TypeOfExamList = typeOfExams.Select(t => new TypeOfExamViewModel
                {
                    Id = t.Id,
                    Title = t.Title
                }).ToList()
            };

            return PartialView("CreateExam", model);
        }

        [HttpPost]
        public async Task<ApiResult> CreateExam(ExamCreateViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            try
            {
                var currentUserToken = User.FindFirstValue("Token");
                var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(currentUserToken), cancellationToken);

                var exam = new Exams
                {
                    Id = Guid.NewGuid(),
                    Title = model.Title,
                    Description = model.Description,
                    TypeOfExamId = model.TypeOfExamId,
                    CreatedBy = token.UserId,
                    HostIp = Request.HttpContext.Connection.RemoteIpAddress?.ToString(),
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                await _examRepository.AddExamsAsync(exam, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ایجاد آزمون");
                return (ApiResult)StatusCode(500, "خطا در سرور");
            }
        }
        #endregion

        #region Edit Exam
        [HttpGet]
        public async Task<IActionResult> EditExam(Guid id, CancellationToken cancellationToken)
        {
            var exam = await _examRepository.GetByExamsIDAsync(id, cancellationToken);
            if (exam == null)
            {
                return NotFound();
            }

            var typeOfExams = await _typeOfExamRepository.GetAllTypeOfExamNoTrackingAsync(cancellationToken);

            var model = new ExamEditViewModel
            {
                Id = exam.Id,
                Title = exam.Title,
                Description = exam.Description,
                TypeOfExamId = exam.TypeOfExamId,
                IsActive = exam.IsActive,
                TypeOfExamList = typeOfExams.Select(t => new TypeOfExamViewModel
                {
                    Id = t.Id,
                    Title = t.Title
                }).ToList()
            };

            return PartialView("EditExam", model);
        }

        [HttpPost]
        public async Task<ApiResult> EditExam(ExamEditViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var existing = await _examRepository.GetByExamsIDAsync(model.Id, cancellationToken);
            if (existing == null)
            {
                return NotFound();
            }

            existing.Title = model.Title;
            existing.Description = model.Description;
            existing.TypeOfExamId = model.TypeOfExamId;
            existing.IsActive = model.IsActive;
            existing.UpdatedDate = DateTime.Now;

            try
            {
                await _examRepository.UpdateExamsAsync(existing, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ویرایش آزمون");
                return (ApiResult)StatusCode(500, "خطا در سرور");
            }
        }
        #endregion

        #region Delete Exam
        [HttpPost]
        public async Task<IActionResult> DeleteExam(Guid examId, CancellationToken cancellationToken)
        {
            try
            {
                if (examId == Guid.Empty)
                {
                    return Json(new { isSuccess = false, message = "آزمون جهت حذف ارسال نشده است یا خطایی رخ داده." });
                }

                var exam = await _examRepository.GetByExamsIDAsync(examId, cancellationToken);
                if (exam == null)
                {
                    return Json(new { isSuccess = false, message = "آزمون یافت نشد." });
                }

                exam.IsDeleted = true;
                await _examRepository.UpdateExamsAsync(exam, cancellationToken);

                return Json(new { isSuccess = true, message = "آزمون با موفقیت حذف شد." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در حذف آزمون");
                return Json(new { isSuccess = false, message = "خطا در سرور" });
            }
        }
        #endregion
    }
}
