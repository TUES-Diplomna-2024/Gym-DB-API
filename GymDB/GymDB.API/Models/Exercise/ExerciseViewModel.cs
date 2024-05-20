using GymDB.API.Data.Enums;
using GymDB.API.Models.ExerciseImage;

namespace GymDB.API.Models.Exercise
{
    public class ExerciseViewModel
    {   
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Instructions { get; set; }

        public string MuscleGroups { get; set; }

        public ExerciseType Type { get; set; }

        public ExerciseDifficulty Difficulty { get; set; }

        public string? Equipment { get; set; }

        public ExerciseVisibility Visibility { get; set; }

        public bool IsCustom { get; set; }

        public List<ExerciseImageViewModel>? Images { get; set; }
    }
}
