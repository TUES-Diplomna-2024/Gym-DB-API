using GymDB.API.Data.Entities;
using GymDB.API.Data.Enums;
using GymDB.API.Models.ExerciseRecord;
using GymDB.API.Models.Exercise;

namespace GymDB.API.Mappers
{
    public static class ExerciseRecordMapper
    {
        public static ExerciseRecord ToEntity(this ExerciseRecordCreateUpdateModel createModel, Exercise exercise, User owner)
        {
            ExerciseRecord record = new()
            {
                Id = Guid.NewGuid(),
                OnCreated = DateTime.UtcNow,
                OnModified = DateTime.UtcNow,
                ExerciseId = exercise.Id,
                OwnerId = owner.Id
            };

            record.ApplyUpdateModel(createModel);

            return record;
        }

        public static void ApplyUpdateModel(this ExerciseRecord record, ExerciseRecordCreateUpdateModel updateModel)
        {
            record.Sets = updateModel.Sets;
            record.Reps = updateModel.Reps;
            record.Weight = updateModel.Weight ?? 0;
            record.Volume = updateModel.Weight != null ? (double)(updateModel.Sets * updateModel.Reps * updateModel.Weight) : 0;
            record.Duration = updateModel.Duration;
        }

        public static ExerciseRecordViewModel ToViewModel(this ExerciseRecord record)
        {
            return new ExerciseRecordViewModel
            {
                Id = record.Id,
                OnCreated = record.OnCreated,
                Sets = record.Sets,
                Reps = record.Reps,
                Weight = record.Weight,
                Duration = record.Duration
            };
        }

        public static StatisticDataPoint ToStatisticDataPoint(this ExerciseRecord record, StatisticMeasurement measurement)
        {
            dynamic value = null;

            switch (measurement)
            {
                case StatisticMeasurement.Sets:
                    value = record.Sets;
                    break;
                case StatisticMeasurement.Reps:
                    value = record.Reps;
                    break;
                case StatisticMeasurement.Volume:
                    value = record.Volume;
                    break;
                case StatisticMeasurement.Duration:
                    value = record.Duration;
                    break;
                case StatisticMeasurement.Weight:
                    value = record.Weight;
                    break;
            }

            return new StatisticDataPoint
            {
                Value = value,
                Date = record.OnCreated
            };
        }
    }
}
