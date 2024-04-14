using GymDB.API.Data.Entities;
using GymDB.API.Models.User;
using GymDB.API.Repositories.Interfaces;
using GymDB.API.Services.Interfaces;
using System.Net;
using GymDB.API.Mappers;
using GymDB.API.Exceptions;


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
            User? user = await userRepository.GetUserByEmailAsync(signInModel.Email);

            if (user == null || IsPasswordCorrect(signInModel.Password, user.Password))
            {
                throw new UnauthorizedException("Invalid sign in attempt!");
            }

            string accessToken = jwtService.GenerateNewAccessToken(user.Id, user.Role.NormalizedName);
            string refreshToken = jwtService.GenerateNewRefreshToken(user.Id);

            return new UserAuthModel(accessToken, refreshToken);
        }

        public async Task<UserAuthModel> SignUpAsync(UserSignUpModel signUpModel)
        {
            if (await IsUserAlreadyRegisteredWithEmail(signUpModel.Email))
            {
                throw new ConflictException($"Email {signUpModel.Email} is already in use by another user!");
            }

            User user = signUpModel.ToEntity();

            // TODO - Assign a role to the new user
            /*if (!roleService.AssignUserRole(user, settings.DBSeed.DefaultRole))
                return StatusCode(500, $"Something went wrong when creating your account!");*/

            await userRepository.AddUserAsync(user);

            string accessToken = jwtService.GenerateNewAccessToken(user.Id, user.Role.NormalizedName);
            string refreshToken = jwtService.GenerateNewRefreshToken(user.Id);

            return new UserAuthModel(accessToken, refreshToken);
        }

        public async Task<string> RefreshAsync(HttpContext context)
        {
            // TODO: GetCurrUserAsync returns nullable value
            User currUser = (await userRepository.GetCurrUserAsync(context))!;

            string accessToken = jwtService.GenerateNewAccessToken(currUser.Id, currUser.Role.NormalizedName);

            return accessToken;
        }

        public async Task<UserProfileModel> GetCurrUserProfileAsync(HttpContext context)
        {
            // TODO: GetCurrUserAsync returns nullable value
            User currUser = (await userRepository.GetCurrUserAsync(context))!;

            return currUser.ToProfileModel();
        }

        public async Task<UserProfileModel> GetUserProfileByIdAsync(Guid userId)
        {
            User? user = await userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new NotFoundException($"User with id '{userId}' could not be found!");
            }

            return user.ToProfileModel();
        }

        public async Task<List<UserPreviewModel>> GetAllUsersPreviewsAsync()
        {
            List<User> usersPreviews = await userRepository.GetAllUsersAsync();

            return usersPreviews.Select(user => user.ToPreviewModel()).ToList();
        }

        public async Task UpdateCurrUserAsync(HttpContext context, UserUpdateModel updateModel)
        {
            // TODO: GetCurrUserAsync returns nullable value
            User currUser = (await userRepository.GetCurrUserAsync(context))!;

            currUser.Username = updateModel.Username;
            currUser.BirthDate = updateModel.BirthDate;
            currUser.Gender = updateModel.Gender;
            currUser.Height = updateModel.Height;
            currUser.Weight = updateModel.Weight;

            await userRepository.UpdateUserAsync(currUser);
        }

        public async Task AssignUserRoleAsync(HttpContext context, Guid userId, UserAssignRoleModel assignRoleModel)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveCurrUserAsync(HttpContext context, UserDeleteModel deleteModel)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveUserByIdAsync(HttpContext context, Guid userId)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> IsUserAlreadyRegisteredWithEmail(string email)
            => (await userRepository.GetUserByEmailAsync(email)) != null;

        private bool IsPasswordCorrect(string plainPassword, string hashedPassword)
            => BCrypt.Net.BCrypt.EnhancedVerify(plainPassword, hashedPassword);
    }
}
