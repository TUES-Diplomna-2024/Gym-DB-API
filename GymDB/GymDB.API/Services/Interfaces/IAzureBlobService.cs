namespace GymDB.API.Services.Interfaces
{
    public interface IAzureBlobService
    {
        Task UploadExerciseImageAsync(Guid exerciseId, Guid imageId, IFormFile image);

        Task DeleteExerciseImageAsync(Guid exerciseId, Guid imageId, string fileExtension);

        Uri GetExerciseImageUri(Guid exerciseId, Guid imageId, string fileExtension);

        bool IsFileAllowedInContainer(IFormFile file);
    }
}
