using GymDB.API.Data.Entities;
using GymDB.API.Exceptions;
using GymDB.API.Mappers;
using GymDB.API.Models.ExerciseImage;
using GymDB.API.Repositories.Interfaces;
using GymDB.API.Services.Interfaces;

namespace GymDB.API.Services
{
    public class ExerciseImageService : IExerciseImageService
    {
        private readonly IAzureBlobService azureBlobService;
        private readonly IExerciseImageRepository exerciseImageRepository;
        private readonly IExerciseRepository exerciseRepository;

        public ExerciseImageService(IAzureBlobService azureBlobService, IExerciseImageRepository exerciseImageRepository, IExerciseRepository exerciseRepository)
        {
            this.azureBlobService = azureBlobService;
            this.exerciseImageRepository = exerciseImageRepository;
            this.exerciseRepository = exerciseRepository;
        }

        public async Task<List<ExerciseImageViewModel>> GetExerciseImagesViewsAsync(Guid exerciseId)
        {
            List<ExerciseImage> exerciseImages = await exerciseImageRepository.GetAllExerciseImagesByExerciseIdAsync(exerciseId);

            return exerciseImages.Select(ei => ei.ToViewModel(azureBlobService.GetExerciseImageUri(exerciseId, ei.Id)))
                                 .ToList();
        }

        public async Task AddImagesToExerciseAsync(Exercise exercise, List<IFormFile> images)
        {
            int lastPosition = exercise.ImageCount;

            // Filter all files that are not images
            int removedCount = images.RemoveAll(file => !azureBlobService.IsFileAllowedInContainer(file));

            try
            {
                foreach (var image in images)
                {
                    ExerciseImage exerciseImage = exercise.ToExerciseImageEntity(lastPosition++);

                    await azureBlobService.UploadExerciseImageAsync(exercise.Id, exerciseImage.Id, image);
                    await exerciseImageRepository.AddExerciseImageAsync(exerciseImage);
                }
            } catch
            {
                lastPosition--;
                throw;
            } finally
            {
                if (exercise.ImageCount != lastPosition)
                {
                    exercise.ImageCount = lastPosition;
                    await exerciseRepository.UpdateExerciseAsync(exercise);
                }
            }

            if (removedCount != 0)
                throw new OkException($"{removedCount} / {images.Count + removedCount} file(s) were not uploaded because they are either invalid image types or their size is too big!");
        }

        public async Task RemoveExerciseImagesAsync(Exercise exercise, List<Guid> exerciseImagesIds)
        {
            List<ExerciseImage> exerciseImages = await exerciseImageRepository.GetAllExerciseImagesByExerciseIdAsync(exercise.Id);
            List<ExerciseImage> toBeRemoved = exerciseImages.Where(exerciseImage => exerciseImagesIds.Contains(exerciseImage.Id)).ToList();
            List<Guid> removedIds = new List<Guid>();

            try
            {
                foreach (var exerciseImage in toBeRemoved)
                {
                    await azureBlobService.DeleteExerciseImageAsync(exercise.Id, exerciseImage.Id);  // Delete from cloud
                    await exerciseImageRepository.RemoveExerciseImageAsync(exerciseImage);  // Delete from database
                    removedIds.Add(exerciseImage.Id);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                // Remove all exercise images that were removed successfully
                exerciseImages.RemoveAll(exerciseImage => removedIds.Contains(exerciseImage.Id));

                // Update possitions if something was removed
                if (exercise.ImageCount != exerciseImages.Count)
                {
                    for (int i = 0; i < exerciseImages.Count - 1; i++)
                    {
                        if (i == 0 && exerciseImages[i].Position != 0)
                        {
                            await exerciseImageRepository.UpdateExerciseImagePossitionAsync(exerciseImages[i], 0);
                        }

                        if (exerciseImages[i + 1].Position - exerciseImages[i].Position > 1)
                        {
                            await exerciseImageRepository.UpdateExerciseImagePossitionAsync(exerciseImages[i + 1], (uint)exerciseImages[i].Position + 1);
                        }
                    }

                    exercise.ImageCount = exerciseImages.Count;
                    await exerciseRepository.UpdateExerciseAsync(exercise);
                }
            }
        }
    
        public async Task RemoveAllExerciseImagesAsync(Guid exerciseId)
        {
            List<ExerciseImage> exerciseImages = await exerciseImageRepository.GetAllExerciseImagesByExerciseIdAsync(exerciseId);

            await exerciseImageRepository.RemoveExerciseImageRangeAsync(exerciseImages);  // Delete all exercise images from database

            // TODO: If something fails take in consideration how to delete all images from the cloud that could not be deleted at the time

            foreach (var exerciseImage in exerciseImages)
            {
                await azureBlobService.DeleteExerciseImageAsync(exerciseId, exerciseImage.Id);  // Delete from cloud
            }
        }
    }
}
