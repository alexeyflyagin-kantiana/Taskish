namespace Taskish.Models
{
    public class HeatMapDayData
    {
        public DateOnly Date { get; init; }
        public int TotalStoryPoints { get; init; }

        public HeatMapDayData(DateOnly date, int totalStoryPoints)
        {
            Date = date;
            TotalStoryPoints = totalStoryPoints;
        }
    }
}
