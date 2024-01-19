using ExerciseClass = GymDB.API.Data.Entities.Exercise;

namespace GymDB.API.Models.Exercise
{
    public class ExercisePreviewModel
    {
        public ExercisePreviewModel(ExerciseClass exercise)
        {
            Id           = exercise.Id;
            Name         = exercise.Name;
            Type         = exercise.Type;
            Difficulty   = exercise.Difficulty;
            MuscleGroups = exercise.MuscleGroups;
            IsPrivate    = exercise.IsPrivate;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }
        
        public string Type { get; set; }

        public string Difficulty { get; set; }

        public string MuscleGroups { get; set; }

        public bool IsPrivate { get; set; }
    }
}
