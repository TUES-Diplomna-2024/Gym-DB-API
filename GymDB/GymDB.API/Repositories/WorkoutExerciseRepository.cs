using Microsoft.EntityFrameworkCore;
using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Repositories.Interfaces;

namespace GymDB.API.Repositories
{
    public class WorkoutExerciseRepository : IWorkoutExerciseRepository
    {
        private readonly ApplicationContext context;

        public WorkoutExerciseRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task<List<WorkoutExercise>> GetAllWorkoutExercisesByWorkoutIdAsync(Guid workoutId)
        {
            return await context.WorkoutsExercises
                                .Include(we => we.Workout)
                                .Include(we => we.Exercise)
                                .Where(we => we.WorkoutId == workoutId)
                                .OrderBy(we => we.Position)
                                .ToListAsync();
        }

        public async Task<List<WorkoutExercise>> GetAllWorkoutExercisesByExerciseIdAsync(Guid exerciseId)
        {
            return await context.WorkoutsExercises
                                .Include(we => we.Workout)
                                .Include(we => we.Exercise)
                                .Where(we => we.ExerciseId == exerciseId)
                                .ToListAsync();
        }

        public async Task<bool> AreAllExercisesInWorkoutAsync(Guid workoutId, List<Guid> exercisesIds)
        {
            var workoutExercisesIds = await context.WorkoutsExercises
                                                   .Where(we => we.WorkoutId == workoutId)
                                                   .Select(we => we.ExerciseId)
                                                   .ToListAsync();

            return exercisesIds.All(id => workoutExercisesIds.Contains(id));
        }

        public async Task AddWorkoutExerciseAsync(WorkoutExercise workoutExercise)
        {
            context.WorkoutsExercises.Add(workoutExercise);
            await context.SaveChangesAsync();
        }
        public async Task UpdateWorkoutExerciseRangeAsync(List<WorkoutExercise> workoutExercises)
        {
            context.WorkoutsExercises.UpdateRange(workoutExercises);
            await context.SaveChangesAsync();
        }

        public async Task RemoveWorkoutExerciseRangeAsync(List<WorkoutExercise> workoutExercises)
        {
            context.WorkoutsExercises.RemoveRange(workoutExercises);
            await context.SaveChangesAsync();
        }

        public async Task RemoveAllWorkoutExercisesByWorkoutIdAsync(Guid workoutId)
        {
            List<WorkoutExercise> toBeRemoved = await GetAllWorkoutExercisesByWorkoutIdAsync(workoutId);
            await RemoveWorkoutExerciseRangeAsync(toBeRemoved);
        }
    }
}
