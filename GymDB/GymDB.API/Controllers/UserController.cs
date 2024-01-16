using Microsoft.AspNetCore.Mvc;
using GymDB.API.Services.Interfaces;
using GymDB.API.Models.User;
using GymDB.API.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using GymDB.API.Data.Settings;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

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

        [HttpPost("signup")]
        public IActionResult SignUp(UserSignUpModel signUpAttempt)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (signUpAttempt.Username == settings.DBSeed.RootUser.Username)
                return BadRequest($"Username '{signUpAttempt.Username}' is prohibited for use!");

            if (userService.IsUserAlreadyRegisteredWithEmail(signUpAttempt.Email))
                return Conflict();

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
                return Unauthorized();

            string refreshToken = sessionService.CreateNewSession(user);

            return Ok(new UserAuthModel(jwtService.GenerateNewJwtToken(user), refreshToken));
        }

        [HttpPost("signout")]
        public IActionResult SignOut(UserSessionRetrievalModel signOutAttempt)
        {
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

        [HttpGet("current"), Authorize]
        public IActionResult GetCurrUser()
        {
            User? currUser = userService.GetCurrUser(HttpContext);

            if (currUser == null)
                return NotFound($"Current user no longer exists!");

            return Ok(new UserProfileModel(currUser));
        }

        [HttpGet("{id}"), Authorize]
        public IActionResult GetUserById(Guid id)
        {
            User? user = userService.GetUserById(id);

            if (user == null)
                return NotFound($"User with id '{id}' could not be found!");
            
            return Ok(new UserProfileModel(user));
        }

        [HttpGet, Authorize(Roles = "ADMIN")]
        public IActionResult GetAllUsers()
        {

            List<UserProfileModel> users = userService.GetAllUsers()
                                                      .Select(user => new UserProfileModel(user))
                                                      .ToList();

            return Ok(users);
        }

        [HttpPut("current"), Authorize]
        public IActionResult UpdateCurrUser(UserUpdateModel update)
        {
            User? currUser = userService.GetCurrUser(HttpContext);

            if (currUser == null)
                return NotFound("Current user no longer exists!");

            if (!ModelState.IsValid)
                return BadRequest();

            if (update.Username == settings.DBSeed.RootUser.Username)
                return BadRequest($"Username '{update.Username}' is prohibited for use!");

            userService.UpdateUser(currUser, update);

            return Ok();
        }

        [HttpPut("{id}/role"), Authorize(Roles = "ADMIN")]
        public IActionResult AssignUserRole(Guid id, UserAssignRoleModel assignRoleAttempt)
        {
            User? user = userService.GetUserById(id);
            User? currUser = userService.GetCurrUser(HttpContext);

            if (user == null)
                return NotFound($"User with id '{id}' could not be found!");

            if (currUser == null)
                return NotFound("Current user no longer exists!");

            if (user.Role.NormalizedName == assignRoleAttempt.RoleNormalizedName)
                return BadRequest($"User has already role '{assignRoleAttempt.RoleNormalizedName}'!");

            if (user.Role.NormalizedName == "ADMIN" && currUser.Username != settings.DBSeed.RootUser.Username)
                return BadRequest("You can't re-assign new role to an admin user! That is only possible if you are root admin!");

            if (!roleService.AssignUserRole(user, assignRoleAttempt.RoleNormalizedName))
                return NotFound($"Role with normalized name '{assignRoleAttempt.RoleNormalizedName}' could not be found!");

            userService.UpdateUser(user);

            return Ok();
        }
    }
}
