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

        UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost("signup")]
        IActionResult SignUpNewUser(UserSignUpModel userInput)
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
        IActionResult ValidateSignInAttempt(UserSignInModel login)
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
