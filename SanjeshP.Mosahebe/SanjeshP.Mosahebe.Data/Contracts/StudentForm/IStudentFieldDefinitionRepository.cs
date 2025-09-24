using SanjeshP.Mosahebe.Data.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SanjeshP.Mosahebe.Entities.StudentForm;

namespace SanjeshP.Mosahebe.Data.Contracts.StudentForm
{
    public interface IStudentFieldDefinitionRepository : IEntityFrameworkRepository<FormItemDefinition>
    {
        Task<IEnumerable<FormItemDefinition>> GetByStudentFormIdAsync(int formId, CancellationToken cancellationToken);
        Task<FormItemDefinition?> GetByStudentFieldDefinitionIdAsync(int id, CancellationToken cancellationToken);
        Task AddStudentFieldDefinitionAsync(FormItemDefinition studentFieldDefinition, CancellationToken cancellationToken);
        Task UpdateStudentFieldDefinitionAsync(FormItemDefinition studentFieldDefinition, CancellationToken cancellationToken);
        Task DeleteStudentFieldDefinitionAsync(int id, CancellationToken cancellationToken);
    }
}
