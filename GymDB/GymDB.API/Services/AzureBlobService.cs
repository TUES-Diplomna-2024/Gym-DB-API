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
            blobServiceClient = new BlobServiceClient(settings.AzureSettings.BlobUri, settings.AzureSettings.Credential);
        }

        public void Test()
        {
            var containers = blobServiceClient.GetBlobContainers();

            Console.WriteLine("==============================");

            foreach(var container in containers)
            {
                Console.WriteLine(container.Name);
            }

            Console.WriteLine("==============================");

        }
    }
}
