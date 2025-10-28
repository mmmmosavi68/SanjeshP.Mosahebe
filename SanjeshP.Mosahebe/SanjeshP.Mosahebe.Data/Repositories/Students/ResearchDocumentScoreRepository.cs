using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Contracts.Students;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Entities.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Repositories.Students
{
    public class ResearchDocumentScoreRepository : EntityFrameworkRepository<ResearchDocumentScore>, IResearchDocumentScoreRepository, IScopedDependency
    {
        public ResearchDocumentScoreRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<ResearchDocumentScore> GetResearchDocumentScoreByStudentIdUserId(int studentId, Guid userId, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                           .Where(r => r.StudentId == studentId && r.IsDeleted == false)
                            .FirstOrDefaultAsync(cancellationToken);
        }
    }
}