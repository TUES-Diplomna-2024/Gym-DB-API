using Microsoft.AspNetCore.Mvc;
using GymDB.API.Attributes;
using GymDB.API.Data.Enums;
using GymDB.API.Models.ExerciseRecord;
using GymDB.API.Services.Interfaces;
using GymDB.API.Models.Exercise;

namespace GymDB.API.Controllers
{
    [ApiController]
    [Route("exercises/{exerciseId}")]
    public class ExerciseRecordController : ControllerBase
    {
        private readonly IExerciseRecordService exerciseRecordService;

        public ExerciseRecordController(IExerciseRecordService exerciseRecordService)
        {
            this.exerciseRecordService = exerciseRecordService;
        }

        /* ======================================================================== POST REQUESTS */

        [HttpPost("records/create"), CustomAuthorize]
        public async Task<IActionResult> CreateNewExerciseRecordAsync(Guid exerciseId, ExerciseRecordCreateUpdateModel createModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await exerciseRecordService.CreateNewExerciseRecordAsync(HttpContext, exerciseId, createModel);

            return NoContent();
        }


        /* ======================================================================== GET REQUESTS */

        [HttpGet("records"), CustomAuthorize]
        public async Task<IActionResult> GetCurrUserExerciseRecordsViewsAsync(Guid exerciseId, StatisticPeriod period)
        {
            List<ExerciseRecordViewModel> recordsViews = await exerciseRecordService.GetCurrUserExerciseRecordsViewsAsync(HttpContext, exerciseId, period);

            return Ok(recordsViews);
        }


        [HttpGet("stats"), CustomAuthorize]
        public async Task<IActionResult> GetCurrUserExerciseStatisticsAsync(Guid exerciseId, StatisticPeriod period, StatisticMeasurement measurement)
        {
            ExerciseStatisticsModel? statistics = await exerciseRecordService.GetCurrUserExerciseStatisticsAsync(HttpContext, exerciseId, period, measurement);

            return Ok(statistics);
        }


        /* ======================================================================== PUT REQUESTS */

        [HttpPut("/exercises/records/{recordId}"), CustomAuthorize]
        public async Task<IActionResult> UpdateExerciseRecordByIdAsync(Guid recordId, ExerciseRecordCreateUpdateModel updateModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await exerciseRecordService.UpdateExerciseRecordByIdAsync(HttpContext, recordId, updateModel);
            
            return NoContent();
        }


        /* ======================================================================== DELETE REQUESTS */

        [HttpDelete("/exercises/records/{recordId}"), CustomAuthorize]
        public async Task<IActionResult> RemoveExerciseRecordByIdAsync(Guid recordId)
        {
            await exerciseRecordService.RemoveExerciseRecordByIdAsync(HttpContext, recordId);

            return NoContent();
        }
    }
}
