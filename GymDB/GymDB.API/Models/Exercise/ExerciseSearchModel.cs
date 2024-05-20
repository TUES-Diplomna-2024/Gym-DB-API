using GymDB.API.Data.Enums;

namespace GymDB.API.Models.Exercise
{
    public class ExerciseSearchModel
    {
        public string Name { get; set; }

        public ExerciseVisibility Visibility { get; set; }
    }
}
