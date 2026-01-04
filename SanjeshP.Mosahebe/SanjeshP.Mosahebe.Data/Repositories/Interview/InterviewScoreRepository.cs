using Microsoft.EntityFrameworkCore;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Contracts.Interview;
using SanjeshP.Mosahebe.Data.Repositories.Common;
using SanjeshP.Mosahebe.Entities.Interview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Data.Repositories.Interview
{
    public class InterviewScoreRepository
        : EntityFrameworkRepository<InterviewScore>, IInterviewScoreRepository, IScopedDependency
    {
        public InterviewScoreRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<InterviewScore>> GetAllNoTrackingAsync(CancellationToken cancellationToken)
        {
            return await TableNoTracking
                .Where(s => !s.IsDeleted)
                .OrderByDescending(s => s.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<InterviewScore?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await Table
                .Where(s => !s.IsDeleted && s.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddScoreAsync(InterviewScore score, CancellationToken cancellationToken)
        {
            await AddAsync(score, cancellationToken);
        }

        public async Task UpdateScoreAsync(InterviewScore score, CancellationToken cancellationToken)
        {
            await UpdateAsync(score, cancellationToken);
        }

        public async Task DeleteScoreAsync(int id, CancellationToken cancellationToken)
        {
            var score = await GetByIdAsync(id, cancellationToken);
            if (score == null)
                throw new KeyNotFoundException("امتیاز مورد نظر یافت نشد");

            score.IsDeleted = true;
            await UpdateAsync(score, cancellationToken);
        }

        public async Task SaveInterviewScoresAsync(Guid userId, int studentId, int stationId, Dictionary<int, decimal> scores, string hostIP, CancellationToken cancellationToken)
        {
            foreach (var item in scores)
            {
                int questionId = item.Key;
                double score = (double)item.Value;

                //  دریافت MaxValue از StationQuestion
                var stationQuestion = await DbContext.Set<StationQuestion>()
                    .Where(q => q.Id == questionId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (stationQuestion == null)
                    throw new KeyNotFoundException($"سوالی با شناسه {questionId} یافت نشد");

                double maxValue = (double)stationQuestion.MaxValue;

                var existingScore = await Table
                    .Where(s =>
                        !s.IsDeleted &&
                        s.StudentId == studentId &&
                        s.StationId == stationId &&
                        s.QuestionId == questionId &&
                        s.UserId == userId)
                    .FirstOrDefaultAsync(cancellationToken);

                double calculatedScore = (double)score * maxValue;

                if (existingScore != null)
                {
                    existingScore.QuestionScore = (double)score;
                    existingScore.QuestionMaxValue = maxValue;
                    existingScore.CalculatedScore = calculatedScore;
                    await UpdateAsync(existingScore, cancellationToken);
                }
                else
                {
                    var newScore = new InterviewScore
                    {
                        StudentId = studentId,
                        StationId = stationId,
                        QuestionId = questionId,
                        QuestionScore = (double)score,
                        QuestionMaxValue = maxValue,
                        CalculatedScore = calculatedScore,
                        UserId = userId,
                        HostIp = hostIP
                    };
                    await AddAsync(newScore, cancellationToken);
                }
            }

            await DbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<InterviewScore>> GetInterviewScoresAsync(IEnumerable<int> studentIds, int stationId, CancellationToken cancellationToken)
        {
            if (studentIds == null || !studentIds.Any())
                return new List<InterviewScore>();

            //ـمام نمرات مصاحبه ای که حذف نشده اند، در ایستگاه مشخص شده و برای دانشجویان مشخص شده هستند
            return await TableNoTracking
                .Where(iscore => !iscore.IsDeleted
                                 && studentIds.Contains(iscore.StudentId)
                                 && iscore.StationId == stationId)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
