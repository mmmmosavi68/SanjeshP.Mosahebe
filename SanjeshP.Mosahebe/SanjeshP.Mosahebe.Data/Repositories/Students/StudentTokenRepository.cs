using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Contracts.Students;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Entities.Student;
using SanjeshP.Mosahebe.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Repositories.Students
{
    public class StudentTokenRepository : EntityFrameworkRepository<StudentToken>, IStudentTokenRepository, IScopedDependency
    {
        public StudentTokenRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public  async Task AddStudentTokenAsync(StudentToken token, CancellationToken cancellationToken, bool saveNow = true)
        {
            await DeleteStudentTokensByStudentIdAsync(token.StudentId, cancellationToken);
            await base.AddAsync(token, cancellationToken);
        }

        public async Task DeleteStudentTokensByStudentIdAsync(int studentId, CancellationToken cancellationToken)
        {
            var tokens = await GetUserStudentTokensByStudentIdAsync(studentId, cancellationToken);
            if (tokens != null)
            {
                foreach (var token in tokens)
                {
                    token.IsDeleted = true;
                    await UpdateAsync(token, cancellationToken);
                }
            }
        }

        public async Task<List<StudentToken>> GetUserStudentTokensByStudentIdAsync(int studentId, CancellationToken cancellationToken)
        {
            return await Table.Where(p => p.StudentId == studentId && p.IsDeleted == false).Include(p => p.Student).ToListAsync(cancellationToken);

        }
    }
}
