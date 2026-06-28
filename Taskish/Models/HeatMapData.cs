namespace Taskish.Models
{
    public class HeatMapData
    {
        public int DailyRecord { get; init; }
        public IReadOnlyList<HeatMapDayData> Days { get; init; }

        public HeatMapData(int dailyRecord, IReadOnlyList<HeatMapDayData> days)
        {
            DailyRecord = dailyRecord;
            Days = days;
        }
    }
}
