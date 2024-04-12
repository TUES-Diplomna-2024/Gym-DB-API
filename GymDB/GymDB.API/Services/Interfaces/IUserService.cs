using GymDB.API.Models.Other;
using GymDB.API.Models.User;

namespace GymDB.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResultModel> SignInAsync(UserSignInModel signInModel);

        Task<ServiceResultModel> SignUpAsync(UserSignUpModel signUpModel);

        Task<ServiceResultModel> RefreshAsync(HttpContext context);

        Task<ServiceResultModel> GetCurrUserProfileAsync(HttpContext context);

        Task<ServiceResultModel> GetUserProfileByIdAsync(Guid userId);

        Task<ServiceResultModel> GetAllUsersPreviewsAsync();

        Task<ServiceResultModel> UpdateCurrUserAsync(HttpContext context, UserUpdateModel updateModel);

        Task<ServiceResultModel> AssignUserRoleAsync(HttpContext context, Guid userId, UserAssignRoleModel assignRoleModel);

        Task<ServiceResultModel> DeleteCurrUserAsync(HttpContext context, UserDeleteModel deleteModel);

        Task<ServiceResultModel> DeleteUserByIdAsync(HttpContext context, Guid userId);
    }
}
