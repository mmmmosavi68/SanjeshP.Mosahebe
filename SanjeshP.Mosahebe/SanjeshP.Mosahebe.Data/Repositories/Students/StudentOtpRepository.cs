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
    internal class StudentOtpRepository : EFRepository<StudentOtp>, IStudentOtpRepository, IScopedDependency
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
    }
}
