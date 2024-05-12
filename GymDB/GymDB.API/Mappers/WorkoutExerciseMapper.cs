using GymDB.API.Data.Entities;

namespace GymDB.API.Mappers
{
    public static class WorkoutExerciseMapper
    {
        public static WorkoutExercise ToWorkoutExerciseEntity(this Exercise exercise, Workout workout, int position)
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
    }
}
