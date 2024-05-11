using Microsoft.AspNetCore.Mvc;
using GymDB.API.Attributes;
using GymDB.API.Models.Workout;
using GymDB.API.Services.Interfaces;
using GymDB.API.Data.Entities;

namespace GymDB.API.Controllers
{
    [ApiController]
    [Route("users/current/workouts")]
    public class WorkoutController : ControllerBase
    {
        private readonly IWorkoutService workoutService;

        public WorkoutController(IWorkoutService workoutService)
        {
            this.workoutService = workoutService;
        }

        /* ======================================================================== POST REQUESTS */

        [HttpPost("create"), CustomAuthorize]
        public async Task<IActionResult> CreateNewWorkoutAsync(WorkoutCreateModel createModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await workoutService.CreateNewWorkoutAsync(HttpContext, createModel);

            return NoContent();
        }


        /* ======================================================================== GET REQUESTS */

        [HttpGet("{workoutId}"), CustomAuthorize]
        public async Task<IActionResult> GetWorkoutViewByIdAsync(Guid workoutId)
        {
            WorkoutViewModel workoutView = await workoutService.GetWorkoutViewByIdAsync(HttpContext, workoutId);

            return Ok(workoutView);
        }


        [HttpGet, CustomAuthorize]
        public async Task<IActionResult> GetCurrUserWorkoutsPreviewsAsync()
        {
            List<WorkoutPreviewModel> workoutsPreviews = await workoutService.GetCurrUserWorkoutsPreviewsAsync(HttpContext);

            return Ok(workoutsPreviews);
        }


        /* ======================================================================== PUT REQUESTS */

        [HttpPut("{workoutId}"), CustomAuthorize]
        public async Task<IActionResult> UpdateWorkoutByIdAsync(Guid workoutId, WorkoutUpdateModel updateModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await workoutService.UpdateWorkoutByIdAsync(HttpContext, workoutId, updateModel);

            return NoContent();
        }


        [HttpPost("/exercises/{exerciseId}/add-to-workouts"), CustomAuthorize]
        public async Task<IActionResult> AddExerciseToWorkoutsAsync(Guid exerciseId, List<Guid> workoutsIds)
        {
            await workoutService.AddExerciseToWorkoutsAsync(HttpContext, exerciseId, workoutsIds);

            return NoContent();
        }


        /* ======================================================================== DELETE REQUESTS */

        [HttpDelete("{workoutId}"), CustomAuthorize]
        public async Task<IActionResult> RemoveWorkoutByIdAsync(Guid workoutId)
        {
            await workoutService.RemoveWorkoutByIdAsync(HttpContext, workoutId);

            return NoContent();
        }
    }
}
