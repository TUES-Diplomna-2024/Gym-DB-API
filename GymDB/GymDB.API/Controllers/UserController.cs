using Microsoft.AspNetCore.Mvc;
using GymDB.API.Services.Interfaces;
using GymDB.API.Models.User;
using GymDB.API.Data.Entities;
using GymDB.API.Mappers;
using GymDB.API.Data;
using GymDB.API.Attributes;

namespace GymDB.API.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationSettings settings;

        private readonly IUserService userService;
        private readonly IRoleService roleService;
        private readonly IJwtService jwtService;
        private readonly IExerciseService exerciseService;

        public UserController(IConfiguration config, IUserService userService, IRoleService roleService, IJwtService jwtService, IExerciseService exerciseService)
        {
            settings = new ApplicationSettings(config);

            this.userService = userService;
            this.roleService = roleService;
            this.jwtService = jwtService;
            this.exerciseService = exerciseService;
        }

        /* POST REQUESTS */

        [HttpPost("signup")]
        public IActionResult SignUp(UserSignUpModel signUpAttempt)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (userService.IsUserAlreadyRegisteredWithEmail(signUpAttempt.Email))
                return Conflict($"Email {signUpAttempt.Email} is already in use by another user!");

            User user = signUpAttempt.ToEntity();

            if (!roleService.AssignUserRole(user, settings.DBSeed.DefaultRole))
                return StatusCode(500, $"Something went wrong when creating your account!");

            userService.AddUser(user);

            string accessToken = jwtService.GenerateNewAccessToken(user.Id, user.Role.NormalizedName);
            string refreshToken = jwtService.GenerateNewRefreshToken(user.Id);

            return Ok(new UserAuthModel(accessToken, refreshToken));
        }

        [HttpPost("signin")]
        public IActionResult SignIn(UserSignInModel signInAttempt)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            User? user = userService.GetUserByEmailAndPassword(signInAttempt.Email, signInAttempt.Password);

            if (user == null)
                return Unauthorized("Invalid sign in attempt!");

            string accessToken = jwtService.GenerateNewAccessToken(user.Id, user.Role.NormalizedName);
            string refreshToken = jwtService.GenerateNewRefreshToken(user.Id);

            return Ok(new UserAuthModel(accessToken, refreshToken));
        }

        /* GET REQUESTS */

        [HttpGet("refresh"), RefreshTokenRequired]
        public IActionResult Refresh()
        {
            // Since the RefreshTokenRequired attribute is applied to this endpoint, the RefreshTokenMiddleware has already validated the refresh token,
            // ensuring that the current user is not null. Therefore, we can safely generate a new access token for the current user.

            User currUser = userService.GetCurrUser(HttpContext)!;

            string accessToken = jwtService.GenerateNewAccessToken(currUser.Id, currUser.Role.NormalizedName);

            return Ok(accessToken);
        }

        [HttpGet("current"), CustomAuthorize]
        public IActionResult GetCurrUser()
        {
            // Since the CustomAuthorize attribute is applied to this endpoint, the AccessTokenMiddleware has already validated the access token,
            // ensuring that the current user is not null.

            User currUser = userService.GetCurrUser(HttpContext)!;

            return Ok(currUser.ToProfileModel());
        }

        [HttpGet("{id}"), CustomAuthorize]
        public IActionResult GetUserById(Guid id)
        {
           User? user = userService.GetUserById(id);

            if (user == null)
                return NotFound($"User with id '{id}' could not be found!");
            
            return Ok(user.ToProfileModel());
        }

        [HttpGet, CustomAuthorize(Roles = "SUPER_ADMIN,ADMIN")]
        public IActionResult GetAllUserPreviews()
        {
            List<UserPreviewModel> userPreviews = userService.GetAllUserPreviews();

            return Ok(userPreviews);
        }

        [HttpGet("current/custom-exercises"), CustomAuthorize]
        public IActionResult GetCurrUserCustomExercisesPreviews()
        {
            User currUser = userService.GetCurrUser(HttpContext)!;

            List<Exercise> customExercises = exerciseService.GetAllUserCustomExercises(currUser);

            return Ok(exerciseService.GetExercisesPreviews(customExercises));
        }

        /* PUT REQUESTS */

        [HttpPut("current"), CustomAuthorize]
        public IActionResult UpdateCurrUser(UserUpdateModel updateAttempt)
        {
            User currUser = userService.GetCurrUser(HttpContext)!;

            if (!ModelState.IsValid)
                return BadRequest();

            userService.UpdateUser(currUser, updateAttempt);

            return Ok();
        }

        [HttpPut("{id}/role"), CustomAuthorize(Roles = "SUPER_ADMIN,ADMIN")]
        public IActionResult AssignUserRole(Guid id, UserAssignRoleModel assignRoleAttempt)
        {
            User currUser = userService.GetCurrUser(HttpContext)!;

            User? user = userService.GetUserById(id);

            if (user == null)
                return NotFound($"User with id '{id}' could not be found!");

            if (roleService.HasUserRole(user, assignRoleAttempt.RoleNormalizedName))
                return StatusCode(403, $"User has already role '{assignRoleAttempt.RoleNormalizedName}'!");

            if (assignRoleAttempt.RoleNormalizedName == "SUPER_ADMIN")
                return StatusCode(403, "Role 'SUPER_ADMIN' is reserved for the root admin only! You cannot assign it to another user!");

            if (roleService.HasUserRole(user, "SUPER_ADMIN"))
                return StatusCode(403, "The role of the root admin cannot be changed!");

            if (roleService.HasUserRole(user, "ADMIN") && !roleService.HasUserRole(currUser, "SUPER_ADMIN"))
                return StatusCode(403, "You cannot re-assign new role to another admin user! This can be done only by the root admin!");

            if (!roleService.AssignUserRole(user, assignRoleAttempt.RoleNormalizedName))
                return NotFound($"Role with normalized name '{assignRoleAttempt.RoleNormalizedName}' could not be found!");

            userService.UpdateUser(user);

            return Ok();
        }

        /* DELETE REQUESTS */

        [HttpDelete("current"), CustomAuthorize]
        public IActionResult DeleteCurrUser(UserDeleteModel deleteAttempt)
        {
            User currUser = userService.GetCurrUser(HttpContext)!;

            if (roleService.HasUserRole(currUser, "SUPER_ADMIN"))
                return StatusCode(403, "The root admin cannot be deleted!");

            if (!ModelState.IsValid)
                return BadRequest();

            if (!userService.IsUserPasswordCorrect(currUser, deleteAttempt.Password))
                return Unauthorized("Incorrect password!");

            userService.RemoveUser(currUser);

            return Ok();
        }

        [HttpDelete("{id}"), CustomAuthorize(Roles = "SUPER_ADMIN,ADMIN")]
        public IActionResult DeleteUserById(Guid id)
        {
            User currUser = userService.GetCurrUser(HttpContext)!;

            User? user = userService.GetUserById(id);

            if (user == null)
                return NotFound($"User with id '{id}' could not be found!");

            if (roleService.HasUserRole(user, "SUPER_ADMIN"))
                return StatusCode(403, "The root admin cannot be deleted!");

            if (roleService.HasUserRole(user, "ADMIN") && !roleService.HasUserRole(currUser, "SUPER_ADMIN"))
                return StatusCode(403, "You cannot delete another admin user! This can be done only by the root admin!");
            
            userService.RemoveUser(user);

            return Ok();
        }
    }
}
