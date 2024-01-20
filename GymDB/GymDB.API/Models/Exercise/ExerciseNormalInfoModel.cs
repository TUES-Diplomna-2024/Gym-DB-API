using ExerciseClass = GymDB.API.Data.Entities.Exercise;

namespace GymDB.API.Models.Exercise
{
    public class ExerciseNormalInfoModel
    {
        public ExerciseNormalInfoModel(ExerciseClass exercise)
        {
            Id           = exercise.Id;
            Name         = exercise.Name;
            Instructions = exercise.Instructions;
            MuscleGroups = exercise.MuscleGroups;
            Type         = exercise.Type;
            Difficulty   = exercise.Difficulty;
            Equipment    = exercise.Equipment;
            IsPrivate    = exercise.IsPrivate;
        }
        
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Instructions { get; set; }

        public string MuscleGroups { get; set; }

        public string Type { get; set; }

        public string Difficulty { get; set; }

        public string? Equipment { get; set; }

        public bool IsPrivate { get; set; }
    }
}
