using Microsoft.AspNetCore.Mvc;
using GymDB.API.Services.Interfaces;
using GymDB.API.Models.User;
using GymDB.API.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using GymDB.API.Models;

namespace GymDB.API.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IJwtService jwtService;
        private readonly IRefreshTokenService refreshTokenService;

        public UserController(IUserService userService, IJwtService jwtService, IRefreshTokenService refreshTokenService)
        {
            this.userService = userService;
            this.jwtService = jwtService;
            this.refreshTokenService = refreshTokenService;
        }

        [HttpGet, Authorize]
        public IActionResult GetAllUsers()
        {
            List<UserProfileModel> users = userService.GetAll()
                                                      .Select(user => new UserProfileModel(user))
                                                      .ToList();

            return Ok(users);
        }

        [HttpPost("signup")]
        public IActionResult SignUpNewUser(UserSignUpModel userInput)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            
            if (userService.IsUserAlreadyRegisteredWithEmail(userInput.Email))
                return Conflict();

            RefreshTokenModel refreshToken = refreshTokenService.GenerateNewRefreshToken();
            User user = new User(userInput, refreshToken);
            userService.Add(user);

            return Ok(new AuthModel(jwtService.GenerateNewJwtToken(user), refreshToken.RefreshToken));
        }

        [HttpPost("signin")]
        public IActionResult ValidateSignInAttempt(UserSignInModel login)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            User? user = userService.GetByEmailAndPassword(login.Email, login.Password);

            if (user == null)
                return Unauthorized();

            return Ok(new AuthModel(jwtService.GenerateNewJwtToken(user), user.RefreshToken));
        }

        [HttpPost("refresh")]
        public IActionResult Refresh(AuthModel refreshAttempt)
        {

        }
    }
}
