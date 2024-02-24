using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Mapping;
using GymDB.API.Models.Exercise;
using GymDB.API.Services.Interfaces;
using Hangfire;
using Hangfire.States;
using Hangfire.Storage;
using Microsoft.EntityFrameworkCore;

namespace GymDB.API.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly ApplicationContext context;
        private readonly IAzureBlobService azureBlobService;

        public ExerciseService(ApplicationContext context, IAzureBlobService azureBlobService)
        {
            this.context = context;
            this.azureBlobService = azureBlobService;
        }

        public List<Exercise> GetAllPublicExercises()
            => context.Exercises.Include(exercise => exercise.User)
                                .Where(exercise => !exercise.IsPrivate)
                                .ToList();

        public List<Exercise> GetAllPrivateAppExercises()
            => context.Exercises.Include(exercise => exercise.User)
                                .Where(exercise => exercise.IsPrivate && exercise.UserId == null)
                                .ToList();

        public List<Exercise> GetAllUserCustomExercises(User user)
            => context.Exercises.Include(exercise => exercise.User)
                                .Where(exercise => exercise.UserId == user.Id)
                                .OrderByDescending(exercise => exercise.OnModified)
                                .ToList();

        public List<Exercise> GetAllUserPublicExercises(User user)
            => context.Exercises.Include(exercise => exercise.User)
                                .Where(exercise => !exercise.IsPrivate && exercise.UserId == user.Id)
                                .ToList();

        public List<Exercise> GetAllUserPrivateExercises(User user)
            => context.Exercises.Include(exercise => exercise.User)
                                .Where(exercise => exercise.IsPrivate && exercise.UserId == user.Id)
                                .ToList();

        public List<Exercise> GetUserAccessibleExercisesByIds(List<Guid> ids, User user)
        {
            List<Exercise> result = new List<Exercise>();

            foreach(var id in ids)
            {
                Exercise? exercise = GetExerciseById(id);
                
                if (exercise != null && (!exercise.IsPrivate || exercise.UserId == user.Id))
                    result.Add(exercise);
            }

            return result;                
        }

        public List<Exercise> GetExercisesBySearch(ExerciseSearchModel search, User user)
            => context.Exercises.Include(exercise => exercise.User)
                                .Where(exercise => (!exercise.IsPrivate || exercise.UserId == user.Id) && 
                                                    exercise.Name.ToLower().Contains(search.Name.ToLower()))
                                .OrderByDescending(exercise => exercise.UserId == user.Id)
                                .ToList();

        public List<ExercisePreviewModel> GetExercisesPreviews(List<Exercise> exercises)
            => exercises.Select(exercise => exercise.ToPreviewModel()).ToList();

        public Exercise? GetExerciseById(Guid id)
            => context.Exercises.Include(exercise => exercise.User)
                                .FirstOrDefault(exercise => exercise.Id == id);

        public List<Uri>? GetExerciseImageUris(Exercise exercise)
        {
            if (exercise.ImageCount == 0)
                return null;

            List<Uri> imageUris = context.ExerciseImages.Include(ei => ei.Exercise)
                                                        .Where(ei => ei.ExerciseId == exercise.Id)
                                                        .OrderBy(ei => ei.Position)
                                                        .Select(ei => azureBlobService.GetBlobUri(ei)).ToList();
            return imageUris;
        }

        public bool IsExerciseOwnedByUser(Exercise exercise, User user)
            => exercise.UserId == user.Id;

        public void AddExercise(Exercise exercise)
        {
            exercise.Type = exercise.Type.ToLower().Replace(" ", "_");
            exercise.Difficulty = exercise.Difficulty.ToLower();

            context.Exercises.Add(exercise);
            context.SaveChanges();
        }

        public void AddImagesToExercise(Exercise exercise, List<IFormFile> images)
        {
            int lastPosition = exercise.ImageCount;

            List<ExerciseImage> exerciseImages = images.Select(image => exercise.ToExerciseImageEntity(Path.GetExtension(image.FileName), lastPosition++))
                                                       .ToList();

            List<Guid> notSaved = new List<Guid>();

            for (int i = 0; i < images.Count; i++)
            {
                bool isSaved = azureBlobService.UploadExerciseImage(exercise, images[i], exerciseImages[i].Id);

                if (!isSaved)
                    notSaved.Add(exerciseImages[i].Id);
            }

            if (notSaved.Count > 0)
            {
                exerciseImages.RemoveAll(ei => notSaved.Contains(ei.Id));

                lastPosition = exercise.ImageCount;

                foreach (var ei in exerciseImages)
                    ei.Position = lastPosition++;
            }

            exercise.ImageCount = lastPosition;

            context.Exercises.Update(exercise);
            context.ExerciseImages.AddRange(exerciseImages);

            context.SaveChanges();
        }

        public void UpdateExerciseVisibility(Exercise exercise, bool isPrivate)
        {
            // TODO: Finish removing the exercise from all workouts not owned by the owner of the exercise

            if (isPrivate) {
                string jobId = BackgroundJob.Enqueue<IWorkoutService>(workoutService => workoutService.RemoveExerciseFromAllWorkouts(exercise, true));

                using (var connection = JobStorage.Current.GetConnection())
                {
                    JobData jobData;
                    do
                    {
                        jobData = connection.GetJobData(jobId);
                        Thread.Sleep(2);
                    } while (jobData.State != SucceededState.StateName && jobData.State != DeletedState.StateName);
                }
            }

            exercise.IsPrivate = isPrivate;

            UpdateExercise(exercise);
        }

        public void UpdateExercise(Exercise exercise, ExerciseUpdateModel update)
        {
            exercise.Name         = update.Name;
            exercise.Instructions = update.Instructions;
            exercise.MuscleGroups = update.MuscleGroups;
            exercise.Type         = update.Type;
            exercise.Difficulty   = update.Difficulty;
            exercise.Equipment    = update.Equipment;

            UpdateExercise(exercise);
        }

        public void UpdateExercise(Exercise exercise)
        {
            exercise.OnModified = DateTime.UtcNow;

            context.Exercises.Update(exercise);
            context.SaveChanges();
        }

        public void RemoveExercise(Exercise exercise)
        {
            string jobId = BackgroundJob.Enqueue<IWorkoutService>(workoutService => workoutService.RemoveExerciseFromAllWorkouts(exercise, false));

            using (var connection = JobStorage.Current.GetConnection())
            {
                JobData jobData;
                do
                {
                    jobData = connection.GetJobData(jobId);
                    Thread.Sleep(2);
                } while (jobData.State != SucceededState.StateName && jobData.State != DeletedState.StateName);
            }

            RemoveAllExerciseImages(exercise);

            context.Exercises.Remove(exercise);
            context.SaveChanges();
        }

        public void RemoveAllUserPrivateExercises(User user)
        {
            List<Exercise> toBeRemoved = GetAllUserPrivateExercises(user);

            toBeRemoved.ForEach(exercise => RemoveAllExerciseImages(exercise));

            context.Exercises.RemoveRange(toBeRemoved);
            context.SaveChanges();
        }

        public void RemoveUserOwnershipOfPublicExercises(User user)
        {
            List<Exercise> publicExercises = GetAllUserPublicExercises(user);

            foreach (var exercise in publicExercises)
            {
                exercise.UserId = null;
                exercise.User = null;
            }

            context.Exercises.UpdateRange(publicExercises);
            context.SaveChanges();
        }

        public void RemoveAllExerciseImages(Exercise exercise)
        {
            List<ExerciseImage> toBeRemoved = context.ExerciseImages.Include(ei => ei.Exercise)
                                                                    .Where(ei => ei.ExerciseId == exercise.Id).ToList();

            toBeRemoved.ForEach(ei => azureBlobService.DeleteExerciseImage(ei));

            context.ExerciseImages.RemoveRange(toBeRemoved);

            exercise.ImageCount = 0;
            context.Exercises.Update(exercise);

            context.SaveChanges();
        }
    }
}
