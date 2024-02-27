namespace GymDB.API.Models.ExerciseRecord
{
    public class ExerciseRecordViewModel
    {
        public Guid Id { get; set; }

        public DateTime OnCreated { get; set; }

        public uint Sets { get; set; }

        public uint Reps { get; set; }

        public double Weight { get; set; }

        public uint Duration { get; set; }
    }
}
