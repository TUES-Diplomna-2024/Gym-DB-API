namespace GymDB.API.Repositories.Interfaces
{
    public interface IAzureBlobRepository
    {
        Task UploadBlobAsync(string blobName, MemoryStream content);

        Task DeleteBlobAsync(string blobName);

        string GetBlobName(Guid exerciseId, Guid imageId, string fileExtension);
    }
}
