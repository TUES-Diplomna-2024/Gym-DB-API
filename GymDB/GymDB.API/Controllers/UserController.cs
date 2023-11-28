using Microsoft.AspNetCore.Mvc;
using GymDB.API.Services.Interfaces;

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

        /*[HttpPost("signin")]
        IActionResult SignInNewUser()
        {

        }*/
    }
}
