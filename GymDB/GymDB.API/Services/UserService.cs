using GymDB.API.Data.Entities;
using GymDB.API.Models.User;
using GymDB.API.Repositories.Interfaces;
using GymDB.API.Services.Interfaces;
using GymDB.API.Mappers;
using GymDB.API.Exceptions;

namespace GymDB.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IJwtService jwtService;
        private readonly IRoleService roleService;

        public UserService(IUserRepository userRepository, IJwtService jwtService, IRoleService roleService)
        {
            this.userRepository = userRepository;
            this.jwtService = jwtService;
            this.roleService = roleService;
        }

        public async Task<UserAuthModel> SignInAsync(UserSignInModel signInModel)
        {
            User? user = await userRepository.GetUserByEmailAsync(signInModel.Email);

            if (user == null || IsPasswordCorrect(signInModel.Password, user.Password))
                throw new UnauthorizedException("Invalid sign in attempt!");

            string accessToken = jwtService.GenerateNewAccessToken(user.Id, user.Role.NormalizedName);
            string refreshToken = jwtService.GenerateNewRefreshToken(user.Id);

            return new UserAuthModel(accessToken, refreshToken);
        }

        public async Task<UserAuthModel> SignUpAsync(UserSignUpModel signUpModel)
        {
            if (await IsUserAlreadyRegisteredWithEmailAsync(signUpModel.Email))
                throw new ConflictException($"Email '{signUpModel.Email}' is already in use by another user!");

            User user = signUpModel.ToEntity();

            await roleService.AssignUserDefaultRoleAsync(user);
            await userRepository.AddUserAsync(user);

            string accessToken = jwtService.GenerateNewAccessToken(user.Id, user.Role.NormalizedName);
            string refreshToken = jwtService.GenerateNewRefreshToken(user.Id);

            return new UserAuthModel(accessToken, refreshToken);
        }

        public async Task<string> RefreshAsync(HttpContext context)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);

            string accessToken = jwtService.GenerateNewAccessToken(currUser.Id, currUser.Role.NormalizedName);

            return accessToken;
        }

        public async Task<UserProfileModel> GetCurrUserProfileAsync(HttpContext context)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);

            return currUser.ToProfileModel();
        }

        public async Task<UserProfileModel> GetUserProfileByIdAsync(Guid userId)
        {
            User? user = await userRepository.GetUserByIdAsync(userId);

            if (user == null)
                throw new NotFoundException("The specified user could not be found!");

            return user.ToProfileModel();
        }

        public async Task<List<UserPreviewModel>> GetAllUsersPreviewsAsync()
        {
            List<User> usersPreviews = await userRepository.GetAllUsersAsync();

            return usersPreviews.Select(user => user.ToPreviewModel()).ToList();
        }

        public async Task<bool> IsUserWithIdExistAsync(Guid userId)
            => (await userRepository.GetUserByIdAsync(userId)) != null;

        private async Task<bool> IsUserAlreadyRegisteredWithEmailAsync(string email)
            => (await userRepository.GetUserByEmailAsync(email)) != null;

        private bool IsPasswordCorrect(string plainPassword, string hashedPassword)
            => BCrypt.Net.BCrypt.EnhancedVerify(plainPassword, hashedPassword);

        public async Task UpdateCurrUserAsync(HttpContext context, UserUpdateModel updateModel)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);

            currUser.ApplyUpdateModel(updateModel);

            await userRepository.UpdateUserAsync(currUser);
        }

        public async Task RemoveCurrUserAsync(HttpContext context, string password)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);

            if (roleService.HasUserRole(currUser, "SUPER_ADMIN"))
                throw new ForbiddenException("The root admin cannot be deleted!");

            if (!IsPasswordCorrect(currUser.Password, password))
                throw new UnauthorizedException("Incorrect password!");

            await userRepository.RemoveUserAsync(currUser);
        }

        public async Task RemoveUserByIdAsync(HttpContext context, Guid userId)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);

            User? user = await userRepository.GetUserByIdAsync(userId);

            if (user == null)
                throw new NotFoundException("The specified user could not be found!");

            if (roleService.HasUserRole(user, "SUPER_ADMIN"))
                throw new ForbiddenException("The root admin cannot be deleted!");

            if (roleService.HasUserRole(user, "ADMIN") && !roleService.HasUserRole(currUser, "SUPER_ADMIN"))
                throw new ForbiddenException("You cannot delete another admin user! This can be done only by the root admin!");

            await userRepository.RemoveUserAsync(user);
        }
    }
}
