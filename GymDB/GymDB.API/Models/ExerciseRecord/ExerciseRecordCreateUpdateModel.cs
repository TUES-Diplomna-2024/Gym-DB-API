using System.ComponentModel.DataAnnotations;

namespace GymDB.API.Models.ExerciseRecord
{
    public class ExerciseRecordCreateUpdateModel
    {
        public uint Sets { get; set; }

        public uint Reps { get; set; }

        [Range(minimum: 0, maximum: 300, ErrorMessage = "Weight must be between 0 and 300 kg!")]
        public double? Weight { get; set; }

        public uint Duration { get; set; }
    }
}
