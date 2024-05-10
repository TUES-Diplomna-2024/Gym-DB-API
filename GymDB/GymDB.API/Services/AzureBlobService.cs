using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MimeDetective;
using GymDB.API.Data.Settings;
using GymDB.API.Repositories.Interfaces;
using GymDB.API.Services.Interfaces;

namespace GymDB.API.Services
{
    public class AzureBlobService : IAzureBlobService
    {
        private readonly AzureSettings azureSettings;
        private readonly IAzureBlobRepository azureBlobRepository;
        private readonly ContentInspector fileInspector;

        public AzureBlobService(IOptions<AzureSettings> settings, IAzureBlobRepository azureBlobRepository)
        {
            azureSettings = settings.Value;
            this.azureBlobRepository = azureBlobRepository;

            fileInspector = new ContentInspectorBuilder()
            {
                Definitions = MimeDetective.Definitions.Default.All()
            }.Build();
        }

        public async Task UploadExerciseImageAsync(Guid exerciseId, Guid imageId, IFormFile image)
        {
            string blobName = azureBlobRepository.GetBlobName(exerciseId, imageId);

            using var stream = image.OpenReadStream();

            await azureBlobRepository.UploadBlobAsync(blobName, stream);
        }

        public async Task DeleteExerciseImageAsync(Guid exerciseId, Guid imageId)
        {
            string blobName = azureBlobRepository.GetBlobName(exerciseId, imageId);
            await azureBlobRepository.DeleteBlobAsync(blobName);
        }

        public Uri GetExerciseImageUri(Guid exerciseId, Guid imageId)
        {
            string blobName = azureBlobRepository.GetBlobName(exerciseId, imageId);
            return new Uri($"{azureSettings.BaseBlobUri}{azureSettings.ImageContainer}/{blobName}");
        }

        public bool IsFileAllowedInContainer(IFormFile file)
        {
            string? fileMimeType = GetFileMimeType(file);

            return !fileMimeType.IsNullOrEmpty() &&
                    azureSettings.AcceptedFileMimeTypes.Contains(fileMimeType!) &&
                    file.Length <= azureSettings.MaxFileSize;
        }

        private string? GetFileMimeType(IFormFile file)
        {
            using var stream = file.OpenReadStream();

            var results = fileInspector.Inspect(stream);

            return results.Any() ? results.First().Definition.File.MimeType : null;
        }
    }
}
