using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Contracts.Students;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Entities.Exam;
using SanjeshP.Mosahebe.Entities.Student;
using SanjeshP.Mosahebe.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Repositories.Students
{
    public class StudentRepository : EntityFrameworkRepository<Student>, IStudentRepository, IScopedDependency
    {
        public StudentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync(CancellationToken cancellationToken)
        {
            return await Table
                   .Include(s => s.Exam)                                  // شامل آزمون
                   .Include(s => s.StudentFields)                         // شامل جدول واسط
                       .ThenInclude(sf => sf.ExamField)                   // شامل رشته‌های واقعی
                   .Where(s => !s.IsDeleted && s.IsActive)
                   .OrderByDescending(s => s.CreatedDate)
                   .ToListAsync(cancellationToken)
                   .ConfigureAwait(false);
        }
        public async Task<IEnumerable<Student>> GetAllStudentsByFieldIdAsync(int fieldId, CancellationToken cancellationToken)
        {
            return await Table
                    .Where(s => !s.IsDeleted && s.IsActive &&
                                s.StudentFields.Any(sf => sf.ExamFieldId == fieldId && !sf.IsDeleted && sf.IsActive))
                    //.Include(s => s.Exam)
                    .Include(s => s.StudentFields)
                        .ThenInclude(sf => sf.ExamField)
                    //.Include(s => s.EducationDocumentScores)
                    .Include(i => i.InterviewScores)
                    .OrderByDescending(s => s.CreatedDate)
                    .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<Student>> GetAllStudentsByFieldIdUserIdAsync(int fieldId, Guid UserId, CancellationToken cancellationToken)
        {
            return await Table
                    .Where(s => !s.IsDeleted && s.IsActive && !s.IsPajoheshi &&
                                s.StudentFields.Any(sf => sf.ExamFieldId == fieldId && sf.IsActive && !sf.IsDeleted))
                    .Include(s => s.Exam)
                    .Include(s => s.StudentFields)
                        .ThenInclude(sf => sf.ExamField)
                    .Include(s => s.EducationDocumentScores)
                    //.Where(e => e.CreatedBy == UserId)
                    .OrderByDescending(s => s.CreatedDate)
                    .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Student>> GetAllStudentsPajoheshiByFieldIdAsync(int fieldId, CancellationToken cancellationToken)
        {
            return await Table
                    .Where(s => !s.IsDeleted && s.IsPajoheshi == true &&
                                s.StudentFields.Any(sf => sf.ExamFieldId == fieldId))
                    .Include(s => s.Exam)
                    .Include(s => s.ResearchDocumentScore)
                    .Include(s => s.StudentFields)
                        .ThenInclude(sf => sf.ExamField)
                    .Include(s => s.EducationDocumentScores)
                    .OrderByDescending(s => s.CreatedDate)
                    .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<Student>> GetAllStudentsNoTrackingAsync(CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Include(s => s.Exam)                   // شامل آزمون مرتبط
                .Include(s => s.StudentFields)                         // شامل جدول واسط
                       .ThenInclude(sf => sf.ExamField)                   // شامل رشته‌های واقعی
                .OrderByDescending(s => s.CreatedDate)  // مرتب‌سازی بر اساس تاریخ ایجاد
                .Where(s => !s.IsDeleted && s.IsActive)       // فقط دانشجویان فعال (حذف نشده)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Student> GetStudentByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                 .Include(s => s.Exam)
                 .Include(s => s.StudentFields)                       // شامل جدول واسط
                       .ThenInclude(sf => sf.ExamField)                   // شامل رشته‌های واقعی
                 .FirstOrDefaultAsync(s => s.Id == id && s.IsDeleted == false, cancellationToken)
                 .ConfigureAwait(false);
        }
        public async Task<Student> GetStudentDetailsOnReviewDocument(int id, CancellationToken cancellationToken)
        {
            return await TableNoTracking
            .Where(s => s.Id == id && !s.IsDeleted)
            .Select(s => new Student
            {
                Id = s.Id,
                Exam = s.Exam,
                FirstName = s.FirstName,
                LastName = s.LastName,
                NationalCode = s.NationalCode,
                StudentFields = s.StudentFields
            .Where(sf => sf.IsActive)
            .Select(sf => new StudentField
            {
                Id = sf.Id,
                IsActive = sf.IsActive,
                ExamField = sf.ExamField
            }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<Student> GetStudentByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                         .Include(s => s.Exam)
                         .Include(s => s.StudentFields)                         // شامل جدول واسط
                       .ThenInclude(sf => sf.ExamField)                   // شامل رشته‌های واقعی
                         .FirstOrDefaultAsync(s => s.NationalCode == nationalCode && !s.IsDeleted && s.IsActive, cancellationToken)
                         .ConfigureAwait(false);
        }

        public async Task<Student> GetStudentByNationalCodeCodeRahgiriAsync(string nationalCode, string codeRahgiri, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                    .Include(s => s.Exam)
                    .Include(s => s.StudentFields)                         // شامل جدول واسط
                       .ThenInclude(sf => sf.ExamField)                   // شامل رشته‌های واقعی
                    .FirstOrDefaultAsync(s => s.NationalCode == nationalCode
                                           && s.CodeRahgiri == codeRahgiri
                                           && !s.IsDeleted && s.IsActive, cancellationToken)
                    .ConfigureAwait(false);
        }

        public async Task<Student> GetStudentByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                        .Include(s => s.Exam)
                        .Include(s => s.StudentFields)                         // شامل جدول واسط
                       .ThenInclude(sf => sf.ExamField)                   // شامل رشته‌های واقعی
                        .FirstOrDefaultAsync(s => s.PhoneNumber == phoneNumber && !s.IsDeleted && s.IsActive, cancellationToken)
                        .ConfigureAwait(false);
        }

        public async Task<IEnumerable<Student>> GetStudentsByExamFieldIdsAsync(IEnumerable<int> examFieldIds, CancellationToken cancellationToken)
        {
            return await Table
                        .Include(s => s.StudentFields)
                            .ThenInclude(sf => sf.ExamField)
                        .Include(s => s.Exam)
                        .Where(s => !s.IsDeleted && s.IsActive
                                    && s.StudentFields.Any(sf => examFieldIds.Contains(sf.ExamFieldId)))
                        .OrderByDescending(s => s.CreatedDate)
                        .ToListAsync(cancellationToken)
                        .ConfigureAwait(false);
        }

        public async Task<IEnumerable<Student>> GetStudentsByExamFieldIdInterviewActivesAsync(IEnumerable<int> examFieldIds, CancellationToken cancellationToken)
        {
            return await Table
                        .Where(s => s.InterviewProcessStatus == InterviewProcessStatus.Active)
                        .Include(s => s.StudentFields)
                            .ThenInclude(sf => sf.ExamField)
                        .Include(s => s.Exam)
                        .Where(s => !s.IsDeleted && s.IsActive
                                    && s.StudentFields.Any(sf => examFieldIds.Contains(sf.ExamFieldId)))
                        .OrderByDescending(s => s.CreatedDate)
                        .ToListAsync(cancellationToken)
                        .ConfigureAwait(false);
        }
        public async Task AddStudentAsync(Student student, CancellationToken cancellationToken)
        {
            await AddAsync(student, cancellationToken);
        }

        public async Task UpdateStudentAsync(Student student, CancellationToken cancellationToken)
        {
            await UpdateAsync(student, cancellationToken);
        }

        public async Task<Student> GetStudentByIdDontRelationAsync(int id, CancellationToken cancellationToken)
        {
            return await Table // بدون NoTracking و Includeهای اضافی
         .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, cancellationToken);
        }

        public async Task<(List<Student>, int totalCount)> GetPagedStudentsAsync(SearchPagingParameters parameters, int fieldId, bool IsPajoheshi, CancellationToken cancellationToken)
        {
            var query = TableNoTracking.Where(s => !s.IsDeleted && s.IsActive && s.IsPajoheshi == IsPajoheshi &&
                        s.StudentFields.Any(sf => sf.ExamFieldId == fieldId && !sf.IsDeleted && sf.IsActive))
                        .Include(ue => ue.StudentFields).ThenInclude(ue => ue.ExamField)
                        .Include(i => i.InterviewScores)
                        .AsQueryable();

            // --- فیلترهای داینامیک ---
            if (parameters.Filters?.Any() == true)
            {
                foreach (var filter in parameters.Filters)
                {
                    var keyword = filter.Value?.Trim().ToLower();
                    if (string.IsNullOrEmpty(keyword)) continue;

                    switch (filter.Key)
                    {
                        case "FirstName": query = query.Where(s => EF.Functions.Like(s.FirstName, $"%{keyword}%")); break;
                        case "LastName": query = query.Where(s => EF.Functions.Like(s.LastName, $"%{keyword}%")); break;
                        case "NationalCode": query = query.Where(s => EF.Functions.Like(s.NationalCode, $"%{keyword}%")); break;
                        case "FatherName": query = query.Where(s => EF.Functions.Like(s.FatherName, $"%{keyword}%")); break;
                        case "InterviewProcessStatus":
                            if (int.TryParse(keyword, out int statusValue))
                            {
                                // تبدیل عدد به اینام اصلی
                                var statusEnum = (Entities.Student.InterviewProcessStatus)statusValue;
                                query = query.Where(s => s.InterviewProcessStatus == statusEnum);
                            }
                            else
                            {
                                // جستجو بر اساس کلمات فارسی (اگر کاربر دستی تایپ کرد)
                                if ("فعال".Contains(keyword))
                                    query = query.Where(s => s.InterviewProcessStatus == InterviewProcessStatus.Active);
                                else if ("تکمیل".Contains(keyword))
                                    query = query.Where(s => s.InterviewProcessStatus == InterviewProcessStatus.Completed);
                                else if ("شروع".Contains(keyword) || "نشده".Contains(keyword))
                                    query = query.Where(s => s.InterviewProcessStatus == InterviewProcessStatus.NotStarted);
                            }
                            break;
                        case "PassedStation":
                            if (int.TryParse(keyword, out int ps))
                                query = query.Where(s => s.InterviewScores.Select(x => x.StationId).Distinct().Count() == ps);
                            break;
                    }
                }
            }

            var totalCount = await query.CountAsync(cancellationToken);

            // --- مرتب‌سازی داینامیک ---
            if (!string.IsNullOrWhiteSpace(parameters.SortColumn))
            {
                query = parameters.SortColumn switch
                {
                    "FirstName" => parameters.SortAscending ? query.OrderBy(u => u.FirstName) : query.OrderByDescending(u => u.FirstName),
                    "LastName" => parameters.SortAscending ? query.OrderBy(u => u.LastName) : query.OrderByDescending(u => u.LastName),
                    "NationalCode" => parameters.SortAscending ? query.OrderBy(u => u.NationalCode) : query.OrderByDescending(u => u.NationalCode),
                    "InterviewProcessStatus" => parameters.SortAscending ? query.OrderBy(u => u.InterviewProcessStatus) : query.OrderByDescending(u => u.InterviewProcessStatus),
                    "PassedStation" => parameters.SortAscending ?
                        query.OrderBy(u => u.InterviewScores.Select(x => x.StationId).Distinct().Count()) :
                        query.OrderByDescending(u => u.InterviewScores.Select(x => x.StationId).Distinct().Count()),
                    _ => query.OrderByDescending(u => u.Id)
                };
            }
            else { query = query.OrderByDescending(u => u.Id); }

            var users = await query.Skip((parameters.PageIndex - 1) * parameters.PageSize).Take(parameters.PageSize).ToListAsync(cancellationToken);
            return (users, totalCount);
        }
    }
}
