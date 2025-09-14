using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Contracts.Exam;
using SanjeshP.Mosahebe.Data.Contracts.Students;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Entities.Exam;
using SanjeshP.Mosahebe.Entities.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Repositories.Students
{
    public class FieldModelRepository : EFRepository<FieldDefinition>, IFieldModelRepository, IScopedDependency
    {
        public FieldModelRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        private static int _nextId = 1;

        //private static readonly List<FieldDefinition> _Data = new()
        //{
        //    // فرم 1
        //    new FieldDefinition { Id=1, FormId=1, Name="BookName",    Label="نام کتاب",     Type=FieldType.Text,     IsRequired=true,  Description="مثال: صد سال تنهایی",      ErrorMessage="وارد کردن نام الزامی است" },
        //    new FieldDefinition { Id=2, FormId=1, Name="ISBN",        Label="شماره شابک",    Type=FieldType.Text,     IsRequired=true,  Description="فرمت: 978-3-16-148410-0", ErrorMessage="شابک معتبر نیست" },
        //    new FieldDefinition { Id=3, FormId=1, Name="BookLink",    Label="لینک وب‌سایت", Type=FieldType.Url,      IsRequired=false, Description="",                           ErrorMessage="" },

        //    // فرم 2
        //    new FieldDefinition { Id=4, FormId=2, Name="ArticleTitle",Label="عنوان مقاله",   Type=FieldType.Text,     IsRequired=true,  Description="",                           ErrorMessage="عنوان الزامی است" },
        //    new FieldDefinition { Id=5, FormId=2, Name="ArticleFile", Type=FieldType.File,     Label="بارگذاری فایل", IsRequired=true,  Description="فرمت PDF",                   ErrorMessage="فایل را انتخاب کنید" },

        //    // فرم 3
        //    new FieldDefinition { Id=6, FormId=3, Name="ArticleTitle2", Label="عنوان مقاله",  Type=FieldType.Text,     IsRequired=true,  Description="",                           ErrorMessage="عنوان الزامی است" },
        //    new FieldDefinition { Id=7, FormId=3, Name="Rank",          Label="نفر چندم",     Type=FieldType.Number,   IsRequired=true,  Description="عدد بین 1 تا 10",            ErrorMessage="عدد معتبر وارد کنید" },
        //    new FieldDefinition { Id=8, FormId=3, Name="IsInternal",    Label="داخلی است؟",   Type=FieldType.Checkbox, IsRequired=false, Description="",                           ErrorMessage="" }
        //};

        public async Task<List<FieldDefinition>> GetAll(CancellationToken cancellationToken)
        {
            var sss = await TableNoTracking
                       .OrderByDescending(e => e.FormId)
                       .ToListAsync(cancellationToken);
            return sss;
        }

        public async Task<List<FieldDefinition>> GetByFormId(int formId, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                         .Where(e => e.IsDeleted==false && e.FormId == formId)  // احتمالا باید FormId چک شود نه Id
                         .OrderByDescending(e => e.FormId)
                         .ToListAsync(cancellationToken);
        }

        public async Task<List<int>> GetFormIds(CancellationToken cancellationToken)
        {
            var allFields = await GetAll(cancellationToken);
            var result = allFields
                         .Select(x => x.FormId)
                         .Distinct()
                         .OrderBy(x => x)
                         .ToList();  // اینجا اضافه شده
            return result;
        }

        public async Task<List<FieldDefinition>> GetFields(int formId, CancellationToken cancellationToken)
        {
            var result = await GetByFormId(formId, cancellationToken);
            return result;
        }

        public void Add(FormSubmission submission)
        {
            submission.Id = _nextId++;
            submission.SubmittedAt = DateTime.UtcNow;
            Add(submission);
        }

        public async Task AddFieldDefinitionAsync(FieldDefinition field,CancellationToken cancellationToken)
        {                                     
            await AddAsync(field, cancellationToken);
        }
    }
}
