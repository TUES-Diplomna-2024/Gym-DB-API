namespace GymDB.API.Models.Exercise
{
    public class ExerciseStatisticsModel
    {
        public uint TotalSets { get; set; }

        public uint TotalReps { get; set; }

        public double AvgRepsPerSet { get; set; }

        public double AvgTrainingDuration { get; set; }

        public double AvgVolume { get; set; }

        public double AvgWeight { get; set; }

        public double MaxVolume { get; set; }

        public double MaxWeight { get; set; }

        public List<StatisticDataPoint> DataPoints { get; set; }
    }

    public class StatisticDataPoint
    {
        public dynamic Value { get; set; }

        public DateTime Date { get; set; }
    }
}
