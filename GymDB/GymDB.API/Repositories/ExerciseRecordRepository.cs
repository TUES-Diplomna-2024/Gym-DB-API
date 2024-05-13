using Microsoft.EntityFrameworkCore;
using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Data.Enums;
using GymDB.API.Repositories.Interfaces;

namespace GymDB.API.Repositories
{
    public class ExerciseRecordRepository : IExerciseRecordRepository
    {
        private readonly ApplicationContext context;

        public ExerciseRecordRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task<ExerciseRecord?> GetExerciseRecordByIdAsync(Guid recordId)
        {
            return await context.ExerciseRecords
                                .FirstOrDefaultAsync(record => record.Id == recordId);
        }

        public async Task<List<ExerciseRecord>> GetAllExerciseRecordsByExerciseId(Guid exerciseId)
        {
            return await context.ExerciseRecords
                                .Where(record => record.ExerciseId == exerciseId)
                                .ToListAsync();
        }

        public async Task<List<ExerciseRecord>> GetAllExerciseRecordsByOwnerId(Guid ownerId)
        {
            return await context.ExerciseRecords
                                .Where(record => record.OwnerId == ownerId)
                                .ToListAsync();
        }

        public async Task<List<ExerciseRecord>> GetAllUserExerciseRecordsSinceAsync(Guid userId, Guid exerciseId, StatisticPeriod period)
        {
            DateTime startDate = GetStartDate(period);

            return await context.ExerciseRecords
                                .Where(record => record.ExerciseId == exerciseId &&
                                                 record.OwnerId == userId &&
                                                 record.OnCreated >= startDate)
                                .OrderByDescending(record => record.OnCreated)
                                .ToListAsync();
        }

        public async Task AddExerciseRecordAsync(ExerciseRecord record)
        {
            context.ExerciseRecords.Add(record);
            await context.SaveChangesAsync();
        }

        public async Task UpdateExerciseRecordAsync(ExerciseRecord record)
        {
            record.OnModified = DateTime.UtcNow;

            context.ExerciseRecords.Update(record);
            await context.SaveChangesAsync();
        }

        public async Task RemoveExerciseRecordAsync(ExerciseRecord record)
        {
            context.ExerciseRecords.Remove(record);
            await context.SaveChangesAsync();
        }

        public async Task RemoveExerciseRecordRangeAsync(List<ExerciseRecord> exerciseRecords)
        {
            context.ExerciseRecords.RemoveRange(exerciseRecords);
            await context.SaveChangesAsync();
        }

        private DateTime GetStartDate(StatisticPeriod period)
        {
            DateTime startDate = DateTime.MinValue;

            switch (period)
            {
                case StatisticPeriod.OneWeek:
                    startDate = DateTime.UtcNow.AddDays(-7);
                    break;
                case StatisticPeriod.OneMonth:
                    startDate = DateTime.UtcNow.AddMonths(-1);
                    break;
                case StatisticPeriod.TwoMonths:
                    startDate = DateTime.UtcNow.AddMonths(-2);
                    break;
                case StatisticPeriod.ThreeMonths:
                    startDate = DateTime.UtcNow.AddMonths(-3);
                    break;
                case StatisticPeriod.SixMonths:
                    startDate = DateTime.UtcNow.AddMonths(-6);
                    break;
                case StatisticPeriod.OneYear:
                    startDate = DateTime.UtcNow.AddYears(-1);
                    break;
                case StatisticPeriod.All:
                    break;
            }

            return startDate.Date;
        }
    }
}
