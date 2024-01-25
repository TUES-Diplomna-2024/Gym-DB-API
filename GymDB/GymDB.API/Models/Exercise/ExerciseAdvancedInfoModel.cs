using UserClass = GymDB.API.Data.Entities.User;
using ExerciseClass = GymDB.API.Data.Entities.Exercise;

namespace GymDB.API.Models.Exercise
{
    public class ExerciseAdvancedInfoModel : ExerciseNormalInfoModel
    {
        public ExerciseAdvancedInfoModel(ExerciseClass exercise, UserClass? owner) : base(exercise, owner)
        {
            OnCreated = exercise.OnCreated;
            OnModified = exercise.OnModified;
        }

        public DateOnly OnCreated { get; set; }

        public DateTime OnModified { get; set; }
    }
}
