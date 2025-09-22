using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Entities.Exam;
using SanjeshP.Mosahebe.Entities.Student;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Contracts.Students
{
    public interface IFieldDefinitionRepository : IEntityFrameworkRepository<FieldDefinition>
    {
        Task<List<FieldDefinition>> GetAll(CancellationToken cancellationToken);
        Task<List<FieldDefinition>> GetByFormId(int formId, CancellationToken cancellationToken);
        Task<List<int>> GetFormIds(CancellationToken cancellationToken);
        Task<List<FieldDefinition>> GetFields(int formId, CancellationToken cancellationToken);
        Task AddFieldDefinitionAsync(FieldDefinition field, CancellationToken cancellationToken);
        void Add(FormSubmission submission);
        // اضافه شده برای دریافت همه رکوردها
        List<FormSubmission> GetAllSubmissions();
        // متد جدید برای ذخیره فرم‌های ارسال شده
        Task AddFormSubmissionAsync(FormSubmission submission, CancellationToken cancellationToken);

    }
}
