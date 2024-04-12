using Microsoft.AspNetCore.Mvc;
/*using GymDB.API.Services.Interfaces;*/
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
        public UserController()
        {
           
        }

        /* POST REQUESTS */

        [HttpPost("signup")]
        public async Task<IActionResult> SignUpAsync(UserSignUpModel signUpAttempt)
        {
            
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignInAsync(UserSignInModel signInAttempt)
        {
            
        }

        /* GET REQUESTS */

        [HttpGet("refresh"), RefreshTokenRequired]
        public async Task<IActionResult> RefreshAsync()
        {
            
        }

        [HttpGet("current"), CustomAuthorize]
        public async Task<IActionResult> GetCurrUserAsync()
        {
            
        }

        [HttpGet("{id}"), CustomAuthorize]
        public async Task<IActionResult> GetUserByIdAsync(Guid id)
        {
           
        }

        [HttpGet, CustomAuthorize(Roles = "SUPER_ADMIN,ADMIN")]
        public async Task<IActionResult> GetAllUserPreviewsAsync()
        {
            
        }

        [HttpGet("current/custom-exercises"), CustomAuthorize]
        public async Task<IActionResult> GetCurrUserCustomExercisesPreviewsAsync()
        {
            
        }

        /* PUT REQUESTS */

        [HttpPut("current"), CustomAuthorize]
        public async Task<IActionResult> UpdateCurrUserAsync(UserUpdateModel updateAttempt)
        {
            
        }

        [HttpPut("{id}/role"), CustomAuthorize(Roles = "SUPER_ADMIN,ADMIN")]
        public async Task<IActionResult> AssignUserRoleAsync(Guid id, UserAssignRoleModel assignRoleAttempt)
        {
            
        }

        /* DELETE REQUESTS */

        [HttpDelete("current"), CustomAuthorize]
        public async Task<IActionResult> DeleteCurrUserAsync(UserDeleteModel deleteAttempt)
        {
            
        }

        [HttpDelete("{id}"), CustomAuthorize(Roles = "SUPER_ADMIN,ADMIN")]
        public async Task<IActionResult> DeleteUserByIdAsync(Guid id)
        {
            
        }
    }
}
