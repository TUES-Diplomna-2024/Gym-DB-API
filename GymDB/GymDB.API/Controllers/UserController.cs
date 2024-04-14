using Microsoft.AspNetCore.Mvc;
using GymDB.API.Services.Interfaces;
using GymDB.API.Models.User;
using GymDB.API.Data.Entities;
using GymDB.API.Mappers;
using GymDB.API.Data;
using GymDB.API.Attributes;
using GymDB.API.Exceptions;

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

        /* POST REQUESTS */

        [HttpPost("signup")]
        public async Task<IActionResult> SignUpAsync(UserSignUpModel signUpModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            UserAuthModel auth = await userService.SignUpAsync(signUpModel);

            return Ok(auth);
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignInAsync(UserSignInModel signInModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            UserAuthModel auth = await userService.SignInAsync(signInModel);

            return Ok(auth);
        }

        /* GET REQUESTS */

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

        /* TODO: Move to ExerciseController
        [HttpGet("current/custom-exercises"), CustomAuthorize]
        public async Task<IActionResult> GetCurrUserCustomExercisesPreviewsAsync()
        {
            
        }*/

        /* PUT REQUESTS */

        [HttpPut("current"), CustomAuthorize]
        public async Task<IActionResult> UpdateCurrUserAsync(UserUpdateModel updateModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await userService.UpdateCurrUserAsync(HttpContext, updateModel);

            return NoContent();
        }

        
        /*[HttpPut("{id}/role"), CustomAuthorize(Roles = "SUPER_ADMIN,ADMIN")]
        public async Task<IActionResult> AssignUserRoleAsync(Guid id, UserAssignRoleModel assignRoleAttempt)
        {
            
        }*/

        /* DELETE REQUESTS */

        [HttpDelete("current"), CustomAuthorize]
        public async Task<IActionResult> RemoveCurrUserAsync(UserDeleteModel deleteModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await userService.RemoveCurrUserAsync(HttpContext, deleteModel);

            return NoContent();
        }

        [HttpDelete("{id}"), CustomAuthorize(Roles = "SUPER_ADMIN,ADMIN")]
        public async Task<IActionResult> RemoveUserByIdAsync(Guid id)
        {
            return NoContent();
        }
    }
}
