using GymDB.API.Data.Entities;

namespace GymDB.API.Services.Interfaces
{
    public interface IExerciseService
    {
        Exercise? GetExerciseById(Guid id);

        void AddExercise(Exercise exercise);
    }
}
