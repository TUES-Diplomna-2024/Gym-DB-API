using GymDB.API.Data.Entities;
using GymDB.API.Models.Exercise;
using GymDB.API.Repositories.Interfaces;
using GymDB.API.Services.Interfaces;
using GymDB.API.Exceptions;
using GymDB.API.Mappers;
using GymDB.API.Data.Enums;

namespace GymDB.API.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly IExerciseRepository exerciseRepository;
        private readonly IUserRepository userRepository;
        private readonly IRoleService roleService;

        public ExerciseService(IExerciseRepository exerciseRepository, IUserRepository userRepository, IRoleService roleService)
        {
            this.exerciseRepository = exerciseRepository;
            this.userRepository = userRepository;
            this.roleService = roleService;
        }

        public async Task CreateNewExerciseAsync(HttpContext context, ExerciseCreateModel createModel)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);

            /* Only the root admin and admin users can create public exercises */
            if (createModel.Visibility == ExerciseVisibility.Public && roleService.HasUserRole(currUser, "NORMIE"))
                throw new ForbiddenException("Users without admin permissions cannot create public еxercises!");

            Exercise exercise = createModel.ToEntity(currUser);

            await exerciseRepository.AddExerciseAsync(exercise);

            // TODO: Add Image Support
        }

        public async Task<ExerciseViewModel> GetExerciseViewByIdAsync(HttpContext context, Guid exerciseId)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);
            Exercise? exercise = await exerciseRepository.GetExerciseByIdAsync(exerciseId);

            if (exercise == null)
                throw new NotFoundException("The specified exercise could not be found!");

            if (IsExercisePrivate(exercise) && !IsExerciseOwnedByUser(exercise, currUser))
                throw new ForbiddenException("You cannot access custom exercises that are owned by another user!");

            // TODO: Get Image Uris and place them in the model

            return exercise.ToViewModel(null);
        }

        public async Task<List<ExercisePreviewModel>> SearchExercisesPreviewsAsync(HttpContext context, string name)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);

            List<Exercise> matchingExercises = await exerciseRepository.FindAllExercisesMatchingNameAsync(name);

            // Users have access to all public exercises and their custom ones

            return matchingExercises.Where(exercise => IsExercisePublic(exercise) || IsExerciseOwnedByUser(exercise, currUser))
                                    .OrderByDescending(exercise => exercise.OwnerId == currUser.Id)
                                    .Select(exercise => exercise.ToPreviewModel())
                                    .ToList();
        }

        public async Task<List<ExercisePreviewModel>> SearchExercisesPreviewsAsync(ExerciseSearchModel searchModel)
        {
            List<Exercise> matchingExercises = await exerciseRepository.FindAllExercisesMatchingNameAsync(searchModel.Name);

            // TODO: Update comment
            // The root admin and all admin users can search for public exercises and private which were created by them
            // Exercises that doesn't have Owner were created by user with admin permissions

            return matchingExercises.Where(exercise => exercise.OwnerId == null &&
                                                       (searchModel.Visibility == ExerciseVisibility.Public && IsExercisePublic(exercise) ||
                                                        searchModel.Visibility == ExerciseVisibility.Private && IsExercisePrivate(exercise)))
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
            if (IsExercisePublic(exercise) && roleService.HasUserRole(currUser, "NORMIE"))
                throw new ForbiddenException("Only admin users can update public exercises!");

            // Only owners can update their custom exercises
            if (IsExercisePrivate(exercise) && !IsExerciseOwnedByUser(exercise, currUser))
                throw new ForbiddenException("You cannot update custom exercises that are owned by another user!");

            exercise.ApplyUpdateModel(updateModel);

            // TODO: Add image support for updating exercise
            // TODO: Add change visibility option here too

            await exerciseRepository.UpdateExerciseAsync(exercise);
        }

        public async Task UpdateExerciseVisibilityAsync(Guid exerciseId, ExerciseVisibility visibility)
        {
            Exercise? exercise = await exerciseRepository.GetExerciseByIdAsync(exerciseId);

            if (exercise == null)
                throw new NotFoundException("The specified exercise could not be found!");

            if (exercise.Visibility == visibility)
                throw new ForbiddenException("The specified exercise already has this visibility!");

            // TODO: You can't change the visibility of custom exercises
            // TODO: Remove exercise from every workout in which it is included

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
            if (IsExercisePublic(exercise) && roleService.HasUserRole(currUser, "NORMIE"))
                throw new ForbiddenException("Only admin users can delete public exercises!");

            // TODO: Update comment
            // User can't remove exercises that are custom and are not their owner
            if (IsExercisePrivate(exercise) && exercise.OwnerId != null && !IsExerciseOwnedByUser(exercise, currUser))
                throw new ForbiddenException("You cannot delete custom exercises that are owned by another user!");

            // TODO: Add image support for removing exercise
            // TODO: Remove exercise from every workout in which it is included
            // TODO: Remove all exercise records

            await exerciseRepository.RemoveExerciseAsync(exercise);
        }

        private bool IsExerciseOwnedByUser(Exercise exercise, User user)
            => exercise.OwnerId == user.Id;

        private bool IsExercisePrivate(Exercise exercise)
            => exercise.Visibility == ExerciseVisibility.Private;

        private bool IsExercisePublic(Exercise exercise)
            => exercise.Visibility == ExerciseVisibility.Public;
    }
}
