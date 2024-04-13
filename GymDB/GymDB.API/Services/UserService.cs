using GymDB.API.Data.Entities;
using GymDB.API.Models.User;
using GymDB.API.Repositories.Interfaces;
using GymDB.API.Services.Interfaces;
using System.Net;
using GymDB.API.Mappers;


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

        public async Task<UserAuthModel> SignInAsync(UserSignInModel signInModel)
        {
            throw new NotImplementedException();
        }

        public async Task<UserAuthModel> SignUpAsync(UserSignUpModel signUpModel)
        {
            throw new NotImplementedException();

            if (await IsUserAlreadyRegisteredWithEmail(signUpModel.Email))
            {
                /*return new ServiceResultModel(null, HttpStatusCode.Conflict,
                                              $"Email {signUpModel.Email} is already in use by another user!");*/
            }

            User user = signUpModel.ToEntity();

            // TODO - Assign a role to the new user
            /*if (!roleService.AssignUserRole(user, settings.DBSeed.DefaultRole))
                return StatusCode(500, $"Something went wrong when creating your account!");*/

            await userRepository.AddUserAsync(user);

            string accessToken = jwtService.GenerateNewAccessToken(user.Id, user.Role.NormalizedName);
            string refreshToken = jwtService.GenerateNewRefreshToken(user.Id);
            var auth = new UserAuthModel(accessToken, refreshToken);

            /*return new ServiceResultModel(auth, HttpStatusCode.OK);*/
        }

        public async Task<string> RefreshAsync(HttpContext context)
        { throw new NotImplementedException(); }

        public async Task<UserProfileModel> GetCurrUserProfileAsync(HttpContext context)
            { throw new NotImplementedException();}

        public async Task<UserProfileModel> GetUserProfileByIdAsync(Guid userId)
            { throw new NotImplementedException();}

        public async Task<List<UserPreviewModel>> GetAllUsersPreviewsAsync()
            { throw new NotImplementedException();}

        public async Task UpdateCurrUserAsync(HttpContext context, UserUpdateModel updateModel)
            { throw new NotImplementedException();}

        public async Task AssignUserRoleAsync(HttpContext context, Guid userId, UserAssignRoleModel assignRoleModel)
            { throw new NotImplementedException();}

        public async Task DeleteCurrUserAsync(HttpContext context, UserDeleteModel deleteModel)
            { throw new NotImplementedException();}

        public async Task DeleteUserByIdAsync(HttpContext context, Guid userId)
            { throw new NotImplementedException();}

        private async Task<bool> IsUserAlreadyRegisteredWithEmail(string email)
        {
            return (await userRepository.GetUserByEmailAsync(email)) != null;
        }
    }
}
