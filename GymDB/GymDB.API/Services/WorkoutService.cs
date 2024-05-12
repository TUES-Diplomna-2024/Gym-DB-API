using GymDB.API.Data.Entities;
using GymDB.API.Mappers;
using GymDB.API.Models.Workout;
using GymDB.API.Repositories.Interfaces;
using GymDB.API.Services.Interfaces;
using GymDB.API.Exceptions;
using GymDB.API.Models.Exercise;
using GymDB.API.Data.Enums;

namespace GymDB.API.Services
{
    public class WorkoutService : IWorkoutService
    {
        private readonly IExerciseService exerciseService;
        private readonly IWorkoutExerciseService workoutExerciseService;
        private readonly IWorkoutRepository workoutRepository;
        private readonly IUserRepository userRepository;

        public WorkoutService(IExerciseService exerciseService, IWorkoutExerciseService workoutExerciseService, IWorkoutRepository workoutRepository, IUserRepository userRepository)
        {
            this.exerciseService = exerciseService;
            this.workoutExerciseService = workoutExerciseService;
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

            Workout workout = await GetWorkoutByIdAsync(currUser, workoutId);
            List<ExercisePreviewModel> exercises = await workoutExerciseService.GetWorkoutExercisesPreviewsAsync(workoutId);

            return workout.ToViewModel(exercises);
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
            Workout workout = await GetWorkoutByIdAsync(currUser, workoutId);

            await workoutExerciseService.UpdateWorkoutExercisesAsync(workout, updateModel.ExercisesIds);

            workout.ApplyUpdateModel(updateModel);
            await workoutRepository.UpdateWorkoutAsync(workout);
        }

        public async Task AddExerciseToWorkoutsAsync(HttpContext context, Guid exerciseId, List<Guid> workoutsIds)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);
            Exercise exercise = await exerciseService.GetExerciseByIdAsync(currUser, exerciseId, ExerciseValidation.AdditionToWorkouts);

            List<Workout> workoutsFound = await workoutRepository.GetWorkoutRangeAsync(workoutsIds);

            if (workoutsFound.Count != workoutsIds.Count)
            {
                string messageStart = workoutsFound.Count == 0 ? "None" : "Some";
                throw new NotFoundException($"{messageStart} of the specified workouts could not be found!");
            }

            if (workoutsFound.Any(workout => !IsWorkoutOwnedByUser(workout, currUser)))
                throw new ForbiddenException("You must own all specified workouts to add exercises to them!");

            foreach(var workout in workoutsFound)
            {
                await workoutExerciseService.AppendExerciseToWorkoutAsync(workout, exercise!);
            }

            await workoutRepository.UpdateWorkoutRangeAsync(workoutsFound);
        }

        public async Task RemoveWorkoutByIdAsync(HttpContext context, Guid workoutId)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);
            Workout workout = await GetWorkoutByIdAsync(currUser, workoutId);

            await workoutExerciseService.RemoveAllWorkoutExercisesAsync(workout);
            await workoutRepository.RemoveWorkoutAsync(workout);
        }

        private async Task<Workout> GetWorkoutByIdAsync(User user, Guid workoutId)
        {
            Workout? workout = await workoutRepository.GetWorkoutByIdAsync(workoutId);

            if (workout == null)
                throw new NotFoundException("The specified workout could not be found!");

            if (!IsWorkoutOwnedByUser(workout, user))
                throw new ForbiddenException("You cannot access workouts that are owned by another user!");

            return workout;
        }

        private bool IsWorkoutOwnedByUser(Workout workout, User user)
            => workout.OwnerId == user.Id;
    }
}
