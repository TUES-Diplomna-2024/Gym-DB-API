using Microsoft.AspNetCore.Mvc;
using GymDB.API.Services.Interfaces;
using GymDB.API.Models.User;
using GymDB.API.Data.Entities;

namespace GymDB.API.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            List<UserCompressedInfoModel> users = userService.GetAll()
                                                             .Select(user => new UserCompressedInfoModel(user))
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

            User user = new User(userInput);
            userService.Add(user);

            return Ok();
        }

        [HttpPost("signin")]
        public IActionResult ValidateSignInAttempt(UserSignInModel login)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            User? user = userService.GetByEmailAndPassword(login.Email, login.Password);

            if (user == null)
                return Unauthorized();

            return Ok(new UserCompressedInfoModel(user));
        }
    }
}
