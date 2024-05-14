using GymDB.API.Data.Enums;
using GymDB.API.Models.Exercise;
using GymDB.API.Models.ExerciseRecord;

namespace GymDB.API.Services.Interfaces
{
    public interface IExerciseRecordService
    {
        Task CreateNewExerciseRecordAsync(HttpContext context, Guid exerciseId, ExerciseRecordCreateUpdateModel createModel);

        Task<List<ExerciseRecordViewModel>> GetCurrUserExerciseRecordsViewsAsync(HttpContext context, Guid exerciseId, StatisticPeriod period);

        Task<ExerciseStatisticsModel> GetCurrUserExerciseStatisticsAsync(HttpContext context, Guid exerciseId, StatisticPeriod period, StatisticMeasurement measurement);

        Task UpdateExerciseRecordByIdAsync(HttpContext context, Guid recordId, ExerciseRecordCreateUpdateModel updateModel);

        Task RemoveExerciseRecordByIdAsync(HttpContext context, Guid recordId);
    }
}
