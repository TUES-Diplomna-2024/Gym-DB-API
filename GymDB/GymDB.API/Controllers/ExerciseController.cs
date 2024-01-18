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
        private readonly IRoleService roleService;
        private readonly IExerciseService exerciseService;

        public ExerciseController(IUserService userService, IRoleService roleService, IExerciseService exerciseService)
        {
            this.userService = userService;
            this.roleService = roleService;
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

        /* GET REQUESTS */

        [HttpGet("{id}"), Authorize]
        public IActionResult GetExerciseById(Guid id)
        {
            User? currUser = userService.GetCurrUser(HttpContext);

            if (currUser == null)
                return NotFound("The current user no longer exists!");

            Exercise? exercise = exerciseService.GetExerciseById(id);

            if (exercise == null)
                return NotFound($"Exercise with id '{id}' could not be found!");

            if (exercise.UserId != null && exercise.UserId != currUser.Id && !roleService.HasUserAnyRole(currUser, new string[] { "SUPER_ADMIN", "ADMIN" }))
                return StatusCode(403, $"You cannot access exercise with id '{exercise.Id}'!");

            return Ok(new ExerciseInfoModel(exercise));
        }
    }
}
