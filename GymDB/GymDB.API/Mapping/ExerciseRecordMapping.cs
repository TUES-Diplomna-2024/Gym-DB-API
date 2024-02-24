using GymDB.API.Data.Entities;
using GymDB.API.Models.ExerciseRecord;

namespace GymDB.API.Mapping
{
    public static class ExerciseRecordMapping
    {
        public static ExerciseRecord ToEntity(this ExerciseRecordCreateModel createModel, Exercise exercise, User owner)
        {
            return new ExerciseRecord
            {
                Id = Guid.NewGuid(),
                OnCreated = DateOnly.FromDateTime(DateTime.UtcNow),
                OnModified = DateTime.UtcNow,
                
                Sets = createModel.Sets,
                Reps = createModel.Reps,
                Weight = createModel.Weight,
                Volume = createModel.Weight != null ? (double)(createModel.Sets * createModel.Reps * createModel.Weight) : 0,
                Duration = createModel.Duration,
                Date = DateTime.UtcNow,
                
                UserId = owner.Id,
                User = owner,
               
                ExerciseId = exercise.Id,
                Exercise = exercise
            };
        }
    }
}
