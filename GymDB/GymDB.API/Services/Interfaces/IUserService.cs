using GymDB.API.Models.User;

namespace GymDB.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserAuthModel> SignInAsync(UserSignInModel signInModel);

        Task<UserAuthModel> SignUpAsync(UserSignUpModel signUpModel);

        Task<string> RefreshAsync(HttpContext context);

        Task<UserProfileModel> GetCurrUserProfileAsync(HttpContext context);

        Task<UserProfileModel> GetUserProfileByIdAsync(Guid userId);

        Task<List<UserPreviewModel>> GetAllUsersPreviewsAsync();

        Task<bool> IsUserWithIdExistAsync(Guid userId);

        Task UpdateCurrUserAsync(HttpContext context, UserUpdateModel updateModel);

        Task RemoveCurrUserAsync(HttpContext context, UserDeleteModel deleteModel);

        Task RemoveUserByIdAsync(HttpContext context, Guid userId);
    }
}
