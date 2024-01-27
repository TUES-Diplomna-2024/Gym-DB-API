using GymDB.API.Data.Entities;
using GymDB.API.Models.Exercise;
using System.Xml.Linq;

namespace GymDB.API.Mapping
{
    public static class ExerciseMapping
    {
        public static Exercise ToEntity(this ExerciseCreateModel createModel, User owner)
        {
            return new Exercise
            {
                Id = Guid.NewGuid(),
                OnCreated = DateOnly.FromDateTime(DateTime.UtcNow),
                OnModified = DateTime.UtcNow,

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

        public static ExerciseNormalInfoModel ToNormalInfoModel(this Exercise exercise, User? owner)
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

                OwnerId = owner != null ? owner.Id : null,
                OwnerUsername = owner != null ? owner.Username : null
            };
        }

        public static ExerciseAdvancedInfoModel ToAdvancedInfoModel(this Exercise exercise, User? owner)
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
