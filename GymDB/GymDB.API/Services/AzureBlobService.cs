using Azure.Storage.Blobs;
using GymDB.API.Data;
using GymDB.API.Services.Interfaces;

namespace GymDB.API.Services
{
    public class AzureBlobService : IAzureBlobService
    {
        private readonly ApplicationSettings settings;
        private readonly BlobServiceClient blobServiceClient;

        public AzureBlobService(IConfiguration config)
        {
            settings = new ApplicationSettings(config);
        }
    }
}
