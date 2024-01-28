using Azure.Storage.Blobs;
using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

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

        public string GetUserProfileImageBlobName(User user, string fileExtension)
            => $"users/{user.Id}{fileExtension}";

        public string GetExerciseFileBlobName(Exercise exercise, Guid fileId, string fileExtension)
            => $"exercises/{exercise.Id}/{fileId}{fileExtension}";

        public Uri? UploadUserProfileImage(User user, IFormFile profileImage)
        {
            string container = settings.AzureSettings.ImageContainer;

            if (!IsFileAllowedInContainer(profileImage.FileName, container))
                return null;

            string fileExtension = Path.GetExtension(profileImage.FileName);
            string blobName = GetUserProfileImageBlobName(user, fileExtension);

            return UploadFile(profileImage, blobName, container);
        }

        public Uri? UploadExerciseImage(Exercise exercise, IFormFile image, Guid imageId)
        {
            string container = settings.AzureSettings.ImageContainer;

            if (!IsFileAllowedInContainer(image.FileName, container))
                return null;

            string fileExtension = Path.GetExtension(image.FileName);
            string blobName = GetExerciseFileBlobName(exercise, imageId, fileExtension);

            return UploadFile(image, blobName, container);
        }

        public Uri? UploadExerciseVideo(Exercise exercise, IFormFile video, Guid videoId)
        {
            string container = settings.AzureSettings.VideoContainer;

            if (!IsFileAllowedInContainer(video.FileName, container))
                return null;

            string fileExtension = Path.GetExtension(video.FileName);
            string blobName = GetExerciseFileBlobName(exercise, videoId, fileExtension);

            return UploadFile(video, blobName, container);
        }

        public Uri UploadFile(IFormFile file, string blobName, string container)
        {
            var blobContainer = blobServiceClient.GetBlobContainerClient(container);
            var blob = blobContainer.GetBlobClient(blobName);

            using (MemoryStream fileUploadStream = new MemoryStream())
            {
                file.CopyTo(fileUploadStream);
                fileUploadStream.Position = 0;

                blob.Upload(fileUploadStream, true);
            }

            return blob.Uri;
        }

        public bool DeleteFile(string blobName, string container)
        {
            var blobContainer = blobServiceClient.GetBlobContainerClient(container);
            var blob = blobContainer.GetBlobClient(blobName);

            return blob.DeleteIfExists();
        }

        public bool IsFileAllowedInContainer(string filename, string container)
        {
            string fileExtension = Path.GetExtension(filename);

            if (fileExtension.IsNullOrEmpty())
                return false;

            fileExtension = fileExtension.Replace(".", "");

            if (container == settings.AzureSettings.ImageContainer)
                return settings.AzureSettings.ImageTypesAccepted.Contains(fileExtension);
            
            if (container == settings.AzureSettings.VideoContainer)
                return settings.AzureSettings.VideoTypesAccepted.Contains(fileExtension);

            return false;
        }
    }
}
