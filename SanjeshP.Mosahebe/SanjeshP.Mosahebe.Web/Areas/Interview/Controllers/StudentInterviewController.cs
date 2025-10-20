using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Threading;
using System;
using SanjeshP.Mosahebe.Data.Contracts.Students;
using System.Linq;
using Microsoft.Extensions.Logging;
using SanjeshP.Mosahebe.Web.Areas.DocumentVerification.Controllers;
using SanjeshP.Mosahebe.Web.Areas.Interview.ViewModels.StudentInterview;
using Microsoft.AspNetCore.Authorization;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using SanjeshP.Mosahebe.Data.Repositories.Users;
using System.Security.Claims;
using SanjeshP.Mosahebe.Data.Contracts.Interview;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Entities.StudentForm;
using SanjeshP.Mosahebe.Web.Areas.Student.ViewModels;
using System.Collections.Generic;
using SanjeshP.Mosahebe.Data.Repositories.Interview;

namespace SanjeshP.Mosahebe.Web.Areas.Interview.Controllers
{
    [AllowAnonymous]
    [Area("Interview")]
    public class StudentInterviewController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<EducationDocumentController> _logger;
        private readonly IUserFieldRepository _userFieldRepository;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly IStationRepository _stationRepository;
        private readonly IFieldStationRepository _fieldStationRepository;
        private readonly IStationQuestionRepository _stationQuestionRepository;
        private readonly IInterviewScoreRepository _interviewScoreRepository;


        public StudentInterviewController(
            IStudentRepository studentRepository
            , ILogger<EducationDocumentController> logger
            , IUserFieldRepository userFieldRepository
            , IUserTokenRepository userTokenRepository
            , IStationRepository stationRepository
            , IFieldStationRepository fieldStationRepository
            , IStationQuestionRepository stationQuestionRepository
            , IInterviewScoreRepository interviewScoreRepository)
        {
            _studentRepository = studentRepository;
            _logger = logger;
            _userFieldRepository = userFieldRepository;
            _userTokenRepository = userTokenRepository;
            _stationRepository = stationRepository;
            _fieldStationRepository = fieldStationRepository;
            _stationQuestionRepository = stationQuestionRepository;
            _interviewScoreRepository = interviewScoreRepository;


        }
        public async Task<IActionResult> Index()
        {
            //آی دی استادی که لاگین شده
            var currentUserToken = User.FindFirstValue("Token");
            var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(currentUserToken), CancellationToken.None);
            var userId = token.UserId;


            //آی دی رشته هایی که مرتبط با استاد هست
            var userFields = await _userFieldRepository.GetExamFieldsByUserIdAsync(userId, CancellationToken.None);
            var examFieldIds = userFields.Select(uf => uf.ExamFieldID).ToList();

            // گرفتن FieldStation های مرتبط
            var fieldStations = await _fieldStationRepository.GetStationsByUserFieldsAsync(examFieldIds, CancellationToken.None);

            // گرفتن (distinct) ایستگاه های مرتبط
            var stations = fieldStations
                .Select(fs => fs.Station)
                .Where(s => s != null)
                .GroupBy(s => s.Id)   
                .Select(g => g.First())
                .ToList();

            return View(stations);
        }

        [HttpGet]
        public async Task<IActionResult> StudentsList(int stationId, CancellationToken cancellationToken)
        {
            try
            {
                // 1. گرفتن FieldStation های مربوط به ایستگاه
                var fieldStations = await _fieldStationRepository.GetFieldsByStationIdAsync(stationId, cancellationToken);
                var examFieldIds = fieldStations.Select(fs => fs.ExamFieldId).ToList();

                // 2. گرفتن دانشجوها بر اساس رشته های ایستگاه
                var filteredStudents = await _studentRepository.GetStudentsByExamFieldIdsAsync(examFieldIds, cancellationToken);


                // 3. تبدیل به ViewModel
                var result = filteredStudents.Select(e => new StudentListViewModel
                {
                    Id = e.Id,
                    RegId = e.RegId,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    FatherName = e.FatherName,
                    NationalCode = e.NationalCode,
                    CodeRahgiri = e.CodeRahgiri,
                    PhoneNumber = e.PhoneNumber,
                    Email = e.Email,
                    Gender = e.Gender == 1 ? "مرد" : "زن",
                    Fields = e.StudentFields != null && e.StudentFields.Any()
                        ? string.Join(", ", e.StudentFields.Select(f => f.ExamField.Title))
                        : ""
                }).ToList();

                ViewBag.StationId = stationId;
                return PartialView("StudentsList", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت لیست داوطلب ‌");
                return StatusCode(500, "خطا در سرور");
            }
        }
        [HttpGet]
        public async Task<IActionResult> StudentInterviewQuestionScore(int studentId, int stationId, CancellationToken cancellationToken)
        {
            if (studentId <= 0 && stationId <= 0) return BadRequest();
            try
            {
                // 1. گرفتن اطلاعات داوطلب
                var student = await _studentRepository.GetStudentByIdAsync(studentId, cancellationToken);
                if (student == null)
                    return NotFound("داوطلب یافت نشد");

                // 2. گرفتن سوالات ایستگاه
                var questions = await _stationQuestionRepository.GetAllQuestionsByStationIdAsync(stationId, cancellationToken);



                var model = new StudentInterviewQuestionViewModel
                {
                    StudentId = student.Id,
                    StudentName = $"{student.FirstName} {student.LastName}",
                    NationalCode = student.NationalCode,
                    StationId = stationId,
                    StationName = (await _stationRepository.GetByStationIDAsync(stationId, cancellationToken))?.Name,
                    Questions = questions.Select(q => new QuestionScoreViewModel
                    {
                        QuestionId = q.Id,
                        QuestionText = q.Text,
                        MaxValue = q.MaxValue
                    }).ToList()
                };

                return PartialView("StudentInterviewQuestionScore", model);
            }
            catch (Exception ex)
            {
                // لاگ دقیق‌تر می‌تونه اضافه بشه در آینده
                return StatusCode(500, "خطا در سرور");
            }
        }
        [HttpPost]
        public async Task<IActionResult> SaveInterviewScores(int studentId, int stationId, Dictionary<int, double> scores, CancellationToken cancellationToken)
        {
            //آی دی استادی که لاگین شده
            var currentUserToken = User.FindFirstValue("Token");
            var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(currentUserToken), CancellationToken.None);
            var userId = token.UserId;

            await _interviewScoreRepository.SaveInterviewScoresAsync(userId, studentId, stationId, scores, cancellationToken);
            //// اینجا می‌تونی نمرات رو تو جدول StudentStationScores یا مشابه ذخیره کنی
            //foreach (var item in scores)
            //{
            //    var questionId = item.Key;
            //    var score = item.Value;

            //    // ذخیره‌سازی در DB
            //}

            //await _dbContext.SaveChangesAsync(cancellationToken);

            return Ok(new { success = true });
        }

    }
}
