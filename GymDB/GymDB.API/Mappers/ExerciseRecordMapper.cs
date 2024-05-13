using GymDB.API.Data.Entities;
using GymDB.API.Models.ExerciseRecord;

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
    }
}
