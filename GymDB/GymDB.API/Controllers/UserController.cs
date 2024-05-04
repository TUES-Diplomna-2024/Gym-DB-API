using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using GymDB.API.Services.Interfaces;
using GymDB.API.Models.User;
using GymDB.API.Attributes;
using GymDB.API.Data.Enums;
using GymDB.API.Data.ValidationAttributes;

namespace GymDB.API.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IRoleService roleService;

        public UserController(IUserService userService, IRoleService roleService)
        {
            this.userService = userService;
            this.roleService = roleService;
        }

        /* ======================================================================== POST REQUESTS */

        [HttpPost("signup")]
        public async Task<IActionResult> SignUpAsync(UserSignUpModel signUpModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            UserAuthModel auth = await userService.SignUpAsync(signUpModel);

            return Ok(auth);
        }


        [HttpPost("signin")]
        public async Task<IActionResult> SignInAsync(UserSignInModel signInModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            UserAuthModel auth = await userService.SignInAsync(signInModel);

            return Ok(auth);
        }


        /* ======================================================================== GET REQUESTS */

        // Since the RefreshTokenRequired attribute is applied to this endpoint, the RefreshTokenMiddleware has already validated the refresh token,
        // ensuring that the current user is not null. Therefore, a new access token for the current user can be safely generated.
        
        [HttpGet("refresh"), RefreshTokenRequired]
        public async Task<IActionResult> RefreshAsync()
        {
            string accessToken = await userService.RefreshAsync(HttpContext);

            return Ok(accessToken);
        }


        // Since the CustomAuthorize attribute is applied to this endpoint, the AccessTokenMiddleware has already validated the access token,
        // ensuring that the current user is not null.

        [HttpGet("current"), CustomAuthorize]
        public async Task<IActionResult> GetCurrUserProfileAsync()
        {
            UserProfileModel userProfile = await userService.GetCurrUserProfileAsync(HttpContext);

            return Ok(userProfile);
        }
        

        [HttpGet("{userId}"), CustomAuthorize]
        public async Task<IActionResult> GetUserProfileByIdAsync(Guid userId)
        {
            UserProfileModel userProfile = await userService.GetUserProfileByIdAsync(userId);

            return Ok(userProfile);
        }
        

        [HttpGet, CustomAuthorize(Roles = "SUPER_ADMIN,ADMIN")]
        public async Task<IActionResult> GetAllUsersPreviewsAsync()
        {
            List<UserPreviewModel> usersPreviews = await userService.GetAllUsersPreviewsAsync();

            return Ok(usersPreviews);
        }


        /* TODO - Move to ExerciseController
        [HttpGet("current/custom-exercises"), CustomAuthorize]
        public async Task<IActionResult> GetCurrUserCustomExercisesPreviewsAsync() { } */

        /* ======================================================================== PUT REQUESTS */

        [HttpPut("current"), CustomAuthorize]
        public async Task<IActionResult> UpdateCurrUserAsync(UserUpdateModel updateModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await userService.UpdateCurrUserAsync(HttpContext, updateModel);

            return NoContent();
        }


        [HttpPut("{userId}/role"), CustomAuthorize(Roles = "SUPER_ADMIN,ADMIN")]
        public async Task<IActionResult> AssignUserNewRoleAsync(Guid userId, [FromForm, Required] AssignableRole role)
        {
            await roleService.AssignUserNewRoleAsync(HttpContext, userId, role);

            return NoContent();
        }


        /* ======================================================================== DELETE REQUESTS */

        [HttpDelete("current"), CustomAuthorize]
        public async Task<IActionResult> RemoveCurrUserAsync([FromForm, Password] string password)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await userService.RemoveCurrUserAsync(HttpContext, password);

            return NoContent();
        }


        [HttpDelete("{userId}"), CustomAuthorize(Roles = "SUPER_ADMIN,ADMIN")]
        public async Task<IActionResult> RemoveUserByIdAsync(Guid userId)
        {
            await userService.RemoveUserByIdAsync(HttpContext, userId);

            return NoContent();
        }
    }
}
