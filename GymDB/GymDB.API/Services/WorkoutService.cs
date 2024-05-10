using GymDB.API.Data.Entities;
using GymDB.API.Mappers;
using GymDB.API.Models.Workout;
using GymDB.API.Repositories.Interfaces;
using GymDB.API.Services.Interfaces;
using GymDB.API.Exceptions;
using GymDB.API.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace GymDB.API.Services
{
    public class WorkoutService : IWorkoutService
    {
        private readonly IExerciseService exerciseService;
        private readonly IExerciseRepository exerciseRepository;
        private readonly IWorkoutRepository workoutRepository;
        private readonly IUserRepository userRepository;

        public WorkoutService(IExerciseService exerciseService, IExerciseRepository exerciseRepository, IWorkoutRepository workoutRepository, IUserRepository userRepository)
        {
            this.exerciseService = exerciseService;
            this.exerciseRepository = exerciseRepository;
            this.workoutRepository = workoutRepository;
            this.userRepository = userRepository;
        }

        public async Task CreateNewWorkoutAsync(HttpContext context, WorkoutCreateModel createModel)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);
            
            Workout workout = createModel.ToEntity(currUser);
            
            await workoutRepository.AddWorkoutAsync(workout);
        }

        public async Task<WorkoutViewModel> GetWorkoutViewByIdAsync(HttpContext context, Guid workoutId)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);
            Workout? workout = await workoutRepository.GetWorkoutByIdAsync(workoutId);

            if (workout == null)
                throw new NotFoundException("The specified workout could not be found!");

            if (!IsWorkoutOwnedByUser(workout, currUser))
                throw new ForbiddenException("You cannot access workouts that are owned by another user!");

            // TODO: Get workout's exercises and add them in the view

            return workout.ToViewModel(null);
        }

        public async Task<List<WorkoutPreviewModel>> GetCurrUserWorkoutsPreviewsAsync(HttpContext context)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);

            List<Workout> workouts = await workoutRepository.GetAllUserWorkoutsAsync(currUser.Id);

            return workouts.Select(workout => workout.ToPreviewModel()).ToList();
        }

        public async Task UpdateWorkoutByIdAsync(HttpContext context, Guid workoutId, WorkoutUpdateModel updateModel)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);
            Workout? workout = await workoutRepository.GetWorkoutByIdAsync(workoutId);

            if (workout == null)
                throw new NotFoundException("The specified workout could not be found!");

            if (!IsWorkoutOwnedByUser(workout, currUser))
                throw new ForbiddenException("You cannot update workouts that are owned by another user!");

            workout.ApplyUpdateModel(updateModel);

            // TODO: Update workout exercises - they come as the new order of the workout exercises
            // if (!updateModel.Exercises.IsNullOrEmpty())

            await workoutRepository.UpdateWorkoutAsync(workout);
        }

        public async Task AddExerciseToWorkoutsAsync(HttpContext context, Guid exerciseId, List<Guid> workoutIds)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);
            Exercise? exercise = await exerciseRepository.GetExerciseByIdAsync(exerciseId);

            exerciseService.VerifyUserCanAddExerciseToTheirWorkouts(exercise, currUser);

            // TODO: Finish
        }

        private bool IsWorkoutOwnedByUser(Workout workout, User user)
            => workout.OwnerId == user.Id;
    }
}
