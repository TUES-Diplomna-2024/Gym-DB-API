using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Models.Exercise;
using GymDB.API.Models.Workout;
using GymDB.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymDB.API.Services
{
    public class WorkoutService : IWorkoutService
    {
        private readonly ApplicationContext context;

        private readonly IExerciseService exerciseService;

        public WorkoutService(ApplicationContext context, IExerciseService exerciseService)
        {
            this.context = context;
            this.exerciseService = exerciseService;
        }

        public List<Workout> GetUserWorkouts(User user)
            => context.Workouts.Include(workout => workout.User)
                               .Where(workout => workout.UserId == user.Id).ToList();

        public List<WorkoutPreviewModel> GetWorkoutsPreviews(List<Workout> workouts)
            => workouts.Select(workout => new WorkoutPreviewModel(workout)).ToList();

        public WorkoutWithExercisesModel GetWorkoutWithExercises(Workout workout)
        {
            List<Exercise> exercises = context.WorkoutsExercises
                                              .Include(workoutExercise => workoutExercise.Exercise)
                                              .Where(workoutExercise => workoutExercise.WorkoutId == workout.Id)
                                              .OrderBy(workoutExercise => workoutExercise.Position)
                                              .Select(workoutExercise => workoutExercise.Exercise)
                                              .ToList();

            List<ExercisePreviewModel> exercisePreviews = exerciseService.GetExercisesPreviews(exercises);

            return new WorkoutWithExercisesModel(workout, exercisePreviews);
        }

        public Workout? GetWorkoutById(Guid id)
            => context.Workouts.Include(workout => workout.User)
                               .FirstOrDefault(workout => workout.Id == id);

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
