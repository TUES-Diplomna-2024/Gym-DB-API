namespace GymDB.API.Models.Exercise
{
    public class ExerciseAdvancedInfoModel : ExerciseNormalInfoModel
    {
        public DateOnly OnCreated { get; set; }

        public DateTime OnModified { get; set; }
    }
}
