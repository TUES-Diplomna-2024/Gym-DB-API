using ExerciseClass = GymDB.API.Data.Entities.Exercise;

namespace GymDB.API.Models.Exercise
{
    public class ExerciseAdvancedInfoModel : ExerciseNormalInfoModel
    {
        public ExerciseAdvancedInfoModel(ExerciseClass exercise) : base(exercise)
        {
            OnCreated = exercise.OnCreated;
            OnModified = exercise.OnModified;
        }

        public DateOnly OnCreated { get; set; }

        public DateTime OnModified { get; set; }
    }
}
