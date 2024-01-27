using GymDB.API.Data.Entities;
using GymDB.API.Models.Exercise;

namespace GymDB.API.Services.Interfaces
{
    public interface IExerciseService
    {
        List<Exercise> GetAllPublicExercises();

        List<Exercise> GetAllPrivateAppExercises();

        List<Exercise> GetAllUserCustomExercises(User user);

        List<Exercise> GetAllUserPublicExercises(User user);

        List<Exercise> GetAllUserPrivateExercises(User user);

        List<Exercise> GetUserAccessibleExercisesByIds(List<Guid> ids, User user);

        List<Exercise> GetExercisesBySearch(ExerciseSearchModel search, User user);

        List<ExercisePreviewModel> GetExercisesPreviews(List<Exercise> exercises);

        Exercise? GetExerciseById(Guid id);

        bool IsExerciseOwnedByUser(Exercise exercise, User user);

        void AddExercise(Exercise exercise);

        void UpdateExerciseVisibility(Exercise exercise, bool isPrivate);

        void UpdateExercise(Exercise exercise, ExerciseUpdateModel update);

        void UpdateExercise(Exercise exercise);

        void RemoveExercise(Exercise exercise);

        void RemoveAllUserPrivateExercises(User user);

        void RemoveUserOwnershipOfPublicExercises(User user);
    }
}
