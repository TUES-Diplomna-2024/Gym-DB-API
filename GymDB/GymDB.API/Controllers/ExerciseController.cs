using GymDB.API.Data.Entities;
using GymDB.API.Mapping;
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

            Exercise exercise = createAttempt.ToEntity(currUser);

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

            bool isCurrUserAdmin = roleService.HasUserAnyRole(currUser, new string[] { "SUPER_ADMIN", "ADMIN" });
            bool isCurrUserExerciseOwner = exerciseService.IsExerciseOwnedByUser(exercise, currUser);

            if (exercise.IsPrivate && !isCurrUserExerciseOwner && !isCurrUserAdmin)
                return StatusCode(403, "You cannot access custom exercises that are owned by another user!");

            if (isCurrUserAdmin)
                return Ok(exercise.ToAdvancedInfoModel(isCurrUserExerciseOwner ? null : exercise.User));

            return Ok(exercise.ToNormalInfoModel(isCurrUserExerciseOwner ? null : exercise.User));
        }

        [HttpGet, Authorize]
        public IActionResult GetAllPublicExercisesPreviews()
        {
            if (userService.GetCurrUser(HttpContext) == null)
                return NotFound("The current user no longer exists!");

            List<Exercise> publicExercises = exerciseService.GetAllPublicExercises();

            return Ok(exerciseService.GetExercisesPreviews(publicExercises));
        }

        [HttpGet("private"), Authorize(Roles = "SUPER_ADMIN,ADMIN")]
        public IActionResult GetAllPrivateAppExercisesPreviews()
        {
            if (userService.GetCurrUser(HttpContext) == null)
                return NotFound("The current user no longer exists!");

            List<Exercise> privateExercises = exerciseService.GetAllPrivateAppExercises();

            return Ok(exerciseService.GetExercisesPreviews(privateExercises));
        }

        [HttpGet("search"), Authorize]
        public IActionResult GetExerciseSearchResults([FromQuery] ExerciseSearchModel search)
        {
            User? currUser = userService.GetCurrUser(HttpContext);

            if (currUser == null)
                return NotFound("The current user no longer exists!");

            List<Exercise> results = exerciseService.GetExercisesBySearch(search, currUser);

            return Ok(exerciseService.GetExercisesPreviews(results));
        }

        /* PUT REQUESTS */

        [HttpPut("{id}"), Authorize]
        public IActionResult UpdateExerciseById(Guid id, ExerciseUpdateModel updateAttempt)
        {
            User? currUser = userService.GetCurrUser(HttpContext);

            if (currUser == null)
                return NotFound("The current user no longer exists!");

            if (!ModelState.IsValid)
                return BadRequest();

            Exercise? exercise = exerciseService.GetExerciseById(id);

            if (exercise == null)
                return NotFound($"Exercise with id '{id}' could not be found!");

            bool isCurrUserAdmin = roleService.HasUserAnyRole(currUser, new string[] { "SUPER_ADMIN", "ADMIN" });

            // Non-admin users cannot update public exercises
            if (!exercise.IsPrivate && !isCurrUserAdmin)
                return StatusCode(403, "Only admin users can update public exercises!");

            // Only owners can update their private custom exercises
            if (exercise.IsPrivate && !exerciseService.IsExerciseOwnedByUser(exercise, currUser))
                return StatusCode(403, "You cannot update private exercise that is owned by another user!");

            exerciseService.UpdateExercise(exercise, updateAttempt);

            return Ok();
        }

        [HttpPut("{id}/visibility"), Authorize(Roles = "SUPER_ADMIN,ADMIN")]
        public IActionResult ChangeExerciseVisibility(Guid id, ExerciseChangeVisModel updateVisAttempt)
        {
            if (userService.GetCurrUser(HttpContext) == null)
                return NotFound("The current user no longer exists!");

            Exercise? exercise = exerciseService.GetExerciseById(id);

            if (exercise == null)
                return NotFound($"Exercise with id '{id}' could not be found!");

            if (exercise.IsPrivate == updateVisAttempt.IsPrivate)
            {
                string state = exercise.IsPrivate ? "private" : "public";
                return StatusCode(403, $"Exercise with id '{id}' is already {state}!");
            }

            exerciseService.UpdateExerciseVisibility(exercise, updateVisAttempt.IsPrivate);

            return Ok();
        }

        /* DELETE REQUESTS */

        [HttpDelete("{id}"), Authorize]
        public IActionResult DeleteExerciseById(Guid id)
        {
            User? currUser = userService.GetCurrUser(HttpContext);

            if (currUser == null)
                return NotFound("The current user no longer exists!");

            Exercise? exercise = exerciseService.GetExerciseById(id);

            if (exercise == null)
                return NotFound($"Exercise with id '{id}' could not be found!");

            bool isCurrUserAdmin = roleService.HasUserAnyRole(currUser, new string[] { "SUPER_ADMIN", "ADMIN" });

            if (!exercise.IsPrivate && !isCurrUserAdmin)
                return StatusCode(403, "Only admin users can delete public exercises!");

            if (exercise.IsPrivate && exercise.UserId != null && !exerciseService.IsExerciseOwnedByUser(exercise, currUser))
                return StatusCode(403, "You cannot delete custom exercise that is owned by another user!");

            exerciseService.RemoveExercise(exercise);

            return Ok();
        }
    }
}
