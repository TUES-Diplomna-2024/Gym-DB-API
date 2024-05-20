namespace GymDB.API.Repositories.Interfaces
{
    public interface IAzureBlobRepository
    {
        Task UploadBlobAsync(string blobName, Stream content);

        Task DeleteBlobAsync(string blobName);

        string GetBlobName(Guid exerciseId, Guid imageId);
    }
}
