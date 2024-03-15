namespace GymDB.API.Models.Exercise
{
    public class ExercisePreviewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        
        public string Type { get; set; }

        public string Difficulty { get; set; }

        public string MuscleGroups { get; set; }

        public bool IsPrivate { get; set; }
    }
}
