using Microsoft.IdentityModel.Tokens;
using GymDB.API.Data.Entities;
using GymDB.API.Mappers;
using GymDB.API.Models.Exercise;
using GymDB.API.Repositories.Interfaces;
using GymDB.API.Services.Interfaces;
using GymDB.API.Exceptions;

namespace GymDB.API.Services
{
    public class WorkoutExerciseService : IWorkoutExerciseService
    {
        private readonly IWorkoutExerciseRepository workoutExerciseRepository;
        private readonly IWorkoutRepository workoutRepository;

        public WorkoutExerciseService(IWorkoutExerciseRepository workoutExerciseRepository, IWorkoutRepository workoutRepository)
        {
            this.workoutExerciseRepository = workoutExerciseRepository;
            this.workoutRepository = workoutRepository;
        }

        public async Task<List<ExercisePreviewModel>> GetWorkoutExercisesPreviewsAsync(Guid workoutId)
        {
            List<WorkoutExercise> wExercises = await workoutExerciseRepository.GetAllWorkoutExercisesByWorkoutIdAsync(workoutId);

            return wExercises.Select(we => we.Exercise.ToPreviewModel()).ToList();
        }

        public async Task AppendExerciseToWorkoutAsync(Workout workout, Exercise exercise)
        {
            WorkoutExercise wExercise = exercise.ToWorkoutExerciseEntity(workout, workout.ExerciseCount++);
            await workoutExerciseRepository.AddWorkoutExerciseAsync(wExercise);
        }

        public async Task UpdateWorkoutExercisesAsync(Workout workout, List<Guid>? exercisesIds)
        {
            if (workout.ExerciseCount == 0 && exercisesIds.IsNullOrEmpty())
                return;

            // Validate if all exercisesIds are ids of exercises contained in the workout
            if (exercisesIds != null && !await workoutExerciseRepository.AreAllExercisesInWorkoutAsync(workout.Id, exercisesIds))
                throw new ForbiddenException("One or more specified exercises are not present in the workout!");

            // Store new positions for each exercise in the workout
            // Note: An exercise can be listed more than once in the workout
            Dictionary<Guid, Queue<int>> newPositions = new();

            if (exercisesIds != null)
            {
                for (int i = 0; i < exercisesIds.Count; i++)
                {
                    if (!newPositions.ContainsKey(exercisesIds[i]))
                        newPositions[exercisesIds[i]] = new Queue<int>();

                    newPositions[exercisesIds[i]].Enqueue(i);
                }
            }

            List<WorkoutExercise> origin = await workoutExerciseRepository.GetAllWorkoutExercisesByWorkoutIdAsync(workout.Id);
            List<WorkoutExercise> wExercisesToBeRemoved = new();
            List<WorkoutExercise> wExercisesToBeUpdated = new();

            for (int i = origin.Count - 1; i >= 0; i--)
            {
                // If exercise is not present in the new positions, mark it for removal
                if (!newPositions.ContainsKey(origin[i].ExerciseId))
                {
                    wExercisesToBeRemoved.Add(origin[i]);
                }
                else
                {
                    int newPosition = newPositions[origin[i].ExerciseId].Dequeue();

                    // Update exercise position and mark it for update if needed 
                    if (origin[i].Position != newPosition)
                    {
                        origin[i].Position = newPosition;
                        wExercisesToBeUpdated.Add(origin[i]);
                    }

                    // Remove exercise id from newPositions if all positions for it are assigned
                    if (newPositions[origin[i].ExerciseId].Count == 0)
                        newPositions.Remove(origin[i].ExerciseId);
                }
            }

            // If there are exercises in new positions not assigned, it means that an exercise originally part of the workout 
            // is passed more times than it was contained in the workout
            if (newPositions.Count != 0)
                throw new ForbiddenException("One or more specified exercises are not present in the workout!");

            // Nothing was changed
            if (wExercisesToBeRemoved.Count == 0 && wExercisesToBeUpdated.Count == 0)
                return;

            // Remove and update marked exercises
            await workoutExerciseRepository.RemoveWorkoutExerciseRangeAsync(wExercisesToBeRemoved);
            await workoutExerciseRepository.UpdateWorkoutExerciseRangeAsync(origin);

            workout.ExerciseCount = exercisesIds?.Count ?? 0;
        }

        public async Task RemoveAllWorkoutExercisesAsync(Workout workout)
        {
            await workoutExerciseRepository.RemoveAllWorkoutExercisesByWorkoutIdAsync(workout.Id);
            workout.ExerciseCount = 0;
        }

        public async Task RemoveExerciseFromAllWorkoutsAsync(Guid exerciseId)
        {
            // Get all workout exercises containing the specified exercise
            List<WorkoutExercise> wExercisesToBeRemoved = await workoutExerciseRepository.GetAllWorkoutExercisesByExerciseIdAsync(exerciseId);

            List<Workout> affectedWorkouts = wExercisesToBeRemoved.Select(we => we.Workout)
                                                                  .DistinctBy(workout => workout.Id)
                                                                  .ToList();

            await workoutExerciseRepository.RemoveWorkoutExerciseRangeAsync(wExercisesToBeRemoved);

            // Update affected workouts
            foreach (var workout in affectedWorkouts)
            {
                // Get all workout exercises for the current workout
                List<WorkoutExercise> wExercises = await workoutExerciseRepository.GetAllWorkoutExercisesByWorkoutIdAsync(workout.Id);

                // Update exercise count for the workout
                workout.ExerciseCount = wExercises.Count;
                await workoutRepository.UpdateWorkoutAsync(workout);

                // Update positions of remaining exercises in the workout
                for (int i = 0; i < wExercises.Count - 1; i++)
                {
                    if (i == 0 && wExercises[i].Position != 0)
                        wExercises[i].Position = 0;

                    if (wExercises[i + 1].Position - wExercises[i].Position > 1)
                        wExercises[i + 1].Position = wExercises[i].Position + 1;
                }

                await workoutExerciseRepository.UpdateWorkoutExerciseRangeAsync(wExercises);
            }
        }
    }
}
