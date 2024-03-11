using GymDB.API.Data.Entities;
using GymDB.API.Mappers;
using GymDB.API.Models.Workout;
using GymDB.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using GymDB.API.Attributes;

namespace GymDB.API.Controllers
{
    [ApiController]
    [Route("users/current/workouts")]
    public class WorkoutController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IWorkoutService workoutService;

        public WorkoutController(IUserService userService, IWorkoutService workoutService)
        {
            this.userService = userService;
            this.workoutService = workoutService;
        }

        /* POST REQUESTS */

        [HttpPost("create"), CustomAuthorize]
        public IActionResult CreateNewWorkout(WorkoutCreateUpdateModel createAttempt)
        {
            User currUser = userService.GetCurrUser(HttpContext)!;

            if (!ModelState.IsValid)
                return BadRequest();

            Workout workout = createAttempt.ToEntity(currUser);

            workoutService.AddWorkout(workout);

            if (!createAttempt.Exercises!.IsNullOrEmpty())
            {
                Guid[]? notFoundExerciseIds = workoutService.AddExercisesToWorkout(workout, createAttempt.Exercises!, currUser);

                if (notFoundExerciseIds != null)
                {
                    string notFound = string.Join(", ", notFoundExerciseIds.Select(nfi => $"'{nfi}'"));
                    return Ok($"Workout was created without exercises with ids {notFound} because they cannot be found or are not available to you!");
                }
            }

            return Ok();
        }

        /* GET REQUESTS */

        [HttpGet, CustomAuthorize]
        public IActionResult GetCurrUserWorkoutsPreviews()
        {
            User currUser = userService.GetCurrUser(HttpContext)!;

            List<Workout> workouts = workoutService.GetUserWorkouts(currUser);

            return Ok(workoutService.GetWorkoutsPreviews(workouts));
        }

        [HttpGet("{id}"), CustomAuthorize]
        public IActionResult GetWorkoutById(Guid id)
        {
            User currUser = userService.GetCurrUser(HttpContext)!;

            Workout? workout = workoutService.GetWorkoutById(id);

            if (workout == null)
                return NotFound($"Workout with id '{id}' could not be found!");

            if (!workoutService.IsWorkoutOwnedByUser(workout, currUser))
                return StatusCode(403, "You cannot access workouts that are owned by another user!");

            return Ok(workoutService.GetWorkoutWithExercises(workout));
        }

        /* PUT REQUESTS */

        [HttpPut("{id}"), CustomAuthorize]
        public IActionResult UpdateWorkoutById(Guid id, WorkoutCreateUpdateModel updateAttempt)
        {
            User currUser = userService.GetCurrUser(HttpContext)!;

            if (!ModelState.IsValid)
                return BadRequest();

            Workout? workout = workoutService.GetWorkoutById(id);

            if (workout == null)
                return NotFound($"Workout with id '{id}' could not be found!");

            if (!workoutService.IsWorkoutOwnedByUser(workout, currUser))
                return StatusCode(403, "You cannot access workouts that are owned by another user!");

            Guid[]? notFoundExerciseIds = workoutService.UpdateWorkout(workout, updateAttempt, currUser);

            if (notFoundExerciseIds != null)
            {
                string notFound = string.Join(", ", notFoundExerciseIds.Select(nfi => $"'{nfi}'"));
                return Ok($"Workout was updated without adding exercises with ids {notFound} because they cannot be found or are not available to you!");
            }
                
            return Ok();
        }

        /* DELETE REQUESTS */

        [HttpDelete("{id}"), CustomAuthorize]
        public IActionResult DeleteWorkoutById(Guid id)
        {
            User currUser = userService.GetCurrUser(HttpContext)!;

            Workout? workout = workoutService.GetWorkoutById(id);

            if (workout == null)
                return NotFound($"Workout with id '{id}' could not be found!");

            if (!workoutService.IsWorkoutOwnedByUser(workout, currUser))
                return StatusCode(403, "You cannot delete workouts that are owned by another user!");

            workoutService.RemoveWorkout(workout);

            return Ok();
        }
    }
}
