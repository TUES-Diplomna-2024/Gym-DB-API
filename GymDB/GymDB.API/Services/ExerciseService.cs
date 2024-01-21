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

        public List<Exercise> GetAllPrivateAppExercises()
            => context.Exercises.Include(exercise => exercise.User)
                                .Where(exercise => exercise.IsPrivate && exercise.UserId == null)
                                .ToList();

        public List<Exercise> GetAllUserCustomExercises(User user)
            => context.Exercises.Include(exercise => exercise.User)
                                .Where(exercise => exercise.UserId == user.Id)
                                .ToList();

        public List<Exercise> GetAllUserPublicExercises(User user)
            => context.Exercises.Include(exercise => exercise.User)
                                .Where(exercise => !exercise.IsPrivate && exercise.UserId == user.Id)
                                .ToList();

        public List<Exercise> GetAllUserPrivateExercises(User user)
            => context.Exercises.Include(exercise => exercise.User)
                                .Where(exercise => exercise.IsPrivate && exercise.UserId == user.Id)
                                .ToList();

        public List<Exercise> GetUserAccessibleExercisesByIds(List<Guid> ids, User user)
        {
            List<Exercise> result = new List<Exercise>();

            foreach(var id in ids)
            {
                Exercise? exercise = GetExerciseById(id);
                
                if (exercise != null && (!exercise.IsPrivate || exercise.UserId == user.Id))
                    result.Add(exercise);
            }

            return result;                
        }

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

        public void UpdateExercise(Exercise exercise, ExerciseUpdateModel update)
        {
            exercise.Name         = update.Name;
            exercise.Instructions = update.Instructions;
            exercise.MuscleGroups = update.MuscleGroups;
            exercise.Type         = update.Type;
            exercise.Difficulty   = update.Difficulty;
            exercise.Equipment    = update.Equipment;

            UpdateExercise(exercise);
        }

        public void UpdateExercise(Exercise exercise)
        {
            exercise.OnModified = DateTime.UtcNow;

            context.Exercises.Update(exercise);
            context.SaveChanges();
        }

        public void RemoveExercise(Exercise exercise)
        {
            // TODO: Remove exercise from every workout & Update workouts exercise count

            context.Exercises.Remove(exercise);
            context.SaveChanges();
        }

        public void RemoveAllUserPrivateExercises(User user)
        {
            List<Exercise> toBeRemoved = GetAllUserPrivateExercises(user);
            context.Exercises.RemoveRange(toBeRemoved);
            context.SaveChanges();
        }

        public void RemoveUserOwnershipOfPublicExercises(User user)
        {
            List<Exercise> publicExercises = GetAllUserPublicExercises(user);

            foreach (var exercise in publicExercises)
            {
                exercise.UserId = null;
                exercise.User = null;
            }

            context.Exercises.UpdateRange(publicExercises);
            context.SaveChanges();
        }
    }
}
