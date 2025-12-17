using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SanjeshP.Mosahebe.Data.Contracts.Exam;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using SanjeshP.Mosahebe.Entities.User;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using SanjeshP.Mosahebe.Web.Areas.User.ViewModels.UserField;
namespace SanjeshP.Mosahebe.Web.Areas.User.Controllers
{
    [Area("User")]
    public class UserFieldController : Controller
    {
        private readonly IUserFieldRepository _userFieldRepository;
        private readonly IExamFieldRepository _examFieldRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserFieldController> _logger;

        public UserFieldController(
            IUserFieldRepository userFieldRepository,
            IExamFieldRepository examFieldRepository,
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<UserFieldController> logger)
        {
            _userFieldRepository = userFieldRepository;
            _examFieldRepository = examFieldRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index(Guid userId)
        {
            ViewData["UserId"] = userId;
            return PartialView();
        }

        [HttpGet]
        public async Task<IActionResult> UserFieldList(Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                var fields = await _userFieldRepository.GetByUserIdAsync(userId, cancellationToken);

                var model = fields.Select(uf => new UserFieldViewModel
                {
                    Id = uf.Id,
                    UserId = uf.UserID,
                    ExamFieldId = uf.ExamFieldID,
                    ExamFieldTitle = uf.ExamFields?.Title ?? "نامشخص",
                    StandardCode = uf.ExamFields?.StandardCode
                }).OrderBy(f => f.ExamFieldTitle).ToList();

                var user = await _userRepository.GetByGuidIdAsync(userId, cancellationToken);
                ViewData["UserDisplayName"] = user?.UserProfiles?.FirstOrDefault()?.FirstName ?? user?.UserName;
                ViewData["UserId"] = userId;

                return PartialView("UserFieldList", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading UserFieldList for userId {UserId}", userId);
                return StatusCode(500, "خطا در بارگزاری فیلدهای کاربر");
            }
        }

        [HttpGet]
        public async Task<IActionResult> FieldsNotAssignedToUser(Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                var allFields = await _examFieldRepository.GetAllFieldsByExamIdAsync(cancellationToken);
                var existing = await _userFieldRepository.GetByUserIdAsync(userId, cancellationToken);

                var notAssigned = allFields
                    .Where(f => !existing.Any(e => e.ExamFieldID == f.Id))
                    .Select(f => new UserFieldViewModel
                    {
                        Id = f.Id,
                        ExamFieldTitle = f.Title,
                        StandardCode = f.StandardCode
                    })
                    .ToList();

                ViewData["UserId"] = userId;
                return PartialView("FieldsNotAssignedToUser", notAssigned);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading FieldsNotAssignedToUser for userId {UserId}", userId);
                return StatusCode(500, "خطا در بارگزاری رشته‌های موجود");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFieldToUser(Guid userId, int fieldId, CancellationToken cancellationToken)
        {
            if (userId == Guid.Empty || fieldId <= 0)
                return BadRequest(new { success = false, message = "پارامترهای ورودی نامعتبر هستند." });

            try
            {
                var exists = await _userFieldRepository.TableNoTracking
                    .AnyAsync(uf => uf.UserID == userId && uf.ExamFieldID == fieldId, cancellationToken);

                if (exists)
                    return BadRequest(new { success = false, message = "رشته قبلاً به کاربر تخصیص داده شده است." });

                var entity = new UserField
                {
                    UserID = userId,
                    ExamFieldID = fieldId
                };

                await _userFieldRepository.AddUserFieldAsync(entity, cancellationToken);
                return Ok(new { success = true, message = "رشته با موفقیت به کاربر اضافه شد." });
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DB error adding field {FieldId} to user {UserId}", fieldId, userId);
                return StatusCode(500, new { success = false, message = "خطا در ثبت اطلاعات در پایگاه داده." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding field {FieldId} to user {UserId}", fieldId, userId);
                return StatusCode(500, new { success = false, message = "خطای سرور" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFieldFromUser(int id, CancellationToken cancellationToken)
        {
            if (id <= 0) return BadRequest(new { success = false, message = "شناسه نامعتبر است." });

            try
            {
                await _userFieldRepository.DeleteUserFieldAsync(id, cancellationToken);
                return Ok(new { success = true, message = "رشته با موفقیت حذف شد." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "رشته‌ای برای حذف یافت نشد." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing UserField id {Id}", id);
                return StatusCode(500, new { success = false, message = "خطای سرور" });
            }
        }
    }
}