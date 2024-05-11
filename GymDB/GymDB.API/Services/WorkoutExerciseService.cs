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

        public async Task UpdateWorkoutExercisesAsync(Workout workout, List<Guid>? exercisesIds)
        {
            if (workout.ExerciseCount == 0 && exercisesIds.IsNullOrEmpty())
                return;

            // Validate if all exercisesIds are ids of exercises contained in the workout
            if (exercisesIds != null && !await workoutExerciseRepository.AreAllExercisesInWorkoutAsync(workout.Id, exercisesIds))
                throw new ForbiddenException("Not all specified exercises are present in the workout!");

            await RemoveAllWorkoutExercisesAsync(workout);

            // TODO: Add exercises to workout
        }

        public async Task RemoveAllWorkoutExercisesAsync(Workout workout)
        {
            await workoutExerciseRepository.RemoveAllWorkoutExercisesByWorkoutIdAsync(workout.Id);

            workout.ExerciseCount = 0;
            await workoutRepository.UpdateWorkoutAsync(workout);
        }
    }
}
