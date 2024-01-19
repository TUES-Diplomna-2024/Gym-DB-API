using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Models.Exercise;
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

        public List<Exercise> GetAllPublicExercises()
            => context.Exercises.Include(exercise => exercise.User)
                                .Where(exercise => !exercise.IsPrivate)
                                .ToList();

        public List<Exercise> GetAllUserCustomExercises(User user)
            => context.Exercises.Include(exercise => exercise.User)
                                .Where(exercise => exercise.UserId == user.Id)
                                .ToList();

        public List<ExercisePreviewModel> GetExercisesPreviews(List<Exercise> exercises)
            => exercises.Select(exercise => new ExercisePreviewModel(exercise)).ToList();

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
