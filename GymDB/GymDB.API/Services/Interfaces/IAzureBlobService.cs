using GymDB.API.Data.Entities;

namespace GymDB.API.Services.Interfaces
{
    public interface IAzureBlobService
    {
        string GetUserProfileImageBlobName(User user, string fileExtension);

        string GetExerciseFileBlobName(Exercise exercise, Guid fileId, string fileExtension);

        Uri? UploadUserProfileImage(User user, IFormFile profileImage);

        Uri? UploadExerciseImage(Exercise exercise, IFormFile image, Guid imageId);

        Uri? UploadExerciseVideo(Exercise exercise, IFormFile video, Guid videoId);

        Uri UploadFile(IFormFile file, string blobName, string container);

        bool DeleteFile(string blobName, string container);

        bool IsFileAllowedInContainer(string filename, string container);
    }
}
