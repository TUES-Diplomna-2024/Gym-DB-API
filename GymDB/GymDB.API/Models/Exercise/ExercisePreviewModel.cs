using GymDB.API.Data.Enums;

namespace GymDB.API.Models.Exercise
{
    public class ExercisePreviewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        
        public ExerciseType Type { get; set; }

        public ExerciseDifficulty Difficulty { get; set; }

        public string MuscleGroups { get; set; }

        public ExerciseVisibility Visibility { get; set; }
    }
}
