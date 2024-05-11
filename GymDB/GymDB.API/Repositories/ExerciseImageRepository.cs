using Microsoft.EntityFrameworkCore;
using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Repositories.Interfaces;

namespace GymDB.API.Repositories
{
    public class ExerciseImageRepository : IExerciseImageRepository
    {
        private readonly ApplicationContext context;

        public ExerciseImageRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task<List<ExerciseImage>> GetAllExerciseImagesByExerciseIdAsync(Guid exerciseId)
        {
            return await context.ExerciseImages
                                .Include(exerciseImage => exerciseImage.Exercise)
                                .Where(exerciseImage => exerciseImage.ExerciseId == exerciseId)
                                .OrderBy(exerciseImage => exerciseImage.Position)
                                .ToListAsync();
        }

        public async Task AddExerciseImageAsync(ExerciseImage exerciseImage)
        {
            context.ExerciseImages.Add(exerciseImage);
            await context.SaveChangesAsync();
        }

        public async Task UpdateExerciseImagePossitionAsync(ExerciseImage exerciseImage, uint possition)
        {
            exerciseImage.Position = (int)possition;

            context.ExerciseImages.Update(exerciseImage);
            await context.SaveChangesAsync();
        }

        public async Task RemoveExerciseImageAsync(ExerciseImage exerciseImage)
        {
            context.ExerciseImages.Remove(exerciseImage);
            await context.SaveChangesAsync();
        }

        public async Task RemoveExerciseImageRangeAsync(List<ExerciseImage> exerciseImages)
        {
            context.ExerciseImages.RemoveRange(exerciseImages);
            await context.SaveChangesAsync();
        }
    }
}
