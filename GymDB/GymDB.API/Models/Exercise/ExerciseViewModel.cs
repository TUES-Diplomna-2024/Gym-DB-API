using GymDB.API.Data.Enums;

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

        public List<Uri>? ImageUris { get; set; }

        public Guid? OwnerId { get; set; }

        public string? OwnerUsername { get; set; }
    }
}
