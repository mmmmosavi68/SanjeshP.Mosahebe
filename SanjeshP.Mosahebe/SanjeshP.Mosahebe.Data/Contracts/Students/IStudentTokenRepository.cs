using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Entities.Student;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SanjeshP.Mosahebe.Entities.User;

namespace SanjeshP.Mosahebe.Data.Contracts.Students
{
    public interface IStudentTokenRepository : IEntityFrameworkRepository<StudentToken>
    {
        Task<List<StudentToken>> GetUserStudentTokensByStudentIdAsync(int studentId, CancellationToken cancellationToken);
        Task AddStudentTokenAsync(StudentToken token, CancellationToken cancellationToken, bool saveNow = true);
        Task DeleteStudentTokensByStudentIdAsync(int studentId, CancellationToken cancellationToken);
    }
}
