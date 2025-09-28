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
    internal class StudentOtpRepository : EntityFrameworkRepository<StudentOtp>, IStudentOtpRepository, IScopedDependency
    {
        public StudentOtpRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<StudentOtp> GetValidOtpAsync(int student_ID, string code, CancellationToken cancellationToken)
        {
            return await Table.Where(p => p.StudentId == student_ID && p.Code == code)
             .SingleOrDefaultAsync(cancellationToken)
             .ConfigureAwait(false);
        }
        public async Task InvalidatePreviousOtpsAsync(int studentId, CancellationToken cancellationToken)
        {
            // ۱. بارگذاری همه OTPهای کاربر که هنوز حذف یا استفاده نشده‌اند
            var prevOtps = await Table
                .Where(o => o.StudentId == studentId && !o.IsDeleted)
                .ToListAsync(cancellationToken);

            if (prevOtps.Count == 0)
                return;

            // ۲. علامت‌گذاری هر کدام به عنوان حذف شده
            prevOtps.ForEach(o => o.IsDeleted = true);

            // ۳. ذخیره تغییرات
            await DbContext.SaveChangesAsync(cancellationToken);
        }
        public Task<int> CountOtpByStudentSinceAsync(int studentId, DateTime since, CancellationToken ct)
        {
            return Table
                .Where(o => o.StudentId == studentId && o.CreatedDate >= since)
                .CountAsync(ct);
        }
    }
}
