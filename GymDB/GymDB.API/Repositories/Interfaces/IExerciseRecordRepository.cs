using GymDB.API.Data.Entities;
using GymDB.API.Data.Enums;

namespace GymDB.API.Repositories.Interfaces
{
    public interface IExerciseRecordRepository
    {
        Task<ExerciseRecord?> GetExerciseRecordByIdAsync(Guid recordId);

        Task<List<ExerciseRecord>> GetAllExerciseRecordsByExerciseId(Guid exerciseId);

        Task<List<ExerciseRecord>> GetAllExerciseRecordsByOwnerId(Guid ownerId);

        Task<List<ExerciseRecord>> GetAllUserExerciseRecordsSinceAsync(Guid userId, Guid exerciseId, StatisticPeriod period);

        Task AddExerciseRecordAsync(ExerciseRecord record);

        Task UpdateExerciseRecordAsync(ExerciseRecord record);

        Task RemoveExerciseRecordAsync(ExerciseRecord record);

        Task RemoveExerciseRecordRangeAsync(List<ExerciseRecord> exerciseRecords);
    }
}
