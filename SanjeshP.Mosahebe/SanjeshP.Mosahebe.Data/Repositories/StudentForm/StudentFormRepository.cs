using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SanjeshP.Mosahebe.Data.Contracts.StudentForm;
using SanjeshP.Mosahebe.Entities.StudentForm;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SanjeshP.Mosahebe.Data.Repositories.StudentForm
{
    public class StudentFormRepository : EFRepository<StudentForms>, IStudentFormRepository, IScopedDependency
    {
        public StudentFormRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<StudentForms>> GetAllStudentFormsNoTrackingAsync(CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Where(f => !f.IsDeleted)
                .OrderByDescending(f => f.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<StudentForms?> GetByStudentFormsIdAsync(int id, CancellationToken cancellationToken)
        {
            return await Table
                .Where(f => !f.IsDeleted && f.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddStudentFormsAsync(StudentForms studentForms, CancellationToken cancellationToken) =>
            await AddAsync(studentForms, cancellationToken);

        public async Task UpdateStudentFormsAsync(StudentForms studentForms, CancellationToken cancellationToken) =>
            await UpdateAsync(studentForms, cancellationToken);

        public async Task DeleteStudentFormsAsync(int id, CancellationToken cancellationToken)
        {
            var form = await GetByStudentFormsIdAsync(id, cancellationToken);
            if (form is null)
                throw new KeyNotFoundException("فرم مورد نظر یافت نشد");

            form.IsDeleted = true;
            await UpdateAsync(form, cancellationToken);
        }
    }
}
