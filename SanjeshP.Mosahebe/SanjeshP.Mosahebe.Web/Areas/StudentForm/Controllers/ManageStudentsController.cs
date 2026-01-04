using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SanjeshP.Mosahebe.Data.Contracts.StudentForm;
using SanjeshP.Mosahebe.Data.Contracts.Students;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using SanjeshP.Mosahebe.Data.Repositories.Students;
using SanjeshP.Mosahebe.Presentation.Controllers;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using SanjeshP.Mosahebe.Web.Areas.StudentForm.ViewModels;
using SanjeshP.Mosahebe.Data.Contracts.Exam;
using SanjeshP.Mosahebe.Entities.Student;
using SanjeshP.Mosahebe.WebFramework.Api;
using System.Security.Claims;
using AutoMapper;
using SanjeshP.Mosahebe.Data.Contracts.Interview;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Repositories.Users;
using SanjeshP.Mosahebe.Web.Areas.User.ViewModels.User;
using SanjeshP.Mosahebe.Web.SharedViewModels;
using static SanjeshP.Mosahebe.Web.Areas.User.Controllers.UsersController;

namespace SanjeshP.Mosahebe.Web.Areas.StudentForm.Controllers
{
    [Area("StudentForm")]
    public class ManageStudentsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ManageStudentsController> _logger;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IExamRepository _examRepository;
        private readonly IExamFieldRepository _examFieldRepository;
        private readonly IStationRepository _stationRepository;
        private readonly IInterviewScoreRepository _interviewScoreRepository;

        public ManageStudentsController(IMapper mapper,
            ILogger<ManageStudentsController> logger,
            IUserTokenRepository userTokenRepository,
            IStudentRepository studentRepository,
            IExamRepository examRepository,
            IExamFieldRepository examFieldRepository,
            IStationRepository stationRepository,
            IInterviewScoreRepository interviewScoreRepository)
        {
            this._mapper = mapper;
            _logger = logger;
            _userTokenRepository = userTokenRepository;
            _studentRepository = studentRepository;
            _examRepository = examRepository;
            _examFieldRepository = examFieldRepository;
            this._stationRepository = stationRepository;
            this._interviewScoreRepository = interviewScoreRepository;
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var exams = await _examRepository.GetAllExamsNoTrackingAsync(cancellationToken); // فرض کنیم متد داریم
            return View(exams);
        }

        // لیست رشته‌ها بر اساس آزمون
        public async Task<IActionResult> FieldDropdown(int examId, CancellationToken cancellationToken)
        {
            var fields = await _examFieldRepository.GetAllFieldsByExamIdAsync(examId, cancellationToken);
            return PartialView("FieldDropdown", fields);
        }

        [HttpGet]
        public async Task<IActionResult> StudentsList(int fieldId, CancellationToken cancellationToken)
        {
            try
            {
                //آی دی کاربری که لاگین شده
                var currentUserToken = User.FindFirstValue("Token");
                var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(currentUserToken), CancellationToken.None);
                var userPajoheshi = token.User?.IsPajoheshi ?? false;

                var CountStationField = await _stationRepository.GetStationsByFieldIdAsync(fieldId, cancellationToken);

                // دریافت همه دانشجویان فعال
                var students = await _studentRepository.GetAllStudentsByFieldIdAsync(fieldId, cancellationToken);

                if (students == null || !students.Any())
                    return PartialView("StudentsList", new List<StudentListViewModel>());


                // نگاشت دانشجویان به ViewModel
                var result = students.Where(s => s.IsPajoheshi == userPajoheshi).Select(e =>
                {
                    return new StudentListViewModel
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

                        // تبدیل جنسیت عددی به متن
                        Gender = e.Gender == 1 ? "مرد" : "زن",

                        // تبدیل IsPajoheshi به بولین
                        IsPajoheshi = e.IsPajoheshi,

                        // نام آزمون از navigation property
                        ExamName = e.Exam?.Title ?? "",

                        // نوع دوره آزمون به متن
                        //ExamCourseType = e.ExamCourseType == 1 ? "آموزشی" : "پژوهشی",

                        InterviewProcessStatus = (InterviewProcessStatusView)e.InterviewProcessStatus,
                        // رشته‌ها به صورت رشته کاما جدا شده
                        Fields = e.StudentFields != null && e.StudentFields.Any()
                                    ? string.Join(", ", e.StudentFields.Select(f => f.ExamField.Title))
                                    : "",
                        CountStationField = CountStationField.Count(),
                        PassedStation = e.InterviewScores?.Select(i => i.StationId).Distinct().Count() ?? 0
                    };
                })
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ThenBy(s => s.IsPajoheshi)
                .ToList();

