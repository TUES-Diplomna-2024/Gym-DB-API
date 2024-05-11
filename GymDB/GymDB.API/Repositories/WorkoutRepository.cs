using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymDB.API.Repositories
{
    public class WorkoutRepository : IWorkoutRepository
    {
        private readonly ApplicationContext context;

        public WorkoutRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task<Workout?> GetWorkoutByIdAsync(Guid id)
        {
            return await context.Workouts
                                .Include(workout => workout.Owner)
                                .FirstOrDefaultAsync(workout => workout.Id == id);
        }

        public async Task<List<Workout>> GetWorkoutRangeAsync(List<Guid> workoutsIds)
        {
            return await context.Workouts
                                .Include(workout => workout.Owner)
                                .Where(workout => workoutsIds.Contains(workout.Id))
                                .ToListAsync();
        }

        public async Task<List<Workout>> GetAllUserWorkoutsAsync(Guid userId)
        {
            return await context.Workouts
                                .Include(workout => workout.Owner)
                                .Where(workout => workout.OwnerId == userId)
                                .OrderByDescending(workout => workout.OnModified)
                                .ThenBy(workout => workout.Name)
                                .ToListAsync();
        }

        public async Task AddWorkoutAsync(Workout workout)
        {
            context.Workouts.Add(workout);
            await context.SaveChangesAsync();
        }

        public async Task UpdateWorkoutAsync(Workout workout)
        {
            workout.OnModified = DateTime.UtcNow;

            context.Workouts.Update(workout);
            await context.SaveChangesAsync();
        }

        public async Task RemoveWorkoutAsync(Workout workout)
        {
            context.Workouts.Remove(workout);
            await context.SaveChangesAsync();
        }
    }
}
