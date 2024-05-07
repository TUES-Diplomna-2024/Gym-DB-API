using Azure;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using GymDB.API.Data.Settings;
using GymDB.API.Repositories.Interfaces;
using GymDB.API.Exceptions;

namespace GymDB.API.Repositories
{
    public class AzureBlobRepository : IAzureBlobRepository
    {
        private readonly AzureSettings azureSettings;
        private readonly BlobServiceClient blobServiceClient;

        public AzureBlobRepository(IOptions<AzureSettings> settings)
        {
            azureSettings = settings.Value;
            blobServiceClient = new BlobServiceClient(azureSettings.BaseBlobUri, azureSettings.Credential);
        }

        public async Task UploadBlobAsync(string blobName, MemoryStream content)
        {
            var blob = GetBlobClient(blobName);

            try
            {
                await blob.UploadAsync(content, true);
            }
            catch (RequestFailedException)
            {
                throw new InternalServerErrorException("Failed to upload a file! Please try again later!");
            }
        }

        public async Task DeleteBlobAsync(string blobName)
        {
            var blob = GetBlobClient(blobName);

            try
            {
                await blob.DeleteIfExistsAsync();
            }
            catch (RequestFailedException)
            {
                throw new InternalServerErrorException("Failed to delete a file! Please try again later!");
            }
        }

        public string GetBlobName(Guid exerciseId, Guid imageId, string fileExtension)
            => $"exercises/{exerciseId}/{imageId}{fileExtension}";

        private BlobClient GetBlobClient(string blobName)
        {
            var blobContainer = blobServiceClient.GetBlobContainerClient(azureSettings.ImageContainer);
            return blobContainer.GetBlobClient(blobName);
        }
    }
}
