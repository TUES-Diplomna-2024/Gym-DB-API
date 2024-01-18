using GymDB.API.Data.Entities;
using GymDB.API.Models.Exercise;
using GymDB.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymDB.API.Controllers
{
    [ApiController]
    [Route("exercises")]
    public class ExerciseController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IExerciseService exerciseService;

        public ExerciseController(IUserService userService, IExerciseService exerciseService)
        {
            this.userService = userService;
            this.exerciseService = exerciseService;
        }

        /* POST REQUESTS */

        [HttpPost("create"), Authorize(Roles = "SUPER_ADMIN,ADMIN")]
        public IActionResult CreateNewExercise(ExerciseCreateModel createAttempt)
        {
            if (userService.GetCurrUser(HttpContext) == null)
                return NotFound("The current user no longer exists!");

            if (!ModelState.IsValid)
                return BadRequest();

            User? user = userService.GetUserById(createAttempt.UserId ?? Guid.Empty);

            if (createAttempt.UserId != null && user == null)
                return NotFound($"User with id '{createAttempt.UserId}' could not be found!");

            Exercise exercise = new Exercise(createAttempt, user);

            exerciseService.AddExercise(exercise);

            return Ok();
        }
    }
}
