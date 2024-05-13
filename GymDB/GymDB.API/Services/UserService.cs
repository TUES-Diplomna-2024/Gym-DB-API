using GymDB.API.Data.Entities;
using GymDB.API.Models.User;
using GymDB.API.Repositories.Interfaces;
using GymDB.API.Services.Interfaces;
using GymDB.API.Mappers;
using GymDB.API.Exceptions;
using GymDB.API.Data.Enums;

namespace GymDB.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IJwtService jwtService;
        private readonly IRoleService roleService;
        private readonly IWorkoutService workoutService;
        private readonly IExerciseService exerciseService;
        private readonly IExerciseRecordService exericseRecordService;

        public UserService(IUserRepository userRepository, IJwtService jwtService, IRoleService roleService, IWorkoutService workoutService, IExerciseService exerciseService, IExerciseRecordService exericseRecordService)
        {
            this.userRepository = userRepository;
            this.jwtService = jwtService;
            this.roleService = roleService;
            this.workoutService = workoutService;
            this.exerciseService = exerciseService;
            this.exericseRecordService = exericseRecordService;
        }

        public async Task<UserAuthModel> SignInAsync(UserSignInModel signInModel)
        {
            User? user = await userRepository.GetUserByEmailAsync(signInModel.Email);

            if (user != null && IsPasswordCorrect(signInModel.Password, user.Password))
            {
                string accessToken = jwtService.GenerateNewAccessToken(user.Id, user.Role.NormalizedName);
                string refreshToken = jwtService.GenerateNewRefreshToken(user.Id);

                return new UserAuthModel(accessToken, refreshToken);
            }

            throw new UnauthorizedException("Invalid sign in attempt!");
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

        public async Task<List<UserPreviewModel>> FindUsersPreviewsAsync(HttpContext context, string query)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);

            List<User> matchingUsers = await userRepository.FindAllUsersMatchingUsernameOrEmailAsync(query);

            return matchingUsers.Select(user => user.ToPreviewModel(GetAssignableRoleForUser(currUser, user))).ToList();
        }

        public async Task<bool> IsUserWithIdExistAsync(Guid userId)
            => (await userRepository.GetUserByIdAsync(userId)) != null;

        public async Task UpdateCurrUserAsync(HttpContext context, UserUpdateModel updateModel)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);

            currUser.ApplyUpdateModel(updateModel);

            await userRepository.UpdateUserAsync(currUser);
        }

        public async Task RemoveCurrUserAsync(HttpContext context, string password)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);

            if (roleService.IsUserSuperAdmin(currUser))
                throw new ForbiddenException("The root admin cannot be deleted!");

            if (!IsPasswordCorrect(password, currUser.Password))
                throw new UnauthorizedException("Incorrect password!");

            await RemoveUserAsync(currUser);
        }

        public async Task RemoveUserByIdAsync(HttpContext context, Guid userId)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);

            User? user = await userRepository.GetUserByIdAsync(userId);

            if (user == null)
                throw new NotFoundException("The specified user could not be found!");

            if (roleService.IsUserSuperAdmin(user))
                throw new ForbiddenException("The root admin cannot be deleted!");

            if (roleService.IsUserAdmin(user) && !roleService.IsUserSuperAdmin(currUser))
                throw new ForbiddenException("You cannot delete another admin user! This can be done only by the root admin!");

            await RemoveUserAsync(user);
        }

        private async Task RemoveUserAsync(User user)
        {
            // Remove all related data associated with the user
            await workoutService.RemoveAllUserWorkoutsAsync(user.Id);  // workouts
            await exerciseService.RemoveAllUserCustomExercisesAsync(user.Id);  // custom exercises
            await exericseRecordService.RemoveAllUserExerciseRecordsAsync(user.Id);  // exercise records

            await userRepository.RemoveUserAsync(user);
        }

        private async Task<bool> IsUserAlreadyRegisteredWithEmailAsync(string email)
            => (await userRepository.GetUserByEmailAsync(email)) != null;

        private bool IsPasswordCorrect(string plainPassword, string hashedPassword)
            => BCrypt.Net.BCrypt.EnhancedVerify(plainPassword, hashedPassword);

        private AssignableRole? GetAssignableRoleForUser(User currUser, User targetUser)
        {
            // Root admin's role cannot be changed.
            // Admin user cannot change another admin's role. Only the root admin can do so.
            if (roleService.IsUserSuperAdmin(targetUser) ||
                (roleService.IsUserAdmin(targetUser) && roleService.IsUserAdmin(currUser)))
            {
                return null;
            }

            // Normal users can be promoted to admins, and admins can be downgraded to normal users
            return roleService.IsUserNormie(targetUser) ? AssignableRole.Admin : AssignableRole.Normie;           
        }
    }
}
