using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymDB.API.Repositories
{
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly ApplicationContext context;

        public ExerciseRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task<Exercise?> GetExerciseByIdAsync(Guid id)
        {
            return await context.Exercises
                                .Include(exercise => exercise.Owner)
                                .FirstOrDefaultAsync(exercise => exercise.Id == id);
        }

        public async Task<List<Exercise>> FindAllExercisesMatchingNameAsync(string name)
        {
            return await context.Exercises
                                .Include(exercise => exercise.Owner)
                                .Where(exercise => exercise.Name.ToLower().Contains(name.ToLower()))
                                .ToListAsync();
        }

        public async Task AddExerciseAsync(Exercise exercise)
        {
            context.Exercises.Add(exercise);
            await context.SaveChangesAsync();
        }

        public async Task UpdateExerciseAsync(Exercise exercise)
        {
            exercise.OnModified = DateTime.UtcNow;

            context.Exercises.Update(exercise);
            await context.SaveChangesAsync();
        }

        public async Task RemoveExerciseAsync(Exercise exercise)
        {
            context.Exercises.Remove(exercise);
            await context.SaveChangesAsync();
        }
    }
}
