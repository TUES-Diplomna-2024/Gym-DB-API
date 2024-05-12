using GymDB.API.Models.Workout;

namespace GymDB.API.Services.Interfaces
{
    public interface IWorkoutService
    {
        Task CreateNewWorkoutAsync(HttpContext context, WorkoutCreateModel createModel);

        Task<WorkoutViewModel> GetWorkoutViewByIdAsync(HttpContext context, Guid workoutId);

        Task<List<WorkoutPreviewModel>> GetCurrUserWorkoutsPreviewsAsync(HttpContext context);

        Task UpdateWorkoutByIdAsync(HttpContext context, Guid workoutId, WorkoutUpdateModel updateModel);

        Task AddExerciseToWorkoutsAsync(HttpContext context, Guid exerciseId, List<Guid> workoutsIds);

        Task RemoveWorkoutByIdAsync(HttpContext context, Guid workoutId);
    }
}
