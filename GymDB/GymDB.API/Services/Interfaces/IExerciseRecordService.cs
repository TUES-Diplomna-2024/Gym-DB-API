using GymDB.API.Data.Entities;

namespace GymDB.API.Services.Interfaces
{
    public interface IExerciseRecordService
    {
        void AddExercise(ExerciseRecord record);
    }
}
