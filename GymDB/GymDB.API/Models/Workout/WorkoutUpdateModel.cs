namespace GymDB.API.Models.Workout
{
    public class WorkoutUpdateModel : WorkoutCreateModel
    {
        public List<Guid>? ExercisesIds { get; set; }
    }
}
