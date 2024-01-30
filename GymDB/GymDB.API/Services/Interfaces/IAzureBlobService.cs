using GymDB.API.Data.Entities;

namespace GymDB.API.Services.Interfaces
{
    public interface IAzureBlobService
    {
        string GetUserProfileImageBlobName(User user, string fileExtension);

        string GetExerciseFileBlobName(Exercise exercise, Guid fileId, string fileExtension);

        Uri GetBlobUri(ExerciseImage exerciseImage);

        Uri GetBlobUri(string container, string blobName);

        bool IsFileAllowedInContainer(string filename, string container);

        IFormFile[] GetNotAllowedFilesInContainer(List<IFormFile> files, string container);

        bool UploadUserProfileImage(User user, IFormFile profileImage);

        bool UploadExerciseImage(Exercise exercise, IFormFile image, Guid imageId);

        bool UploadExerciseVideo(Exercise exercise, IFormFile video, Guid videoId);

        bool UploadFile(IFormFile file, string blobName, string container);

        void DeleteExerciseImage(ExerciseImage exerciseImage);

        void DeleteFile(string blobName, string container);
    }
}
