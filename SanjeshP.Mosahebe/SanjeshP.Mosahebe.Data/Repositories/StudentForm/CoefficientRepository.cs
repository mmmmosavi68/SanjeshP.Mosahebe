using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Contracts.StudentForm;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Entities.StudentForm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Repositories.StudentForm
{
    public class CoefficientRepository : EntityFrameworkRepository<Coefficient>, ICoefficientRepository, IScopedDependency
    {
        public CoefficientRepository(ApplicationDbContext dbContext) : base(dbContext) { }
        public async Task<IEnumerable<Coefficient>> GetByCoefficientIdAsync(int coefficientId, CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .ToListAsync(cancellationToken);
        }
    }
}
