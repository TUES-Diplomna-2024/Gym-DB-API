using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Models.Exercise;
using GymDB.API.Models.Workout;
using GymDB.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

        public List<Workout> GetWorkoutsContainingExercise(Exercise exercise)
            => context.WorkoutsExercises.Include(workoutExercise => workoutExercise.Workout.User)
                                        .Where(workoutExercise => workoutExercise.ExerciseId == exercise.Id)
                                        .Select(workoutExercise => workoutExercise.Workout).ToList();

        public List<Exercise> GetWorkoutExercises(Workout workout)
            => context.WorkoutsExercises.Include(workoutExercise => workoutExercise.Exercise.User)
                                        .Where(workoutExercise => workoutExercise.WorkoutId == workout.Id)
                                        .OrderBy(workoutExercise => workoutExercise.Position)
                                        .Select(workoutExercise => workoutExercise.Exercise).ToList();


        public WorkoutWithExercisesModel GetWorkoutWithExercises(Workout workout)
        {
            List<Exercise> exercises = GetWorkoutExercises(workout);

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
            int lastPosition = workout.ExerciseCount;

            List<WorkoutExercise> workoutExercises = exercises.Select(exercise => new WorkoutExercise(workout, exercise, lastPosition++))
                                                              .ToList();

            workout.ExerciseCount = lastPosition;

            context.Workouts.Update(workout);
            context.WorkoutsExercises.AddRange(workoutExercises);

            context.SaveChanges();
        }

        public Guid[]? AddExercisesToWorkout(Workout workout, List<Guid> exercisesIds, User owner)
        {
            List<Exercise> exercises = exerciseService.GetUserAccessibleExercisesByIds(exercisesIds, owner);

            AddExercisesToWorkout(workout, exercises);

            if (exercises.Count() != exercisesIds!.Count())
            {
                Guid[] foundExerciseIds = exercises.Select(exercise => exercise.Id).ToArray();
                Guid[] notFoundExerciseIds = exercisesIds!.Where(id => !foundExerciseIds.Contains(id)).ToArray();

                return notFoundExerciseIds;
            }

            return null;
        }

        public Guid[]? UpdateWorkout(Workout workout, WorkoutCreateUpdateModel update, User owner)
        {
            workout.Name = update.Name;
            workout.Description = update.Description;
            workout.OnModified = DateTime.UtcNow;

            if (workout.ExerciseCount != 0)
                RemoveAllWorkoutExercises(workout);

            if (!update.Exercises.IsNullOrEmpty())
            {
                Guid[]? notFoundExerciseIds = AddExercisesToWorkout(workout, update.Exercises!, owner);
                return notFoundExerciseIds;
            }

            return null;
        }

        public void RemoveAllWorkoutExercises(Workout workout)
        {
            List<WorkoutExercise> toBeRemoved = context.WorkoutsExercises
                                                       .Where(workoutExercise => workoutExercise.WorkoutId == workout.Id)
                                                       .ToList();

            context.WorkoutsExercises.RemoveRange(toBeRemoved);

            workout.ExerciseCount = 0;
            context.Workouts.Update(workout);

            context.SaveChanges();
        }

        public void RemoveExerciseFromAllWorkouts(Exercise exercise, bool excludeOwnerWorkouts)
        {
            List<Workout> workouts = GetWorkoutsContainingExercise(exercise);

            if (excludeOwnerWorkouts)
                workouts = workouts.Where(workout => workout.UserId != exercise.UserId).ToList();

            foreach (var workout in workouts)
            {
                List<Exercise> workoutExercises = GetWorkoutExercises(workout);

                workoutExercises.RemoveAll(e => e.Id == exercise.Id);

                RemoveAllWorkoutExercises(workout);

                if (workoutExercises.Count != 0)
                    AddExercisesToWorkout(workout, workoutExercises);
            }
        }

        public void RemoveWorkout(Workout workout)
        {
            RemoveAllWorkoutExercises(workout);

            context.Workouts.Remove(workout);
            context.SaveChanges();
        }

        public void RemoveAllUserWorkouts(User user)
        {
            List<Workout> toBeRemoved = GetUserWorkouts(user);

            foreach (var workout in toBeRemoved)
                RemoveAllWorkoutExercises(workout);

            context.Workouts.RemoveRange(toBeRemoved);
            context.SaveChanges();
        }
    }
}
