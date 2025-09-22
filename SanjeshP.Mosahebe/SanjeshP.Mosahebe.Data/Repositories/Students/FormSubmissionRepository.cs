using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Contracts.Students;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Entities.Student;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Data.Repositories.Students
{
     public class FormSubmissionRepository : EntityFrameworkRepository<FormSubmission>, IFormSubmissionRepository, IScopedDependency
    {
        public FormSubmissionRepository(ApplicationDbContext dbContext) : base(dbContext) { }


    }
}
