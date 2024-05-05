using GymDB.API.Data.Entities;
using GymDB.API.Data.Enums;
using GymDB.API.Models.Exercise;

namespace GymDB.API.Mappers
{
    public static class ExerciseMapper
    {
        public static Exercise ToEntity(this ExerciseCreateModel createModel, User owner)
        {
            return new Exercise
            {
                Id = Guid.NewGuid(),
                OnCreated = DateOnly.FromDateTime(DateTime.UtcNow),
                OnModified = DateTime.UtcNow,
                ImageCount = 0,

                Name = createModel.Name,
                Instructions = createModel.Instructions,
                MuscleGroups = createModel.MuscleGroups,
                Type = createModel.Type,
                Difficulty = createModel.Difficulty,
                Equipment = createModel.Equipment,
                Visibility = createModel.Visibility,

                OwnerId = createModel.Visibility == ExerciseVisibility.Private ? owner.Id : null,
                Owner = createModel.Visibility == ExerciseVisibility.Private ? owner : null
            };
        }

        public static void ApplyUpdateModel(this Exercise exercise, ExerciseUpdateModel update)
        {
            exercise.Name = update.Name;
            exercise.Instructions = update.Instructions;
            exercise.MuscleGroups = update.MuscleGroups;
            exercise.Type = update.Type;
            exercise.Difficulty = update.Difficulty;
            exercise.Equipment = update.Equipment;
        }

        public static ExerciseViewModel ToViewModel(this Exercise exercise, List<Uri>? imageUris)
        {
            return new ExerciseViewModel
            {
                Id = exercise.Id,

                Name = exercise.Name,
                Instructions = exercise.Instructions,
                MuscleGroups = exercise.MuscleGroups,
                Type = exercise.Type,
                Difficulty = exercise.Difficulty,
                Equipment = exercise.Equipment,
                Visibility = exercise.Visibility,
                ImageUris = imageUris,

                OwnerId = exercise.OwnerId,
                OwnerUsername = exercise.Owner?.Username
            };
        }

        public static ExercisePreviewModel ToPreviewModel(this Exercise exercise)
        {
            return new ExercisePreviewModel
            {
                Id = exercise.Id,
                Name = exercise.Name,
                Type = exercise.Type,
                Difficulty = exercise.Difficulty,
                MuscleGroups = exercise.MuscleGroups,
                Visibility = exercise.Visibility
            };
        }
    }
}
