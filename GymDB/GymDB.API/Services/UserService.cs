using GymDB.API.Data.Entities;
using GymDB.API.Models.Other;
using GymDB.API.Models.User;
using GymDB.API.Repositories.Interfaces;
using GymDB.API.Services.Interfaces;
using System.Net;
using GymDB.API.Mappers;
using Azure.Core;


namespace GymDB.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IJwtService jwtService;

        public UserService(IUserRepository userRepository, IJwtService jwtService)
        {
            this.userRepository = userRepository;
            this.jwtService = jwtService;
        }

        public async Task<ServiceResultModel> SignInAsync(UserSignInModel signInModel)
        {
            
        }

        public async Task<ServiceResultModel> SignUpAsync(UserSignUpModel signUpModel)
        {
            if (await IsUserAlreadyRegisteredWithEmail(signUpModel.Email))
            {
                return new ServiceResultModel(null, HttpStatusCode.Conflict,
                                              $"Email {signUpModel.Email} is already in use by another user!");
            }

            User user = signUpModel.ToEntity();

            // TODO - Assign a role to the new user
            /*if (!roleService.AssignUserRole(user, settings.DBSeed.DefaultRole))
                return StatusCode(500, $"Something went wrong when creating your account!");*/

            await userRepository.AddUserAsync(user);

            string accessToken = jwtService.GenerateNewAccessToken(user.Id, user.Role.NormalizedName);
            string refreshToken = jwtService.GenerateNewRefreshToken(user.Id);
            var auth = new UserAuthModel(accessToken, refreshToken);

            return new ServiceResultModel(auth, HttpStatusCode.OK);
        }

        public async Task<ServiceResultModel> RefreshAsync(HttpContext context);

        public async Task<ServiceResultModel> GetCurrUserProfileAsync(HttpContext context);

        public async Task<ServiceResultModel> GetUserProfileByIdAsync(Guid userId);

        public async Task<ServiceResultModel> GetAllUsersPreviewsAsync();

        public async Task<ServiceResultModel> UpdateCurrUserAsync(HttpContext context, UserUpdateModel updateModel);

        public async Task<ServiceResultModel> AssignUserRoleAsync(HttpContext context, Guid userId, UserAssignRoleModel assignRoleModel);

        public async Task<ServiceResultModel> DeleteCurrUserAsync(HttpContext context, UserDeleteModel deleteModel);

        public async Task<ServiceResultModel> DeleteUserByIdAsync(HttpContext context, Guid userId);

        private async Task<bool> IsUserAlreadyRegisteredWithEmail(string email)
        {
            return (await userRepository.GetUserByEmailAsync(email)) != null;
        }
    }
}
