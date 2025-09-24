using SanjeshP.Mosahebe.Data.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SanjeshP.Mosahebe.Entities.StudentForm;

namespace SanjeshP.Mosahebe.Data.Contracts.StudentForm
{
    public interface IStudentFormRepository : IEntityFrameworkRepository<FormItem>
    {
        Task<IEnumerable<FormItem>> GetAllStudentFormsNoTrackingAsync(CancellationToken cancellationToken);
        Task<FormItem?> GetByStudentFormsIdAsync(int id, CancellationToken cancellationToken);
        Task AddStudentFormsAsync(FormItem studentForm, CancellationToken cancellationToken);
        Task UpdateStudentFormsAsync(FormItem studentForm, CancellationToken cancellationToken);
        Task DeleteStudentFormsAsync(int id, CancellationToken cancellationToken);
    }
}
