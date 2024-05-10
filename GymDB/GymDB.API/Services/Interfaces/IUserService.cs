﻿using GymDB.API.Models.User;

namespace GymDB.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserAuthModel> SignInAsync(UserSignInModel signInModel);

        Task<UserAuthModel> SignUpAsync(UserSignUpModel signUpModel);

        Task<string> RefreshAsync(HttpContext context);

        Task<UserProfileModel> GetCurrUserProfileAsync(HttpContext context);

        Task<UserProfileModel> GetUserProfileByIdAsync(Guid userId);

        Task<List<UserPreviewModel>> FindUsersPreviewsAsync(HttpContext context, string query);

        Task<bool> IsUserWithIdExistAsync(Guid userId);

        Task UpdateCurrUserAsync(HttpContext context, UserUpdateModel updateModel);

        Task RemoveCurrUserAsync(HttpContext context, string password);

        Task RemoveUserByIdAsync(HttpContext context, Guid userId);
    }
}
