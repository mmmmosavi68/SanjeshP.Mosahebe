using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Entities.Student;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SanjeshP.Mosahebe.Data.Contracts.Students
{
    public interface IResearchDocumentScoreRepository : IEntityFrameworkRepository<ResearchDocumentScore>
    {
        Task<ResearchDocumentScore> GetResearchDocumentScoreByStudentIdUserId(int studentId, Guid userId, CancellationToken cancellationToken);

    }
}

