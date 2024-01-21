using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GymDB.API.Data.Entities
{
    public class WorkoutExercise
    {
        public WorkoutExercise() { }

        public WorkoutExercise(Workout workout, Exercise exercise, int position)
        {
            Id         = Guid.NewGuid();
            WorkoutId  = workout.Id;
            Workout    = workout;
            ExerciseId = exercise.Id;
            Exercise   = exercise;
            Position   = position;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [ForeignKey(nameof(Workout))]
        public Guid WorkoutId { get; set; }

        public Workout Workout { get; set; }

        [ForeignKey(nameof(Exercise))]
        public Guid ExerciseId { get; set; }

        public Exercise Exercise { get; set; }

        public int Position { get; set; }
    }
}
