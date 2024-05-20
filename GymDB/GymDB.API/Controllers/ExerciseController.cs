using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using GymDB.API.Attributes;
using GymDB.API.Data.Enums;
using GymDB.API.Models.Exercise;
using GymDB.API.Services.Interfaces;

namespace GymDB.API.Controllers
{
    [ApiController]
    [Route("exercises")]
    public class ExerciseController : ControllerBase
    {
        private readonly IExerciseService exerciseService;

        public ExerciseController(IExerciseService exerciseService)
        {
            this.exerciseService = exerciseService;
        }

        /* ======================================================================== POST REQUESTS */

        [HttpPost("create"), CustomAuthorize]
        public async Task<IActionResult> CreateNewExerciseAsync([FromForm] ExerciseCreateModel createModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await exerciseService.CreateNewExerciseAsync(HttpContext, createModel);

            return NoContent();
        }


        /* ======================================================================== GET REQUESTS */

        [HttpGet("{exerciseId}"), CustomAuthorize]
        public async Task<IActionResult> GetExerciseViewByIdAsync(Guid exerciseId)
        {
            ExerciseViewModel exerciseView = await exerciseService.GetExerciseViewByIdAsync(HttpContext, exerciseId);

            return Ok(exerciseView);
        }


        [HttpGet("/users/current/custom-exercises"), CustomAuthorize]
        public async Task<IActionResult> GetCurrUserCustomExercisesPreviewsAsync()
        {
            List<ExercisePreviewModel> customExercises = await exerciseService.GetCurrUserCustomExercisesPreviewsAsync(HttpContext);

            return Ok(customExercises);
        }


        [HttpGet("search"), CustomAuthorize]
        public async Task<IActionResult> FindPublicAndCustomExercisesPreviewsAsync([FromQuery] string name)
        {
            List<ExercisePreviewModel> searchResults = await exerciseService.FindPublicAndCustomExercisesPreviewsAsync(HttpContext, name);

            return Ok(searchResults);
        }


        [HttpGet("advanced-search"), CustomAuthorize(Roles = "SUPER_ADMIN,ADMIN")]
        public async Task<IActionResult> FindAdminCreatedExercisesPreviewsAsync([FromQuery] ExerciseSearchModel searchModel)
        {
            List<ExercisePreviewModel> searchResults = await exerciseService.FindAdminCreatedExercisesPreviewsAsync(searchModel);

            return Ok(searchResults);
        }


        /* ======================================================================== PUT REQUESTS */

        [HttpPut("{exerciseId}"), CustomAuthorize]
        public async Task<IActionResult> UpdateExerciseByIdAsync(Guid exerciseId, [FromForm] ExerciseUpdateModel updateModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await exerciseService.UpdateExerciseByIdAsync(HttpContext, exerciseId, updateModel);

            return NoContent();
        }


        [HttpPut("{exerciseId}/visibility"), CustomAuthorize(Roles = "SUPER_ADMIN,ADMIN")]
        public async Task<IActionResult> UpdateExerciseVisibilityAsync(Guid exerciseId, [FromForm, Required] ExerciseVisibility visibility)
        {
            await exerciseService.UpdateExerciseVisibilityAsync(exerciseId, visibility);

            return NoContent();
        }


        /* ======================================================================== DELETE REQUESTS */

        [HttpDelete("{exerciseId}"), CustomAuthorize]
        public async Task<IActionResult> RemoveExerciseByIdAsync(Guid exerciseId)
        {
            await exerciseService.RemoveExerciseByIdAsync(HttpContext, exerciseId);

            return NoContent();
        }
    }
}
