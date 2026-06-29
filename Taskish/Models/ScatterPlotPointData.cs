namespace Taskish.Models
{
    public enum ScatterPointCategory { OnTime, Late, NoDeadline }

    public class ScatterPlotPointData
    {
        public int StoryPoints { get; init; }
        public double DaysToComplete { get; init; }
        public ScatterPointCategory Category { get; init; }
    }
}
