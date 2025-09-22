using SanjeshP.Mosahebe.Data.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SanjeshP.Mosahebe.Entities.StudentForm;

namespace SanjeshP.Mosahebe.Data.Contracts.StudentForm
{
    public interface IStudentFormRepository : IEntityFrameworkRepository<StudentForms>
    {
        Task<IEnumerable<StudentForms>> GetAllStudentFormsNoTrackingAsync(CancellationToken cancellationToken);
        Task<StudentForms?> GetByStudentFormsIdAsync(int id, CancellationToken cancellationToken);
        Task AddStudentFormsAsync(StudentForms studentForm, CancellationToken cancellationToken);
        Task UpdateStudentFormsAsync(StudentForms studentForm, CancellationToken cancellationToken);
        Task DeleteStudentFormsAsync(int id, CancellationToken cancellationToken);
    }
}
