using GymDB.API.Data.Entities;
using GymDB.API.Models.Exercise;

namespace GymDB.API.Services.Interfaces
{
    public interface IExerciseService
    {
        List<Exercise> GetAllPublicExercises();

        List<Exercise> GetAllUserCustomExercises(User user);

        List<ExercisePreviewModel> GetExercisesPreviews(List<Exercise> exercises);

        Exercise? GetExerciseById(Guid id);

        void AddExercise(Exercise exercise);
    }
}
