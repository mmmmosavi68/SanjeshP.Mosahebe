using SanjeshP.Mosahebe.Data.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SanjeshP.Mosahebe.Entities.StudentForm;

namespace SanjeshP.Mosahebe.Data.Contracts.StudentForm
{
    public interface IStudentFieldDefinitionRepository : IEntityFrameworkRepository<StudentFieldDefinition>
    {
        Task<IEnumerable<StudentFieldDefinition>> GetByStudentFormIdAsync(int formId, CancellationToken cancellationToken);
        Task<StudentFieldDefinition?> GetByStudentFieldDefinitionIdAsync(int id, CancellationToken cancellationToken);
        Task AddStudentFieldDefinitionAsync(StudentFieldDefinition studentFieldDefinition, CancellationToken cancellationToken);
        Task UpdateStudentFieldDefinitionAsync(StudentFieldDefinition studentFieldDefinition, CancellationToken cancellationToken);
        Task DeleteStudentFieldDefinitionAsync(int id, CancellationToken cancellationToken);
    }
}
