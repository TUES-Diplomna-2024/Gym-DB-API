using GymDB.API.Data.Entities;
using GymDB.API.Models.Exercise;
using GymDB.API.Models.ExerciseRecord;

namespace GymDB.API.Mappers
{
    public static class ExerciseRecordMapper
    {
        public static ExerciseRecord ToEntity(this ExerciseRecordCreateUpdateModel createModel, Exercise exercise, User owner)
        {
            return new ExerciseRecord
            {
                Id = Guid.NewGuid(),
                OnCreated = DateTime.UtcNow,
                OnModified = DateTime.UtcNow,
                
                Sets = createModel.Sets,
                Reps = createModel.Reps,
                Weight = createModel.Weight ?? 0,
                Volume = createModel.Weight != null ? (double)(createModel.Sets * createModel.Reps * createModel.Weight) : 0,
                Duration = createModel.Duration,

                UserId = owner.Id,
                User = owner,
               
                ExerciseId = exercise.Id,
                Exercise = exercise
            };
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
