using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Services.Interfaces;

namespace GymDB.API.Services
{
    public class ExerciseRecordService : IExerciseRecordService
    {
        private readonly ApplicationContext context;

        public ExerciseRecordService(ApplicationContext context)
        {
            this.context = context;
        }

        public void AddExercise(ExerciseRecord record)
        {
            context.ExerciseRecords.Add(record);
            context.SaveChanges();
        }
    }
}
