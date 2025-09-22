using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SanjeshP.Mosahebe.Entities.StudentForm;
using SanjeshP.Mosahebe.Data.Contracts.StudentForm;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SanjeshP.Mosahebe.Data.Repositories.StudentForm
{
    public class StudentFieldDefinitionRepository : EFRepository<StudentFieldDefinition>, IStudentFieldDefinitionRepository, IScopedDependency
    {
        public StudentFieldDefinitionRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<StudentFieldDefinition>> GetByStudentFormIdAsync(int formId, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Where(f => f.FormId == formId && !f.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<StudentFieldDefinition?> GetByStudentFieldDefinitionIdAsync(int id, CancellationToken cancellationToken)
        {
            return await Table
                .Where(f => f.Id == id && !f.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddStudentFieldDefinitionAsync(StudentFieldDefinition studentFieldDefinition, CancellationToken cancellationToken) =>
            await AddAsync(studentFieldDefinition, cancellationToken);

        public async Task UpdateStudentFieldDefinitionAsync(StudentFieldDefinition studentFieldDefinition, CancellationToken cancellationToken) =>
            await UpdateAsync(studentFieldDefinition, cancellationToken);

        public async Task DeleteStudentFieldDefinitionAsync(int id, CancellationToken cancellationToken)
        {
            var field = await GetByStudentFieldDefinitionIdAsync(id, cancellationToken);
            if (field is null)
                throw new KeyNotFoundException("فیلد مورد نظر یافت نشد");

            field.IsDeleted = true;
            await UpdateAsync(field, cancellationToken);
        }
    }
}
