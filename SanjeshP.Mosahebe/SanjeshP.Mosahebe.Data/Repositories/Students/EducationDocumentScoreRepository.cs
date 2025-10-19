using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SanjeshP.Mosahebe.Data.Contracts.Students;
using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Entities.Student;
using Microsoft.Data.SqlClient;
using SanjeshP.Mosahebe.Common.Utilities;
using System.Data;
using System.Linq;

namespace SanjeshP.Mosahebe.Data.Repositories.Students
{
    public class EducationDocumentScoreRepository : EntityFrameworkRepository<EducationDocumentScore>, IEducationDocumentScoreRepository, IScopedDependency
    {
        public EducationDocumentScoreRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<double> GetEducationDocumentScoreValueByStudentId(int studentId, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                         .Where(ed => ed.StudentId == studentId && ed.IsActive && !ed.IsDeleted)
                         .Select(ed => ed.FinalTotalScore)
                         .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<Dictionary<int, double>> GetScoresByStudentIdsAsync(List<int> studentIds, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Where(ed => studentIds.Contains(ed.StudentId) && ed.IsActive && !ed.IsDeleted)
                .ToDictionaryAsync(ed => ed.StudentId, ed => ed.FinalTotalScore, cancellationToken);
        }
        public async Task SaveOrUpdateFinalScoreAsync(int studentId, double finalScore, CancellationToken cancellationToken)
        {
            var existing = await Table.FirstOrDefaultAsync(x => x.StudentId == studentId, cancellationToken);

            if (existing != null)
            {
                existing.FinalTotalScore = finalScore;
                await UpdateAsync(existing, cancellationToken);
            }
            else
            {
                var doc = new EducationDocumentScore
                {
                    StudentId = studentId,
                    FinalTotalScore = finalScore,
                };
                await AddAsync(doc, cancellationToken);
            }

            await SaveChangesAsync(cancellationToken);
        }

        public async Task CalculateAndSaveFinalScoreAsync(int studentId, Guid userId, CancellationToken cancellationToken)
        {

            CreateSqlParameter createSqlParameter = new CreateSqlParameter();
            SqlParameter[] param_userid = new SqlParameter[2];
            param_userid[0] = createSqlParameter.Create_Param("@StudentId", studentId, SqlDbType.Int, 4);
            param_userid[1] = createSqlParameter.Create_Param("@UserId", userId, SqlDbType.UniqueIdentifier, 32);
            var result = Entities.FromSqlRaw($"EXEC Calculator_StudentsFinalScores_EducationDocument @StudentId,@UserId", param_userid).AsNoTracking().ToList();

        }
    }
}