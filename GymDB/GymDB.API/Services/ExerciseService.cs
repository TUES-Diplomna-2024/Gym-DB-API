using Microsoft.IdentityModel.Tokens;
using GymDB.API.Data.Entities;
using GymDB.API.Models.Exercise;
using GymDB.API.Repositories.Interfaces;
using GymDB.API.Services.Interfaces;
using GymDB.API.Exceptions;
using GymDB.API.Mappers;
using GymDB.API.Data.Enums;
using GymDB.API.Models.ExerciseImage;

namespace GymDB.API.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly IExerciseImageService exerciseImageService;
        private readonly IWorkoutExerciseService workoutExerciseService;
        private readonly IRoleService roleService;
        private readonly IExerciseRepository exerciseRepository;
        private readonly IExerciseRecordRepository exerciseRecordRepository;
        private readonly IUserRepository userRepository;

        public ExerciseService(IExerciseImageService exerciseImageService, IWorkoutExerciseService workoutExerciseService, IRoleService roleService, IExerciseRepository exerciseRepository, IExerciseRecordRepository exerciseRecordRepository, IUserRepository userRepository)
        {
            this.exerciseImageService = exerciseImageService;
            this.workoutExerciseService = workoutExerciseService;
            this.roleService = roleService;
            this.exerciseRepository = exerciseRepository;
            this.exerciseRecordRepository = exerciseRecordRepository;
            this.userRepository = userRepository;
        }

        public async Task CreateNewExerciseAsync(HttpContext context, ExerciseCreateModel createModel)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);

            // Only the root admin and admin users can create public exercises
            if (createModel.Visibility == ExerciseVisibility.Public && roleService.IsUserNormie(currUser))
                throw new ForbiddenException("Users without admin permissions cannot create public еxercises!");

            Exercise exercise = createModel.ToEntity(currUser);

            await exerciseRepository.AddExerciseAsync(exercise);

            if (createModel.Images != null)
                await exerciseImageService.AddImagesToExerciseAsync(exercise, createModel.Images);
        }

        public async Task<Exercise> GetExerciseByIdAsync(User user, Guid exerciseId, ExerciseValidation validation = ExerciseValidation.Access)
        {
            Exercise? exercise = await exerciseRepository.GetExerciseByIdAsync(exerciseId);

            if (exercise == null)
                throw new NotFoundException("The specified exercise could not be found!");

            // All users can't access exercises that are custom and not their own
            if (IsExerciseCustom(exercise) && !IsExerciseOwnedByUser(exercise, user))
                throw new ForbiddenException("You cannot access custom exercises that are owned by another user!");

            switch (validation)
            {
                case ExerciseValidation.AdditionToWorkouts:
                    if (IsExercisePrivateAdminCreated(exercise))
                    {
                        throw new ForbiddenException("You cannot add this exercise in your workouts!");
                    }

                    break;
                default:  // Access or Modification cases
                    // Public exercises turned into private can be only accessed by the root and other admins
                    if (IsExercisePrivateAdminCreated(exercise) && roleService.IsUserNormie(user))
                    {
                        throw new ForbiddenException("You don't have permission to access this exercise!");
                    }

                    // Normal user can't update/remove public exercises
                    if (validation == ExerciseValidation.Modification && IsExercisePublic(exercise) && roleService.IsUserNormie(user))
                        throw new ForbiddenException("Only admin users can update/delete public exercises!");

                    break;
            }

            return exercise;
        }

        public async Task<ExerciseViewModel> GetExerciseViewByIdAsync(HttpContext context, Guid exerciseId)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);
            
            Exercise exercise = await GetExerciseByIdAsync(currUser, exerciseId);
            List<ExerciseImageViewModel> images = await exerciseImageService.GetExerciseImagesViewsAsync(exerciseId);

            return exercise.ToViewModel(images);
        }

        public async Task<List<ExercisePreviewModel>> GetCurrUserCustomExercisesPreviewsAsync(HttpContext context)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);

            List<Exercise> customExercises = await exerciseRepository.GetAllUserCustomExercisesAsync(currUser.Id);

            return customExercises.Select(exercise => exercise.ToPreviewModel()).ToList();
        }

        public async Task<List<ExercisePreviewModel>> FindPublicAndCustomExercisesPreviewsAsync(HttpContext context, string name)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);

            List<Exercise> matchingExercises = await exerciseRepository.FindAllExercisesMatchingNameAsync(name);

            // Users have access to all public exercises and their custom ones

            return matchingExercises.Where(exercise => IsExercisePublic(exercise) || IsExerciseOwnedByUser(exercise, currUser))
                                    .OrderByDescending(exercise => exercise.OwnerId == currUser.Id)  // Place user's own exercises first
                                    .ThenBy(exercise => exercise.Name)
                                    .Select(exercise => exercise.ToPreviewModel())
                                    .ToList();
        }

        public async Task<List<ExercisePreviewModel>> FindAdminCreatedExercisesPreviewsAsync(ExerciseSearchModel searchModel)
        {
            List<Exercise> matchingExercises = await exerciseRepository.FindAllExercisesMatchingNameAsync(searchModel.Name);

            // The root admin and all admin users can search for public and private exercises created by admins.

            return matchingExercises.Where(exercise => !IsExerciseCustom(exercise) && exercise.Visibility == searchModel.Visibility)
                                    .OrderBy(exercise => exercise.Name)
                                    .Select(exercise => exercise.ToPreviewModel())
                                    .ToList();
        }

        public async Task UpdateExerciseByIdAsync(HttpContext context, Guid exerciseId, ExerciseUpdateModel updateModel)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);
            Exercise exercise = await GetExerciseByIdAsync(currUser, exerciseId, ExerciseValidation.Modification);

            exercise.ApplyUpdateModel(updateModel);
            await exerciseRepository.UpdateExerciseAsync(exercise);

            if (!updateModel.ImagesToBeRemoved.IsNullOrEmpty())
                await exerciseImageService.RemoveExerciseImagesAsync(exercise, updateModel.ImagesToBeRemoved!);

            if (!updateModel.ImagesToBeAdded.IsNullOrEmpty())
                await exerciseImageService.AddImagesToExerciseAsync(exercise, updateModel.ImagesToBeAdded!);
        }

        public async Task UpdateExerciseVisibilityAsync(Guid exerciseId, ExerciseVisibility visibility)
        {
            Exercise? exercise = await exerciseRepository.GetExerciseByIdAsync(exerciseId);

            if (exercise == null)
                throw new NotFoundException("The specified exercise could not be found!");

            if (IsExerciseCustom(exercise))
                throw new ForbiddenException("The visibility of custom exercises can't be changed!");

            if (exercise.Visibility == visibility)
                throw new ForbiddenException("The specified exercise already has this visibility!");

            await workoutExerciseService.RemoveExerciseFromAllWorkoutsAsync(exerciseId);

            exercise.Visibility = visibility;
            await exerciseRepository.UpdateExerciseAsync(exercise);
        }

        public async Task RemoveExerciseByIdAsync(HttpContext context, Guid exerciseId)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);
            Exercise exercise = await GetExerciseByIdAsync(currUser, exerciseId, ExerciseValidation.Modification);

            await RemoveExerciseAsync(exercise);
        }

        public async Task RemoveAllUserCustomExercisesAsync(Guid userId)
        {
            List<Exercise> customExercises = await exerciseRepository.GetAllUserCustomExercisesAsync(userId);

            foreach (var exercise in customExercises)
                await RemoveExerciseAsync(exercise);
        }

        private async Task RemoveExerciseAsync(Exercise exercise)
        {
            // Remove all related data associated with the exercise
            await exerciseImageService.RemoveAllExerciseImagesAsync(exercise.Id);  // images
            await exerciseRecordRepository.RemoveAllExerciseRecordsByExerciseIdAsync(exercise.Id);  // records
            await workoutExerciseService.RemoveExerciseFromAllWorkoutsAsync(exercise.Id);  // workout exercises

            await exerciseRepository.RemoveExerciseAsync(exercise);
        }

        private bool IsExerciseOwnedByUser(Exercise exercise, User user)
            => exercise.OwnerId == user.Id;

        private bool IsExerciseCustom(Exercise exercise)
            => exercise.OwnerId != null;

        private bool IsExercisePublic(Exercise exercise)
            => exercise.Visibility == ExerciseVisibility.Public;

        private bool IsExercisePrivate(Exercise exercise)
            => exercise.Visibility == ExerciseVisibility.Private;

        private bool IsExercisePrivateAdminCreated(Exercise exercise)
            => IsExercisePrivate(exercise) && !IsExerciseCustom(exercise);
    }
}
