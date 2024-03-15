using GymDB.API.Data.Entities;
using GymDB.API.Data.Enums;
using GymDB.API.Models.Exercise;
using GymDB.API.Models.ExerciseRecord;

namespace GymDB.API.Services.Interfaces
{
    public interface IExerciseRecordService
    {
        List<ExerciseRecord> GetUserExerciseRecordsSince(User user, Exercise exercise, StatisticPeriod period);

        List<ExerciseRecordViewModel> GetRecordsViews(List<ExerciseRecord> records);

        ExerciseRecord? GetRecordById(Guid id);

        ExerciseStatisticsModel? GetUserExerciseStatisticsSince(User user, Exercise exercise, StatisticPeriod period, StatisticMeasurement measurement);

        bool IsRecordBelongsToExercise(ExerciseRecord record, Exercise exercise);

        bool IsRecordOwnedByUser(ExerciseRecord record, User user);

        void AddRecord(ExerciseRecord record);

        void UpdateRecord(ExerciseRecord record, ExerciseRecordCreateUpdateModel update);

        void RemoveRecord(ExerciseRecord record);

        void RemoveAllExerciseRecords(Exercise exercise);

        void RemoveAllUserRecords(User user);
    }
}
