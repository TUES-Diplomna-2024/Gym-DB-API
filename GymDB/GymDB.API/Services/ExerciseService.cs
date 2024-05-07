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
        private readonly IExerciseRepository exerciseRepository;
        private readonly IExerciseImageService exerciseImageService;
        private readonly IUserRepository userRepository;
        private readonly IRoleService roleService;

        public ExerciseService(IExerciseRepository exerciseRepository, IExerciseImageService exerciseImageService, IUserRepository userRepository, IRoleService roleService)
        {
            this.exerciseRepository = exerciseRepository;
            this.exerciseImageService = exerciseImageService;
            this.userRepository = userRepository;
            this.roleService = roleService;
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

        public async Task<ExerciseViewModel> GetExerciseViewByIdAsync(HttpContext context, Guid exerciseId)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);
            Exercise? exercise = await exerciseRepository.GetExerciseByIdAsync(exerciseId);

            if (exercise == null)
                throw new NotFoundException("The specified exercise could not be found!");

            if (IsExerciseCustom(exercise) && !IsExerciseOwnedByUser(exercise, currUser))
                throw new ForbiddenException("You cannot access custom exercises that are owned by another user!");

            // Public exercises turned into private can be only accessed by the root and other admins
            if (IsExercisePrivate(exercise) && !IsExerciseCustom(exercise) && roleService.IsUserNormie(currUser))
                throw new ForbiddenException("You don't have permission to access this exercise!");

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
                                    .Select(exercise => exercise.ToPreviewModel())
                                    .ToList();
        }

        public async Task<List<ExercisePreviewModel>> FindAdminCreatedExercisesPreviewsAsync(ExerciseSearchModel searchModel)
        {
            List<Exercise> matchingExercises = await exerciseRepository.FindAllExercisesMatchingNameAsync(searchModel.Name);

            // The root admin and all admin users can search for public and private exercises created by admins.

            return matchingExercises.Where(exercise => !IsExerciseCustom(exercise) && exercise.Visibility == searchModel.Visibility)
                                    .OrderByDescending(exercise => exercise.Name)
                                    .Select(exercise => exercise.ToPreviewModel())
                                    .ToList();
        }

        public async Task UpdateExerciseByIdAsync(HttpContext context, Guid exerciseId, ExerciseUpdateModel updateModel)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);
            Exercise? exercise = await exerciseRepository.GetExerciseByIdAsync(exerciseId);

            if (exercise == null)
                throw new NotFoundException("The specified exercise could not be found!");

            // Non-admin users cannot update public exercises
            if (IsExercisePublic(exercise) && roleService.IsUserNormie(currUser))
                throw new ForbiddenException("Only admin users can update public exercises!");

            if (IsExercisePrivate(exercise) && !IsExerciseCustom(exercise) && roleService.IsUserNormie(currUser))
                throw new ForbiddenException("You don't have permission to update this exercise!");

            // Only owners can update their custom exercises
            if (IsExerciseCustom(exercise) && !IsExerciseOwnedByUser(exercise, currUser))
                throw new ForbiddenException("You cannot update custom exercises that are owned by another user!");

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

            // TODO: Remove exercise from every workout in which it is included (public to private) OR show the exercise in different way

            exercise.Visibility = visibility;

            await exerciseRepository.UpdateExerciseAsync(exercise);
        }

        public async Task RemoveExerciseByIdAsync(HttpContext context, Guid exerciseId)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);
            Exercise? exercise = await exerciseRepository.GetExerciseByIdAsync(exerciseId);

            if (exercise == null)
                throw new NotFoundException("The specified exercise could not be found!");

            // Normal user can't remove public exercises
            if (IsExercisePublic(exercise) && roleService.IsUserNormie(currUser))
                throw new ForbiddenException("Only admin users can delete public exercises!");

            if (IsExercisePrivate(exercise) && !IsExerciseCustom(exercise) && roleService.IsUserNormie(currUser))
                throw new ForbiddenException("You don't have permission to delete this exercise!");

            // All users can't remove exercises that are custom and are not their own
            if (IsExerciseCustom(exercise) && !IsExerciseOwnedByUser(exercise, currUser))
                throw new ForbiddenException("You cannot delete custom exercises that are owned by another user!");

            await exerciseImageService.RemoveAllExerciseImagesAsync(exerciseId);

            // TODO: Remove exercise from every workout in which it is included
            // TODO: Remove all exercise records

            await exerciseRepository.RemoveExerciseAsync(exercise);
        }

        private bool IsExerciseOwnedByUser(Exercise exercise, User user)
            => exercise.OwnerId == user.Id;

        private bool IsExerciseCustom(Exercise exercise)
            => exercise.OwnerId != null;

        private bool IsExercisePrivate(Exercise exercise)
            => exercise.Visibility == ExerciseVisibility.Private;

        private bool IsExercisePublic(Exercise exercise)
            => exercise.Visibility == ExerciseVisibility.Public;
    }
}
