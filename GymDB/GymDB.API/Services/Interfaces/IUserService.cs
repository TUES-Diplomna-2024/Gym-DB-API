using GymDB.API.Models.User;

namespace GymDB.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserAuthModel> SignInAsync(UserSignInModel signInModel);

        Task<UserAuthModel> SignUpAsync(UserSignUpModel signUpModel);

        Task<string> RefreshAsync(HttpContext context);

        Task<UserProfileModel> GetCurrUserProfileAsync(HttpContext context);

        Task<UserProfileExtendedModel> GetUserProfileByIdAsync(HttpContext context, Guid userId);

        Task<List<UserPreviewModel>> FindUsersPreviewsAsync(string query);

        Task<bool> IsUserWithIdExistAsync(Guid userId);

        Task UpdateCurrUserAsync(HttpContext context, UserUpdateModel updateModel);

        Task RemoveCurrUserAsync(HttpContext context, string password);

        Task RemoveUserByIdAsync(HttpContext context, Guid userId);
    }
}
