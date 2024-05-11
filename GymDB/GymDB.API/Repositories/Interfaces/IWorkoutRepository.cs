using GymDB.API.Data.Entities;

namespace GymDB.API.Repositories.Interfaces
{
    public interface IWorkoutRepository
    {
        Task<Workout?> GetWorkoutByIdAsync(Guid id);

        Task<List<Workout>> GetWorkoutRangeAsync(List<Guid> workoutsIds);

        Task<List<Workout>> GetAllUserWorkoutsAsync(Guid userId);

        Task AddWorkoutAsync(Workout workout);

        Task UpdateWorkoutAsync(Workout workout);

        Task RemoveWorkoutAsync(Workout workout);
    }
}
