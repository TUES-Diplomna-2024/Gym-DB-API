namespace GymDB.API.Services.Interfaces
{
    public interface IAzureBlobService
    {
        Task UploadExerciseImageAsync(Guid exerciseId, Guid imageId, IFormFile image);

        Task DeleteExerciseImageAsync(Guid exerciseId, Guid imageId);

        Uri GetExerciseImageUri(Guid exerciseId, Guid imageId);

        bool IsFileAllowedInContainer(IFormFile file);
    }
}
