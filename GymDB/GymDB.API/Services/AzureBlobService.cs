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

        public Uri GetBlobUri(ExerciseImage exerciseImage)
        {
            string blobName = GetExerciseFileBlobName(exerciseImage.Exercise, exerciseImage.Id, exerciseImage.FileExtension);
            return GetBlobUri(settings.AzureSettings.ImageContainer, blobName);
        }

        public Uri GetBlobUri(string container, string blobName)
            => new Uri($"{settings.AzureSettings.BlobUri}{container}/{blobName}");

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

        public IFormFile[] GetNotAllowedFilesInContainer(List<IFormFile> files, string container)
            => files.Where(file => !IsFileAllowedInContainer(file.FileName, container)).ToArray();

        public bool UploadUserProfileImage(User user, IFormFile profileImage)
        {
            string container = settings.AzureSettings.ImageContainer;

            string fileExtension = Path.GetExtension(profileImage.FileName);
            string blobName = GetUserProfileImageBlobName(user, fileExtension);

            return UploadFile(profileImage, blobName, container);
        }

        public bool UploadExerciseImage(Exercise exercise, IFormFile image, Guid imageId)
        {
            string container = settings.AzureSettings.ImageContainer;

            string fileExtension = Path.GetExtension(image.FileName);
            string blobName = GetExerciseFileBlobName(exercise, imageId, fileExtension);

            return UploadFile(image, blobName, container);
        }

        public bool UploadExerciseVideo(Exercise exercise, IFormFile video, Guid videoId)
        {
            string container = settings.AzureSettings.VideoContainer;

            string fileExtension = Path.GetExtension(video.FileName);
            string blobName = GetExerciseFileBlobName(exercise, videoId, fileExtension);

            return UploadFile(video, blobName, container);
        }

        public bool UploadFile(IFormFile file, string blobName, string container)
        {
            try
            {
                var blobContainer = blobServiceClient.GetBlobContainerClient(container);
                var blob = blobContainer.GetBlobClient(blobName);

                using (MemoryStream fileUploadStream = new MemoryStream())
                {
                    file.CopyTo(fileUploadStream);
                    fileUploadStream.Position = 0;

                    blob.Upload(fileUploadStream, true);
                }
            } catch (Azure.RequestFailedException)
            {
                return false;
            }

            return true;
        }

        public void DeleteExerciseImage(ExerciseImage exerciseImage)
        {
            string container = settings.AzureSettings.ImageContainer;

            string blobName = GetExerciseFileBlobName(exerciseImage.Exercise, exerciseImage.Id, exerciseImage.FileExtension);

            DeleteFile(blobName, container);
        }

        public void DeleteFile(string blobName, string container)
        {
            try
            {
                var blobContainer = blobServiceClient.GetBlobContainerClient(container);
                var blob = blobContainer.GetBlobClient(blobName);

                blob.DeleteIfExists();
            } catch (Azure.RequestFailedException) { }
        }
    }
}
