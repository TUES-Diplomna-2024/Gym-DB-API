using GymDB.API.Data.Entities;
using GymDB.API.Models.ExerciseImage;

namespace GymDB.API.Mappers
{
    public static class ExerciseImageMapper
    {
        public static ExerciseImage ToEntity(this IFormFile image, Exercise exercise, int position)
        {
            return new ExerciseImage
            {
                Id = Guid.NewGuid(),
                ExerciseId = exercise.Id,
                Exercise = exercise,
                FileExtension = Path.GetExtension(image.FileName),  // TODO: Update the way you are getting the file extension
                Position = position
            };
        }

        public static ExerciseImageViewModel ToViewModel(this ExerciseImage exerciseImage, Uri uri)
        {
            return new ExerciseImageViewModel
            {
                Id = exerciseImage.Id,
                Uri = uri
            };
        }
    }
}
