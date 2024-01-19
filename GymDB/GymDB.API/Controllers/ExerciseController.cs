using GymDB.API.Data.Entities;
using GymDB.API.Models.Exercise;
using GymDB.API.Models.User;
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

        [HttpPost("create"), Authorize]
        public IActionResult CreateNewExercise(ExerciseCreateModel createAttempt)
        {
            User? currUser = userService.GetCurrUser(HttpContext);

            if (currUser == null)
                return NotFound("The current user no longer exists!");

            if (!ModelState.IsValid)
                return BadRequest();

            bool isCurrUserAdmin = roleService.HasUserAnyRole(currUser, new string[] { "SUPER_ADMIN", "ADMIN" });

            if (!createAttempt.IsPrivate && !isCurrUserAdmin)
                return StatusCode(403, "Only admin users can create public еxercises!");

            Exercise exercise = new Exercise(createAttempt, currUser);

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

            if (exercise.IsPrivate && exercise.UserId != currUser.Id)
                return StatusCode(403, $"You cannot access the custom exercise with id '{exercise.Id}'! It's owned by another user!");

            if (roleService.HasUserAnyRole(currUser, new string[] { "SUPER_ADMIN", "ADMIN" }))
                return Ok(new ExerciseAdvancedInfoModel(exercise));

            return Ok(new ExerciseNormalInfoModel(exercise));
        }

        [HttpGet, Authorize]
        public IActionResult GetAllPublicExercisesPreviews()
        {
            if (userService.GetCurrUser(HttpContext) == null)
                return NotFound("The current user no longer exists!");

            List<Exercise> publicExercises = exerciseService.GetAllPublicExercises();

            return Ok(exerciseService.GetExercisesPreviews(publicExercises));
        }
    }
}
