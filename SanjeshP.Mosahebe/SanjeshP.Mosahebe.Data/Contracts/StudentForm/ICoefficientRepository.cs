using SanjeshP.Mosahebe.Entities.StudentForm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SanjeshP.Mosahebe.Data.Contracts.Common;
using System.Linq.Expressions;

namespace SanjeshP.Mosahebe.Data.Contracts.StudentForm
{
    public interface ICoefficientRepository : IEntityFrameworkRepository<Coefficient>
    {
        Task<IEnumerable<Coefficient>> GetByCoefficientIdAsync(int coefficientId, CancellationToken cancellationToken);
        Task<Coefficient?> FirstOrDefaultAsync(Expression<Func<Coefficient, bool>> predicate, CancellationToken cancellationToken);

    }
}
