using GymDB.API.Data.Entities;
using GymDB.API.Data.Settings;
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
        private readonly ApplicationSettings settings;

        private readonly IUserService userService;
        private readonly IExerciseService exerciseService;
        private readonly IWorkoutService workoutService;

        public WorkoutController(IConfiguration config, IUserService userService, IExerciseService exerciseService, IWorkoutService workoutService)
        {
            settings = new ApplicationSettings(config);

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

            if (!createAttempt.Exercises.IsNullOrEmpty())
            {
                List<Exercise> exercises = exerciseService.GetExercisesByIds(createAttempt.Exercises!);

                if (exercises.Count() != createAttempt.Exercises!.Count())
                {
                    Guid[] foundIds = exercises.Select(exercise => exercise.Id).ToArray();
                    Guid[] notFoundIds = createAttempt.Exercises!.Where(id => !foundIds.Contains(id)).ToArray();

                    return NotFound($"Exercises with ids {string.Join(", ", notFoundIds.Select(nfi => $"'{nfi}'"))} could not be found!");
                }

                workoutService.AddWorkout(workout);
                workoutService.AddExercisesToWorkout(workout, exercises);

            } else
            {
                workoutService.AddWorkout(workout);
            }

            return Ok();
        }

        /* GET REQUESTS */

        /* PUT REQUESTS */

        /* DELETE REQUESTS */
    }
}
