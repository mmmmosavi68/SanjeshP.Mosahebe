using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Contracts.Students;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using SanjeshP.Mosahebe.Data.Repositories.Common;
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
                 .Include(s => s.StudentFields)                         // شامل جدول واسط
                       .ThenInclude(sf => sf.ExamField)                   // شامل رشته‌های واقعی
                 .FirstOrDefaultAsync(s => s.Id == id && s.IsDeleted == false && s.IsActive == true, cancellationToken)
                 .ConfigureAwait(false);
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
    }
}
