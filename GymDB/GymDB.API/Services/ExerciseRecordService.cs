using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Data.Enums;
using GymDB.API.Mapping;
using GymDB.API.Models.Exercise;
using GymDB.API.Models.ExerciseRecord;
using GymDB.API.Services.Interfaces;

namespace GymDB.API.Services
{
    public class ExerciseRecordService : IExerciseRecordService
    {
        private readonly ApplicationContext context;

        public ExerciseRecordService(ApplicationContext context)
        {
            this.context = context;
        }

        public List<ExerciseRecord> GetUserExerciseRecordsSince(User user, Exercise exercise, StatisticPeriod period)
        {
            DateTime startDate = GetStartDate(period);

            return context.ExerciseRecords.Where(er => er.ExerciseId == exercise.Id &&
                                                       er.UserId == user.Id &&
                                                       er.OnCreated >= startDate)
                                          .OrderByDescending(er => er.OnCreated)
                                          .ToList();
        }

        public List<ExerciseRecordViewModel> GetRecordsViews(List<ExerciseRecord> records)
            => records.Select(r => r.ToViewModel()).ToList();

        public ExerciseRecord? GetRecordById(Guid id)
            => context.ExerciseRecords.FirstOrDefault(er => er.Id == id);

        public ExerciseStatisticsModel? GetUserExerciseStatisticsSince(User user, Exercise exercise, StatisticPeriod period, StatisticMeasurement measurement)
        {
            List<ExerciseRecord> records = GetUserExerciseRecordsSince(user, exercise, period);
            
            if (records.Count == 0) return null;

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

            return new ExerciseStatisticsModel {
                TotalSets = totalSets,
                TotalReps = totalReps,

                AvgRepsPerSet = avgRepsPerSet,
                AvgTrainingDuration = avgTrainingDuration,
                AvgVolume = avgVolume,
                AvgWeight = avgWeight,

                MaxVolume = maxVolume,
                MaxWeight = maxWeight,

                DataPoints = GetStatisticDataPoints(records, measurement)
            };
        }

        public bool IsRecordBelongsToExercise(ExerciseRecord record, Exercise exercise)
            => record.ExerciseId == exercise.Id;

        public bool IsRecordOwnedByUser(ExerciseRecord record, User user)
            => record.UserId == user.Id;

        public void AddRecord(ExerciseRecord record)
        {
            context.ExerciseRecords.Add(record);
            context.SaveChanges();
        }

        public void UpdateRecord(ExerciseRecord record, ExerciseRecordCreateUpdateModel update)
        {
            record.Sets = update.Sets;
            record.Reps = update.Reps;
            record.Weight = update.Weight ?? 0;
            record.Volume = update.Weight != null ? (double)(update.Sets * update.Reps * update.Weight) : 0;
            record.Duration = update.Duration;
            record.OnModified = DateTime.UtcNow;

            context.ExerciseRecords.Update(record);
            context.SaveChanges();
        }

        public void RemoveRecord(ExerciseRecord record)
        {
            context.ExerciseRecords.Remove(record);
            context.SaveChanges();
        }

        public void RemoveAllExerciseRecords(Exercise exercise)
        {
            List<ExerciseRecord> recordsToBeRemoved = context.ExerciseRecords.Where(er => er.ExerciseId == exercise.Id).ToList();

            context.ExerciseRecords.RemoveRange(recordsToBeRemoved);
            context.SaveChanges();
        }

        public void RemoveAllUserRecords(User user)
        {
            List<ExerciseRecord> recordsToBeRemoved = context.ExerciseRecords.Where(er => er.UserId == user.Id).ToList();

            context.ExerciseRecords.RemoveRange(recordsToBeRemoved);
            context.SaveChanges();
        }

        private DateTime GetStartDate(StatisticPeriod period)
        {
            DateTime startDate = DateTime.MinValue;

            switch(period)
            {
                case StatisticPeriod.OneWeek:
                    startDate = DateTime.UtcNow.AddDays(-7);
                    break;
                case StatisticPeriod.OneMonth:
                    startDate = DateTime.UtcNow.AddMonths(-1);
                    break;
                case StatisticPeriod.TwoMonths:
                    startDate = DateTime.UtcNow.AddMonths(-2);
                    break;
                case StatisticPeriod.ThreeMonths:
                    startDate = DateTime.UtcNow.AddMonths(-3);
                    break;
                case StatisticPeriod.SixMonths:
                    startDate = DateTime.UtcNow.AddMonths(-6);
                    break;
                case StatisticPeriod.OneYear:
                    startDate = DateTime.UtcNow.AddYears(-1);
                    break;
                case StatisticPeriod.All:
                    break;
            }

            return startDate.Date;
        }

        private List<StatisticDataPoint> GetStatisticDataPoints(List<ExerciseRecord> records, StatisticMeasurement measurement)
        {
            List<StatisticDataPoint> points = new List<StatisticDataPoint>();

            switch (measurement)
            {
                case StatisticMeasurement.Sets:
                    points = records.Select(r => new StatisticDataPoint { Value = r.Sets, Date = r.OnCreated }).ToList();
                    break;
                case StatisticMeasurement.Reps:
                    points = records.Select(r => new StatisticDataPoint { Value = r.Reps, Date = r.OnCreated }).ToList();
                    break;
                case StatisticMeasurement.Volume:
                    points = records.Select(r => new StatisticDataPoint { Value = r.Volume, Date = r.OnCreated }).ToList();
                    break;
                case StatisticMeasurement.Duration:
                    points = records.Select(r => new StatisticDataPoint { Value = r.Duration, Date = r.OnCreated }).ToList();
                    break;
                case StatisticMeasurement.Weight:
                    points = records.Select(r => new StatisticDataPoint { Value = r.Weight, Date = r.OnCreated }).ToList();
                    break;
            }

            return points;
        }
    }
}
