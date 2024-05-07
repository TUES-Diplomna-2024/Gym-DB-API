using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using GymDB.API.Data.Settings;
using GymDB.API.Repositories.Interfaces;
using GymDB.API.Services.Interfaces;

namespace GymDB.API.Services
{
    public class AzureBlobService : IAzureBlobService
    {
        private readonly AzureSettings azureSettings;
        private readonly IAzureBlobRepository azureBlobRepository;

        public AzureBlobService(IOptions<AzureSettings> settings, IAzureBlobRepository azureBlobRepository)
        {
            azureSettings = settings.Value;
            this.azureBlobRepository = azureBlobRepository;
        }

        public async Task UploadExerciseImageAsync(Guid exerciseId, Guid imageId, IFormFile image)
        {
            // TODO: Get the image file extension in better way
            string blobName = azureBlobRepository.GetBlobName(exerciseId, imageId, Path.GetExtension(image.FileName));

            using MemoryStream fileUploadStream = new MemoryStream();
            image.CopyTo(fileUploadStream);
            fileUploadStream.Position = 0;

            await azureBlobRepository.UploadBlobAsync(blobName, fileUploadStream);
        }

        public async Task DeleteExerciseImageAsync(Guid exerciseId, Guid imageId, string fileExtension)
        {
            string blobName = azureBlobRepository.GetBlobName(exerciseId, imageId, fileExtension);
            await azureBlobRepository.DeleteBlobAsync(blobName);
        }

        public Uri GetExerciseImageUri(Guid exerciseId, Guid imageId, string fileExtension)
        {
            string blobName = azureBlobRepository.GetBlobName(exerciseId, imageId, fileExtension);
            return new Uri($"{azureSettings.BaseBlobUri}{azureSettings.ImageContainer}/{blobName}");
        }

        // TODO: Add Better And More Secure Implementation
        public bool IsFileAllowedInContainer(IFormFile file)
        {
            string fileExtension = Path.GetExtension(file.FileName);

            if (fileExtension.IsNullOrEmpty())
                return false;

            fileExtension = fileExtension.Replace(".", "");

            return azureSettings.AcceptedImageTypes.Contains(fileExtension);
        }
    }
}
