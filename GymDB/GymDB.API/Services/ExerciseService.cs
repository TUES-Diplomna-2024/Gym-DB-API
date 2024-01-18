using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymDB.API.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly ApplicationContext context;

        public ExerciseService(ApplicationContext context)
        {
            this.context = context;
        }

        public Exercise? GetExerciseById(Guid id)
            => context.Exercises.Include(exercise => exercise.User)
                                .FirstOrDefault(exercise => exercise.Id == id);

        public void AddExercise(Exercise exercise)
        {
            exercise.Type = exercise.Type.ToLower().Replace(" ", "_");
            exercise.Difficulty = exercise.Difficulty.ToLower();

            context.Exercises.Add(exercise);
            context.SaveChanges();
        }
    }
}
