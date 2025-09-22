using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Entities.Student;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanjeshP.Mosahebe.Data.Contracts.Students
{
    public interface IFormSubmissionRepository : IEntityFrameworkRepository<FormSubmission>
    {
    }
}
