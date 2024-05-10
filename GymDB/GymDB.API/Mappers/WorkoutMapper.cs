using GymDB.API.Data.Entities;
using GymDB.API.Models.Exercise;
using GymDB.API.Models.Workout;
using Microsoft.IdentityModel.Tokens;

namespace GymDB.API.Mappers
{
    public static class WorkoutMapper
    {
        public static Workout ToEntity(this WorkoutCreateModel createModel, User owner)
        {
            return new Workout
            {
                Id = Guid.NewGuid(),
                OnCreated = DateOnly.FromDateTime(DateTime.UtcNow),
                OnModified = DateTime.UtcNow,
                ExerciseCount = 0,

                Name = createModel.Name,
                Description = createModel.Description,

                OwnerId = owner.Id,
                Owner = owner
            };
        }

        public static void ApplyUpdateModel(this Workout workout, WorkoutUpdateModel update)
        {
            workout.Name = update.Name;
            workout.Description = update.Description;
        }

        public static WorkoutViewModel ToViewModel(this Workout workout, List<ExercisePreviewModel>? exercises)
        {
            return new WorkoutViewModel
            {
                Id = workout.Id,
                Name = workout.Name,
                Description = workout.Description,
                Exercises = exercises.IsNullOrEmpty() ? null : exercises
            };
        }

        public static WorkoutPreviewModel ToPreviewModel(this Workout workout)
        {
            return new WorkoutPreviewModel
            {
                Id = workout.Id,
                Name = workout.Name,
                ExerciseCount = workout.ExerciseCount
            };
        }
    }
}
