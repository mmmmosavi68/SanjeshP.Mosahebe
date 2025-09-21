using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Entities.Student;
using SanjeshP.Mosahebe.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SanjeshP.Mosahebe.Data.Contracts.Students
{
    public interface IStudentOtpRepository : IEntityFrameworkRepository<StudentOtp>
    {
        Task<StudentOtp> GetValidOtpAsync(int student_ID, string code, CancellationToken cancellationToken);

    }
}
