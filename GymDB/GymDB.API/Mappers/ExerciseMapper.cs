using GymDB.API.Data.Entities;
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
                IsPrivate = createModel.IsPrivate,

                UserId = createModel.IsPrivate ? owner.Id : null,
                User = createModel.IsPrivate ? owner : null
            };
        }

        public static ExerciseImage ToExerciseImageEntity(this Exercise exercise, string fileExtension, int position)
        {
            return new ExerciseImage
            {
                Id = Guid.NewGuid(),
                ExerciseId = exercise.Id,
                Exercise = exercise,
                FileExtension = fileExtension,
                Position = position
            };
        }

        public static ExerciseNormalInfoModel ToNormalInfoModel(this Exercise exercise, User? owner, List<Uri>? imageUris)
        {
            return new ExerciseNormalInfoModel
            {
                Id = exercise.Id,
                Name = exercise.Name,
                Instructions = exercise.Instructions,
                MuscleGroups = exercise.MuscleGroups,
                Type = exercise.Type,
                Difficulty = exercise.Difficulty,
                Equipment = exercise.Equipment,
                IsPrivate = exercise.IsPrivate,
                ImageCount = exercise.ImageCount,
                ImageUris = imageUris,

                OwnerId = owner != null ? owner.Id : null,
                OwnerUsername = owner != null ? owner.Username : null
            };
        }

        public static ExerciseAdvancedInfoModel ToAdvancedInfoModel(this Exercise exercise, User? owner, List<Uri>? imageUris)
        {
            return new ExerciseAdvancedInfoModel
            {
                Id = exercise.Id,
                Name = exercise.Name,
                Instructions = exercise.Instructions,
                MuscleGroups = exercise.MuscleGroups,
                Type = exercise.Type,
                Difficulty = exercise.Difficulty,
                Equipment = exercise.Equipment,
                IsPrivate = exercise.IsPrivate,
                ImageCount = exercise.ImageCount,
                ImageUris = imageUris,

                OwnerId = owner != null ? owner.Id : null,
                OwnerUsername = owner != null ? owner.Username : null,

                OnCreated = exercise.OnCreated,
                OnModified = exercise.OnModified
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
                IsPrivate = exercise.IsPrivate
            };
        }
    }
}
