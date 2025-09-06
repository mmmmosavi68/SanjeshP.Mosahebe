using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SanjeshP.Mosahebe.Data;
using SanjeshP.Mosahebe.Data.Contracts.Exam;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Entities.Exam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class TypeOfExamRepository : EFRepository<TypeOfExam>, ITypeOfExamRepository, IScopedDependency
{
    private readonly ILogger<TypeOfExamRepository> _logger;

    public TypeOfExamRepository(ApplicationDbContext dbContext, ILogger<TypeOfExamRepository> logger)
        : base(dbContext)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<TypeOfExam>> GetAllTypeOfExamNoTrackingAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await TableNoTracking
                 .Where(g => !g.IsDeleted)
                 .OrderByDescending(g => g.CreatedDate)
                 .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت همه نوع آزمون‌ها");
            throw;
        }
    }

    public async Task<TypeOfExam> GetByTypeOfExamIDAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            return await Table
                     .Where(g => !g.IsDeleted && g.Id == id)
                     .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت نوع آزمون با شناسه {Id}", id);
            throw;
        }
    }

    public async Task AddTypeOfExamAsync(TypeOfExam typeOfExam, CancellationToken cancellationToken)
    {
        try
        {
            await base.AddAsync(typeOfExam, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در افزودن نوع آزمون");
            throw;
        }
    }

    public async Task UpdateTypeOfExamAsync(TypeOfExam typeOfExam, CancellationToken cancellationToken)
    {
        try
        {
            await base.UpdateAsync(typeOfExam, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در به‌روزرسانی نوع آزمون با شناسه {Id}", typeOfExam.Id);
            throw;
        }
    }

    public async Task DeleteTypeOfExamAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var typeOfExam = await GetByTypeOfExamIDAsync(id, cancellationToken);
            if (typeOfExam == null)
            {
                var msg = $"نوع آزمون با شناسه {id} یافت نشد برای حذف.";
                _logger.LogWarning(msg);
                throw new InvalidOperationException(msg);
            }

            typeOfExam.IsDeleted = true;
            await base.UpdateAsync(typeOfExam, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در حذف نوع آزمون با شناسه {Id}", id);
            throw;
        }
    }
}
