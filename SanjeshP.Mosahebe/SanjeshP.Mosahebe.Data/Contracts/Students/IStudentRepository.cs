using SanjeshP.Mosahebe.Common;
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
    public interface IStudentRepository : IEntityFrameworkRepository<Student>
    {
        Task<IEnumerable<Student>> GetAllStudentsNoTrackingAsync(CancellationToken cancellationToken);
        Task<IEnumerable<Student>> GetAllStudentsAsync(CancellationToken cancellationToken);
        Task<IEnumerable<Student>> GetAllStudentsByFieldIdAsync(int fieldIf, CancellationToken cancellationToken);
        Task<IEnumerable<Student>> GetAllStudentsByFieldIdUserIdAsync(int fieldIf, Guid UserId, CancellationToken cancellationToken);
        Task<IEnumerable<Student>> GetAllStudentsPajoheshiByFieldIdAsync(int fieldIf, CancellationToken cancellationToken);
        Task<Student> GetStudentByNationalCodeAsync(string NationalCode, CancellationToken cancellationToken);
        Task<Student> GetStudentByNationalCodeCodeRahgiriAsync(string NationalCode, string CodeRahgiri, CancellationToken cancellationToken);
        Task<Student> GetStudentByIdAsync(int id, CancellationToken cancellationToken);
        Task<Student> GetStudentByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken);
        Task<IEnumerable<Student>> GetStudentsByExamFieldIdsAsync(IEnumerable<int> examFieldIds, CancellationToken cancellationToken);
        Task<IEnumerable<Student>> GetStudentsByExamFieldIdInterviewActivesAsync(IEnumerable<int> examFieldIds, CancellationToken cancellationToken);

        Task AddStudentAsync(Student student, CancellationToken cancellationToken);
        Task UpdateStudentAsync(Student student, CancellationToken cancellationToken);
    }
}
