using GymDB.API.Data.Entities;
using GymDB.API.Data.Settings;
using GymDB.API.Models.User;
using GymDB.API.Models.Workout;
using GymDB.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace GymDB.API.Controllers
{
    [ApiController]
    [Route("users/current/workouts")]
    public class WorkoutController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IExerciseService exerciseService;
        private readonly IWorkoutService workoutService;

        public WorkoutController(IUserService userService, IExerciseService exerciseService, IWorkoutService workoutService)
        {
            this.userService = userService;
            this.exerciseService = exerciseService;
            this.workoutService = workoutService;
        }

        /* POST REQUESTS */

        [HttpPost("create"), Authorize]
        public IActionResult CreateNewWorkout(WorkoutCreateModel createAttempt)
        {
            User? currUser = userService.GetCurrUser(HttpContext);

            if (currUser == null)
                return NotFound("The current user no longer exists!");

            if (!ModelState.IsValid)
                return BadRequest();

            Workout workout = new Workout(createAttempt, currUser);

            workoutService.AddWorkout(workout);

            if (!createAttempt.Exercises.IsNullOrEmpty())
            {
                List<Exercise> exercises = exerciseService.GetUserAccessibleExercisesByIds(createAttempt.Exercises!, currUser);

                workoutService.AddExercisesToWorkout(workout, exercises);

                if (exercises.Count() != createAttempt.Exercises!.Count())
                {
                    Guid[] foundIds = exercises.Select(exercise => exercise.Id).ToArray();
                    Guid[] notFoundIds = createAttempt.Exercises!.Where(id => !foundIds.Contains(id)).ToArray();

                    return Ok($"Workout was created without exercises with ids {string.Join(", ", notFoundIds.Select(nfi => $"'{nfi}'"))} because they cannot be found or are not available to you!");
                }
            }

            return Ok();
        }

        /* GET REQUESTS */

        [HttpGet, Authorize]
        public IActionResult GetCurrUserWorkoutsPreviews()
        {
            User? currUser = userService.GetCurrUser(HttpContext);

            if (currUser == null)
                return NotFound("The current user no longer exists!");

            List<Workout> workouts = workoutService.GetUserWorkouts(currUser);

            return Ok(workoutService.GetWorkoutsPreviews(workouts));
        }

        [HttpGet("{id}"), Authorize]
        public IActionResult GetWorkoutById(Guid id)
        {
            User? currUser = userService.GetCurrUser(HttpContext);

            if (currUser == null)
                return NotFound("The current user no longer exists!");

            Workout? workout = workoutService.GetWorkoutById(id);

            if (workout == null)
                return NotFound($"Workout with id '{id}' could not be found!");

            if (workout.UserId != currUser.Id)
                return StatusCode(403, "You cannot access workouts that are owned by another user!");

            return Ok(workoutService.GetWorkoutWithExercises(workout));
        }

        /* PUT REQUESTS */

        /* DELETE REQUESTS */
    }
}
