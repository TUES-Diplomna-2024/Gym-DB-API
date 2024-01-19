using GymDB.API.Data.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace GymDB.API.Models.Exercise
{
    public class ExerciseCreateModel
    {
        [StringLength(130, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 130 characters long!")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Instructions must be up to 500 characters long!")]
        public string Instructions { get; set; }

        public string MuscleGroups { get; set; }

        [ExerciseType]
        public string Type { get; set; }

        [ExerciseDifficulty]
        public string Difficulty { get; set; }

        public string? Equipment { get; set; }

        public bool IsPrivate { get; set; }
    }
}
