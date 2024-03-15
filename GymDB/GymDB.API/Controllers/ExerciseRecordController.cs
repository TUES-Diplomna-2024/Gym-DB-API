using GymDB.API.Attributes;
using GymDB.API.Data.Entities;
using GymDB.API.Data.Enums;
using GymDB.API.Mappers;
using GymDB.API.Models.Exercise;
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
        public IActionResult CreateNewExerciseRecord(Guid exerciseId, ExerciseRecordCreateUpdateModel createAttempt)
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
            exerciseRecordService.AddRecord(record);

            return Ok();
        }

        /* GET REQUESTS */

        [HttpGet("records"), CustomAuthorize]
        public IActionResult GetCurrUserExerciseRecordsViews(Guid exerciseId, StatisticPeriod period)
        {
            User currUser = userService.GetCurrUser(HttpContext)!;
            Exercise? exercise = exerciseService.GetExerciseById(exerciseId);

            if (exercise == null)
                return NotFound($"Exercise with id '{exerciseId}' could not be found!");

            bool isCurrUserExerciseOwner = exerciseService.IsExerciseOwnedByUser(exercise, currUser);

            if (exercise.IsPrivate && !isCurrUserExerciseOwner)
                return StatusCode(403, "You cannot access custom exercises that are owned by another user!");

            List<ExerciseRecord> records = exerciseRecordService.GetUserExerciseRecordsSince(currUser, exercise, period);

            return Ok(exerciseRecordService.GetRecordsViews(records));
        }

        [HttpGet("stats"), CustomAuthorize]
        public IActionResult GetCurrUserExerciseStatistics(Guid exerciseId, StatisticPeriod period, StatisticMeasurement measurement)
        {
            User currUser = userService.GetCurrUser(HttpContext)!;
            Exercise? exercise = exerciseService.GetExerciseById(exerciseId);

            if (exercise == null)
                return NotFound($"Exercise with id '{exerciseId}' could not be found!");

            bool isCurrUserExerciseOwner = exerciseService.IsExerciseOwnedByUser(exercise, currUser);

            if (exercise.IsPrivate && !isCurrUserExerciseOwner)
                return StatusCode(403, "You cannot access custom exercises that are owned by another user!");

            ExerciseStatisticsModel? statistics = exerciseRecordService.GetUserExerciseStatisticsSince(currUser, exercise, period, measurement);

            return Ok(statistics);
        }

        /* PUT REQUESTS */

        [HttpPut("records/{recordId}"), CustomAuthorize]
        public IActionResult UpdateExerciseRecordById(Guid exerciseId, Guid recordId, ExerciseRecordCreateUpdateModel updateAttempt)
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

            ExerciseRecord? record = exerciseRecordService.GetRecordById(recordId);

            if (record == null)
                return NotFound($"Record with id '{recordId}' could not be found!");

            if (!exerciseRecordService.IsRecordOwnedByUser(record, currUser))
                return StatusCode(403, "You cannot access records that are owned by another user!");

            if (!exerciseRecordService.IsRecordBelongsToExercise(record, exercise))
                return BadRequest("This record does not belong to the specified exercise!");

            exerciseRecordService.UpdateRecord(record, updateAttempt);

            return Ok();
        }

        /* DELETE REQUESTS */
        [HttpDelete("records/{recordId}"), CustomAuthorize]
        public IActionResult DeleteExerciseRecordById(Guid exerciseId, Guid recordId)
        {
            User currUser = userService.GetCurrUser(HttpContext)!;
            Exercise? exercise = exerciseService.GetExerciseById(exerciseId);

            if (exercise == null)
                return NotFound($"Exercise with id '{exerciseId}' could not be found!");

            bool isCurrUserExerciseOwner = exerciseService.IsExerciseOwnedByUser(exercise, currUser);

            if (exercise.IsPrivate && !isCurrUserExerciseOwner)
                return StatusCode(403, "You cannot access custom exercises that are owned by another user!");

            ExerciseRecord? record = exerciseRecordService.GetRecordById(recordId);

            if (record == null)
                return NotFound($"Record with id '{recordId}' could not be found!");

            if (!exerciseRecordService.IsRecordOwnedByUser(record, currUser))
                return StatusCode(403, "You cannot access records that are owned by another user!");

            if (!exerciseRecordService.IsRecordBelongsToExercise(record, exercise))
                return BadRequest("This record does not belong to the specified exercise!");

            exerciseRecordService.RemoveRecord(record);

            return Ok();
        }
    }
}
