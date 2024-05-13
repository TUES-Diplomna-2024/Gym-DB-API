using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GymDB.API.Data.Entities
{
    public class ExerciseRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public uint Sets { get; set; }

        public uint Reps { get; set; }

        [Range(minimum: 0, maximum: 300, ErrorMessage = "Weight must be between 0 and 300 kg!")]
        public double Weight { get; set; }

        public double Volume { get; set; }

        public uint Duration { get; set; }

        [ForeignKey(nameof(User))]
        public Guid OwnerId { get; set; }

        [ForeignKey(nameof(Exercise))]
        public Guid ExerciseId { get; set; }

        public DateTime OnCreated { get; set; }

        public DateTime OnModified { get; set; }
    }
}
