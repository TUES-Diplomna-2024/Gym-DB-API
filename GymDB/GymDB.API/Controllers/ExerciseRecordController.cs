using GymDB.API.Attributes;
using GymDB.API.Data.Entities;
using GymDB.API.Mapping;
using GymDB.API.Models.ExerciseRecord;
using GymDB.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymDB.API.Controllers
{
    [ApiController]
    [Route("exercises/{exerciseId}")]
    public class ExerciseRecordController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IExerciseService exerciseService;
        private readonly IExerciseRecordService exerciseRecordService;

        public ExerciseRecordController(IUserService userService, IExerciseService exerciseService, IExerciseRecordService exerciseRecordService)
        {
            this.userService = userService;
            this.exerciseService = exerciseService;
            this.exerciseRecordService = exerciseRecordService;
        }

        /* POST REQUESTS */

        [HttpPost("records/create"), CustomAuthorize]
        public IActionResult CreateNewExerciseRecord(Guid exerciseId, ExerciseRecordCreateModel createAttempt)
        {
            User currUser = userService.GetCurrUser(HttpContext)!;

            if (!ModelState.IsValid)
                return BadRequest();

            Exercise? exercise = exerciseService.GetExerciseById(exerciseId);

            if (exercise == null)
                return NotFound($"Exercise with id '{exerciseId}' could not be found!");

            bool isCurrUserExerciseOwner = exerciseService.IsExerciseOwnedByUser(exercise, currUser);

            if (exercise.IsPrivate && !isCurrUserExerciseOwner)
                return StatusCode(403, "You cannot access custom exercises that are owned by another user!");

            ExerciseRecord record = createAttempt.ToEntity(exercise, currUser);
            exerciseRecordService.AddExercise(record);

            return Ok();
        }

        /* GET REQUESTS */

        [HttpGet("records/{recordId}"), CustomAuthorize]
        public IActionResult GetExerciseRecordById(Guid exerciseId, Guid recordId)
        {
            return Ok();
        }

        [HttpGet("records"), CustomAuthorize]
        public IActionResult GetCurrUserExerciseRecords(Guid exerciseId)
        {
            return Ok();
        }

        [HttpGet("stats"), CustomAuthorize]
        public IActionResult GetCurrUserExerciseStats(Guid exerciseId)
        {
            return Ok();
        }

        /* PUT REQUESTS */

        [HttpPut("/records/{recordId}"), CustomAuthorize]
        public IActionResult UpdateExerciseRecordById(Guid exerciseId, Guid recordId)
        {
            return Ok();
        }

        /* DELETE REQUESTS */
        [HttpDelete("/records/{recordId}"), CustomAuthorize]
        public IActionResult DeleteExerciseRecordById(Guid recordId)
        {
            return Ok();
        }
    }
}
