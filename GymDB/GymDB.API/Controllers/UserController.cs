using Microsoft.AspNetCore.Mvc;
using GymDB.API.Services.Interfaces;
using GymDB.API.Models.User;
using GymDB.API.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using GymDB.API.Data.Settings;

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
        private readonly ISessionService sessionService;

        public UserController(IConfiguration config, IUserService userService, IRoleService roleService, IJwtService jwtService, ISessionService sessionService)
        {
            settings = new ApplicationSettings(config);

            this.userService = userService;
            this.roleService = roleService;
            this.jwtService = jwtService;
            this.sessionService = sessionService;
        }

        /* POST REQUESTS */

        [HttpPost("signup")]
        public IActionResult SignUp(UserSignUpModel signUpAttempt)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (userService.IsUserAlreadyRegisteredWithEmail(signUpAttempt.Email))
                return Conflict($"Email {signUpAttempt.Email} is already in use by another user!");

            User user = new User(signUpAttempt);

            if (!roleService.AssignUserRole(user, settings.DBSeed.DefaultRole))
                return StatusCode(500, $"Role '{settings.DBSeed.DefaultRole}' could not be found!");

            userService.AddUser(user);

            string refreshToken = sessionService.CreateNewSession(user);

            return Ok(new UserAuthModel(jwtService.GenerateNewJwtToken(user), refreshToken));
        }

        [HttpPost("signin")]
        public IActionResult SignIn(UserSignInModel signInAttempt)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            User? user = userService.GetUserByEmailAndPassword(signInAttempt.Email, signInAttempt.Password);

            if (user == null)
                return Unauthorized("Invalid sign in attempt!");

            string refreshToken = sessionService.CreateNewSession(user);

            return Ok(new UserAuthModel(jwtService.GenerateNewJwtToken(user), refreshToken));
        }

        [HttpPost("signout"), Authorize]
        public IActionResult SignOut(UserSessionRetrievalModel signOutAttempt)
        {
            if (userService.GetCurrUser(HttpContext) == null)
                return NotFound("The current user no longer exists!");

            Session? userSession = sessionService.GetSessionByRefreshToken(signOutAttempt.RefreshToken);

            if (userSession == null)
                return Unauthorized();

            sessionService.RemoveSession(userSession);

            return Ok();
        }

        [HttpPost("refresh")]
        public IActionResult Refresh(UserSessionRetrievalModel refreshAttempt)
        {
            Session? userSession = sessionService.GetSessionByRefreshToken(refreshAttempt.RefreshToken);

            if (userSession == null)
                return Unauthorized("You must sign in again!");

            return Ok(jwtService.GenerateNewJwtToken(userSession.User));
        }

        /* GET REQUESTS */

        [HttpGet("current"), Authorize]
        public IActionResult GetCurrUser()
        {
            User? currUser = userService.GetCurrUser(HttpContext);

            if (currUser == null)
                return NotFound("The current user no longer exists!");

            return Ok(new UserProfileModel(currUser));
        }

        [HttpGet("{id}"), Authorize]
        public IActionResult GetUserById(Guid id)
        {
           if (userService.GetCurrUser(HttpContext) == null)
                return NotFound("The current user no longer exists!");

            User? user = userService.GetUserById(id);

            if (user == null)
                return NotFound($"User with id '{id}' could not be found!");
            
            return Ok(new UserProfileModel(user));
        }

        [HttpGet, Authorize(Roles = "SUPER_ADMIN,ADMIN")]
        public IActionResult GetAllUsers()
        {
            if (userService.GetCurrUser(HttpContext) == null)
                return NotFound("The current user no longer exists!");

            List<UserProfileModel> users = userService.GetAllUsers()
                                                      .Select(user => new UserProfileModel(user))
                                                      .ToList();

            return Ok(users);
        }

        /* PUT REQUESTS */

        [HttpPut("current"), Authorize]
        public IActionResult UpdateCurrUser(UserUpdateModel updateAttempt)
        {
            User? currUser = userService.GetCurrUser(HttpContext);

            if (currUser == null)
                return NotFound("The current user no longer exists!");

            if (!ModelState.IsValid)
                return BadRequest();

            userService.UpdateUser(currUser, updateAttempt);

            return Ok();
        }

        [HttpPut("{id}/role"), Authorize(Roles = "SUPER_ADMIN,ADMIN")]
        public IActionResult AssignUserRole(Guid id, UserAssignRoleModel assignRoleAttempt)
        {
            User? currUser = userService.GetCurrUser(HttpContext);

            if (currUser == null)
                return NotFound("The current user no longer exists!");

            User? user = userService.GetUserById(id);

            if (user == null)
                return NotFound($"User with id '{id}' could not be found!");

            if (user.Role.NormalizedName == assignRoleAttempt.RoleNormalizedName)
                return StatusCode(403, $"User has already role '{assignRoleAttempt.RoleNormalizedName}'!");

            if (assignRoleAttempt.RoleNormalizedName == "SUPER_ADMIN")
                return StatusCode(403, "Role 'SUPER_ADMIN' is reserved for the root admin only! You cannot assign it to another user!");

            if (user.Role.NormalizedName == "SUPER_ADMIN")
                return StatusCode(403, "The role of the root admin cannot be changed!");

            if (user.Role.NormalizedName == "ADMIN" && currUser.Role.NormalizedName != "SUPER_ADMIN")
                return StatusCode(403, "You cannot re-assign new role to another admin user! This can be done only by the root admin!");

            if (!roleService.AssignUserRole(user, assignRoleAttempt.RoleNormalizedName))
                return NotFound($"Role with normalized name '{assignRoleAttempt.RoleNormalizedName}' could not be found!");

            userService.UpdateUser(user);

            return Ok();
        }

        /* DELETE REQUESTS */

        [HttpDelete("current"), Authorize]
        public IActionResult DeleteCurrUser(UserDeleteModel deleteAttempt)
        {
            User? currUser = userService.GetCurrUser(HttpContext);

            if (currUser == null)
                return NotFound("The current user no longer exists!");

            if (currUser.Role.NormalizedName == "SUPER_ADMIN")
                return StatusCode(403, "The root admin cannot be deleted!");

            if (!ModelState.IsValid)
                return BadRequest();

            if (!userService.IsUserPasswordCorrect(currUser, deleteAttempt.Password))
                return Unauthorized("Incorrect password!");

            // TODO: Delete all data connected to the user [first TODO]
            sessionService.RemoveAllUserSessions(currUser);
            userService.RemoveUser(currUser);

            return Ok();
        }

        [HttpDelete("{id}"), Authorize(Roles = "SUPER_ADMIN,ADMIN")]
        public IActionResult DeleteUserById(Guid id)
        {
            User? currUser = userService.GetCurrUser(HttpContext);

            if (currUser == null)
                return NotFound("The current user no longer exists!");

            User? user = userService.GetUserById(id);

            if (user == null)
                return NotFound($"User with id '{id}' could not be found!");

            if (user.Role.NormalizedName == "SUPER_ADMIN")
                return StatusCode(403, "The root admin cannot be deleted!");

            if (user.Role.NormalizedName == "ADMIN" && currUser.Role.NormalizedName != "SUPER_ADMIN")
                return StatusCode(403, "You cannot delete another admin user! This can be done only by the root admin!");

            // TODO: Delete all user data
            sessionService.RemoveAllUserSessions(user);
            userService.RemoveUser(user);

            return Ok();
        }
    }
}