                return PartialView("StudentsList", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت لیست دانشجویان");
                return StatusCode(500, "خطا در سرور");
            }
        }

        public class StudentSearchFilter
        {
            [FromQuery(Name = "pageIndex")]
            public int PageIndex { get; set; } = 1;

            [FromQuery(Name = "pageSize")]
            public int PageSize { get; set; } = 10;

            [FromQuery(Name = "sortColumn")]
            public string SortColumn { get; set; }

            [FromQuery(Name = "sortAscending")]
            public bool SortAscending { get; set; }
        }
                                         
        public async Task<IActionResult> StudentsListPaged([FromQuery] StudentSearchFilter filter, int fieldId, CancellationToken cancellationToken = default)
        {
            ViewBag.FieldId = fieldId; 
            // جلوگیری از نال بودن مدل
            filter ??= new StudentSearchFilter();

            var mainParams = new[] { "pageIndex", "pageSize", "sortColumn", "sortAscending", "_" };

            // دریافت همه QueryStringها بجز pageIndex/pageSize
            var filters = Request.Query
                .Where(kv => kv.Key != "pageIndex" && kv.Key != "pageSize")
                .ToDictionary(kv => kv.Key, kv => kv.Value.ToString()?.Trim());

            var parameters = new SearchPagingParameters
            {
                PageIndex = filter.PageIndex,
                PageSize = filter.PageSize,
                Filters = filters,
                SortColumn = filter.SortColumn,
                SortAscending = filter.SortAscending
            };

            //آی دی کاربری که لاگین شده
            var currentUserToken = User.FindFirstValue("Token");
            var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(currentUserToken), CancellationToken.None);
            var userPajoheshi = token.User?.IsPajoheshi ?? false;

            parameters.SortAscending = filter.SortAscending;
            // دریافت کاربران با جستجوهای چندفیلدی و صفحه‌بندی
            var (students, totalCount) = await _studentRepository.GetPagedStudentsAsync(parameters, fieldId, userPajoheshi, cancellationToken);
            var CountStationField = await _stationRepository.GetStationsByFieldIdAsync(fieldId, cancellationToken);

            // ساخت لیست ViewModel با سطح دسترسی
            var viewModels = students.Select(e =>
            {
                StudentListViewModel vm = new()
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

                    // تبدیل جنسیت عددی به متن
                    Gender = e.Gender == 1 ? "مرد" : "زن",

                    // تبدیل IsPajoheshi به بولین
                    IsPajoheshi = e.IsPajoheshi,

                    // نام آزمون از navigation property
                    ExamName = e.Exam?.Title ?? "",

                    // نوع دوره آزمون به متن
                    //ExamCourseType = e.ExamCourseType == 1 ? "آموزشی" : "پژوهشی",

                    InterviewProcessStatus = (InterviewProcessStatusView)e.InterviewProcessStatus,
                    // رشته‌ها به صورت رشته کاما جدا شده
                    Fields = e.StudentFields != null && e.StudentFields.Any()
                                    ? string.Join(", ", e.StudentFields.Select(f => f.ExamField.Title))
                                    : "",
                    CountStationField = CountStationField.Count(),
                    PassedStation = e.InterviewScores?.Select(i => i.StationId).Distinct().Count() ?? 0
                };

                return new ListItemAccessViewModel<StudentListViewModel>
                {
                    Data = vm,
                };
            }).ToList();

            // ساخت مدل نهایی همراه با اطلاعات صفحه‌بندی
            var result = new PagedListAccessViewModel<StudentListViewModel>
            {
                Items = viewModels,
                TotalCount = totalCount,
                PageIndex = filter.PageIndex, // تغییر از pageIndex به filter.PageIndex
                PageSize = filter.PageSize,   // تغییر از pageSize به filter.PageSize
                SortColumn = filter.SortColumn, // اضافه کنید به کلاس مدل
                SortAscending = filter.SortAscending, // اضافه کنید به کلاس مدل
                AppliedFilters = filters
            };

            return PartialView("StudentsListPaged", result);
        }


        #region جزئیات دانشجو
        [HttpGet]
        public async Task<IActionResult> DetailStudent(int id, CancellationToken cancellationToken)
        {
            try
            {
                var student = await _studentRepository.GetStudentByIdAsync(id, cancellationToken);
                if (student == null)
                    return NotFound();

                var model = _mapper.Map<StudentListViewModel>(student);
                return PartialView("DetailStudent", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت جزئیات دانشجو");
                return StatusCode(500, "خطا در سرور");
            }
        }
        #endregion

        #region افزودن دانشجو
        [HttpGet]
        public IActionResult CreateStudent()
        {
            return PartialView("CreateStudent");
        }

        [HttpPost]
        public async Task<ApiResult> CreateStudent(StudentCreateViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            try
            {
                var currentUserToken = User.FindFirstValue("Token");
                var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(currentUserToken), cancellationToken);

                var student = new Entities.Student.Student
                {
                    RegId = model.RegId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    FatherName = model.FatherName,
                    NationalCode = model.NationalCode,
                    CodeRahgiri = model.CodeRahgiri,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    //Gender = model.Gender  ? 1 : 2,
                    IsPajoheshi = model.IsPajoheshi ? true : false,
                    ExamId = model.ExamId,
                    //ExamCourseType = model.ExamCourseType,
                    InterviewProcessStatus = InterviewProcessStatus.NotStarted,
                    CreatedBy = token.UserId,
                    CreatedDate = DateTime.Now,
                    HostIp = Request.HttpContext.Connection.RemoteIpAddress?.ToString(),
                    IsActive = true
                };

                await _studentRepository.AddStudentAsync(student, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ایجاد دانشجو");
                return (ApiResult)StatusCode(500, "خطا در سرور");
            }
        }
        #endregion

        #region ویرایش دانشجو
        [HttpGet]
        public async Task<IActionResult> EditStudent(int id, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetStudentByIdAsync(id, cancellationToken);
            if (student == null)
                return NotFound();

            var model = _mapper.Map<StudentEditViewModel>(student);
            return PartialView("EditStudent", model);
        }

        [HttpPost]
        public async Task<ApiResult> EditStudent(StudentEditViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var existing = await _studentRepository.GetStudentByIdDontRelationAsync(model.Id, cancellationToken);
            if (existing == null)
                return NotFound();

            existing.InterviewProcessStatus = (InterviewProcessStatus)model.InterviewProcessStatus;
            existing.UpdatedDate = DateTime.Now;

            try
            {
                await _studentRepository.UpdateStudentAsync(existing, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ویرایش دانشجو");
                return (ApiResult)StatusCode(500, "خطا در سرور");
            }
        }
        #endregion

        #region حذف دانشجو
        [HttpPost]
        public async Task<IActionResult> DeleteStudent(int studentId, CancellationToken cancellationToken)
        {
            try
            {
                var student = await _studentRepository.GetStudentByIdAsync(studentId, cancellationToken);
                if (student == null)
                    return Json(new { isSuccess = false, message = "دانشجو یافت نشد." });

                student.IsDeleted = true;
                await _studentRepository.UpdateStudentAsync(student, cancellationToken);

                return Json(new { isSuccess = true, message = "دانشجو با موفقیت حذف شد." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در حذف دانشجو");
                return Json(new { isSuccess = false, message = "خطا در سرور" });
            }
        }
        #endregion
    }
}
