using GymDB.API.Data.Entities;
using GymDB.API.Data.Enums;
using GymDB.API.Mappers;
using GymDB.API.Models.Exercise;
using GymDB.API.Models.ExerciseRecord;
using GymDB.API.Repositories.Interfaces;
using GymDB.API.Services.Interfaces;
using GymDB.API.Exceptions;

namespace GymDB.API.Services
{
    public class ExerciseRecordService : IExerciseRecordService
    {
        private readonly IExerciseRecordRepository exerciseRecordRepository;
        private readonly IExerciseService exerciseService;
        private readonly IUserRepository userRepository;

        public ExerciseRecordService(IExerciseRecordRepository exerciseRecordRepository, IExerciseService exerciseService, IUserRepository userRepository)
        {
            this.exerciseRecordRepository = exerciseRecordRepository;
            this.exerciseService = exerciseService;
            this.userRepository = userRepository;
        }

        public async Task CreateNewExerciseRecordAsync(HttpContext context, Guid exerciseId, ExerciseRecordCreateUpdateModel createModel)
        {
            // If the user has access to a specific exercise, he can create a record for it
            User currUser = await userRepository.GetCurrUserAsync(context);
            Exercise exercise = await exerciseService.GetExerciseByIdAsync(currUser, exerciseId);

            ExerciseRecord record = createModel.ToEntity(exercise, currUser);

            await exerciseRecordRepository.AddExerciseRecordAsync(record);
        }

        public async Task<List<ExerciseRecordViewModel>> GetCurrUserExerciseRecordsViewsAsync(HttpContext context, Guid exerciseId, StatisticPeriod period)
        {
            List<ExerciseRecord> records = await GetAllUserExerciseRecordsSinceAsync(context, exerciseId, period);

            return records.Select(record => record.ToViewModel()).ToList();
        }

        public async Task<ExerciseStatisticsModel> GetCurrUserExerciseStatisticsAsync(HttpContext context, Guid exerciseId, StatisticPeriod period, StatisticMeasurement measurement)
        {
            List<ExerciseRecord> records = await GetAllUserExerciseRecordsSinceAsync(context, exerciseId, period);

            if (records.Count == 0)
                throw new OkException("No statistics available for the selected time period!");

            uint totalSets = 0, totalReps = 0, totalDuration = 0;
            double totalVolume = 0, totalWeight = 0;
            double maxVolume = 0, maxWeight = 0;

            foreach (var record in records)
            {
                totalSets += record.Sets;
                totalReps += record.Reps;
                totalDuration += record.Duration;
                totalVolume += record.Volume;
                totalWeight += record.Weight;

                if (record.Volume > maxVolume) maxVolume = record.Volume;
                if (record.Weight > maxWeight) maxWeight = record.Weight;
            }

            double avgRepsPerSet = totalSets != 0 ? totalReps / totalSets : 0;
            double avgTrainingDuration = totalDuration / records.Count;
            double avgVolume = totalVolume / records.Count;
            double avgWeight = totalWeight / records.Count;

            List<StatisticDataPoint> dataPoints = records.Select(record => record.ToStatisticDataPoint(measurement))
                                                         .ToList();

            return new ExerciseStatisticsModel
            {
                TotalSets = totalSets,
                TotalReps = totalReps,

                AvgRepsPerSet = avgRepsPerSet,
                AvgTrainingDuration = avgTrainingDuration,
                AvgVolume = avgVolume,
                AvgWeight = avgWeight,

                MaxVolume = maxVolume,
                MaxWeight = maxWeight,

                DataPoints = dataPoints
            };
        }

        public async Task UpdateExerciseRecordByIdAsync(HttpContext context, Guid recordId, ExerciseRecordCreateUpdateModel updateModel)
        {
            ExerciseRecord record = await GetExerciseRecordByIdAsync(context, recordId);

            record.ApplyUpdateModel(updateModel);
            await exerciseRecordRepository.UpdateExerciseRecordAsync(record);
        }

        public async Task RemoveExerciseRecordByIdAsync(HttpContext context, Guid recordId)
        {
            ExerciseRecord record = await GetExerciseRecordByIdAsync(context, recordId);
            await exerciseRecordRepository.RemoveExerciseRecordAsync(record);
        }

        private async Task<ExerciseRecord> GetExerciseRecordByIdAsync(HttpContext context, Guid recordId)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);
            ExerciseRecord? record = await exerciseRecordRepository.GetExerciseRecordByIdAsync(recordId);

            if (record == null)
                throw new NotFoundException("The specified exercise record could not be found!");

            if (!IsExerciseRecordOwnedByUser(record, currUser))
                throw new ForbiddenException("You cannot access exercise records that are owned by another user!");

            return record;
        }

        private async Task<List<ExerciseRecord>> GetAllUserExerciseRecordsSinceAsync(HttpContext context, Guid exerciseId, StatisticPeriod period)
        {
            // Validates if user can access the exercise
            User currUser = await userRepository.GetCurrUserAsync(context);
            await exerciseService.GetExerciseByIdAsync(currUser, exerciseId);

            return await exerciseRecordRepository.GetAllUserExerciseRecordsSinceAsync(currUser.Id, exerciseId, period);
        }

        private bool IsExerciseRecordOwnedByUser(ExerciseRecord record, User user)
            => record.OwnerId == user.Id;
    }
}
