using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Services.Interfaces;

namespace GymDB.API.Services
{
    public class WorkoutService : IWorkoutService
    {
        private readonly ApplicationContext context;

        public WorkoutService(ApplicationContext context)
        {
            this.context = context;
        }

        public void AddWorkout(Workout workout)
        {
            context.Workouts.Add(workout);
            context.SaveChanges();
        }

        public void AddExercisesToWorkout(Workout workout, List<Exercise> exercises)
        {
            int lastPosition = workout.ExerciseCount - 1;

            List<WorkoutExercise> workoutExercises = exercises.Select(exercise => new WorkoutExercise(workout, exercise, ++lastPosition))
                                                              .ToList();

            workout.ExerciseCount = lastPosition + 1;

            context.Workouts.Update(workout);
            context.WorkoutsExercises.AddRange(workoutExercises);

            context.SaveChanges();
        }
    }
}
