namespace GymDB.API.Models.Exercise
{
    public class ExerciseNormalInfoModel
    {   
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Instructions { get; set; }

        public string MuscleGroups { get; set; }

        public string Type { get; set; }

        public string Difficulty { get; set; }

        public string? Equipment { get; set; }

        public bool IsPrivate { get; set; }

        public Guid? OwnerId { get; set; }

        public string? OwnerUsername { get; set; }
    }
}
