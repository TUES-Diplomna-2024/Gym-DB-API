using GymDB.API.Data.Entities;
using GymDB.API.Models.Exercise;
using GymDB.API.Models.Workout;

namespace GymDB.API.Mappers
{
    public static class WorkoutMapper
    {
        public static Workout ToEntity(this WorkoutCreateUpdateModel model, User owner)
        {
            return new Workout
            {
                Id = Guid.NewGuid(),
                OnCreated = DateOnly.FromDateTime(DateTime.UtcNow),
                OnModified = DateTime.UtcNow,
                ExerciseCount = 0,

                Name = model.Name,
                Description = model.Description,
                UserId = owner.Id,
                User = owner
            };
        }

        public static WorkoutExercise ToWorkoutExerciseEntity(this Workout workout, Exercise exercise, int position)
        {
            return new WorkoutExercise
            {
                Id = Guid.NewGuid(),
                WorkoutId = workout.Id,
                Workout = workout,
                ExerciseId = exercise.Id,
                Exercise = exercise,
                Position = position
            };
        }

        public static WorkoutWithExercisesModel ToWorkoutWithExercisesModel(this Workout workout, List<ExercisePreviewModel>? exercises)
        {
            return new WorkoutWithExercisesModel
            {
                Id = workout.Id,
                Name = workout.Name,
                Description = workout.Description,
                ExerciseCount = workout.ExerciseCount,
                Exercises = exercises
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
