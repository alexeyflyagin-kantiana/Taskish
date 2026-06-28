using Taskish.Data.Repositories;
using Taskish.Models;

namespace Taskish.Services
{
    public class StatisticsService
    {
        private readonly ITaskRepository _taskRepository;

        public StatisticsService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public IReadOnlyList<TaskItem> GetCompletedTasksInPeriod(int numWeeks)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            int daysFromMonday = ((int)today.DayOfWeek + 6) % 7;
            var startDate = today.AddDays(-daysFromMonday).AddDays(-(numWeeks - 1) * 7);

            return _taskRepository.GetAll()
                .Where(t => t.IsCompleted && t.CompletedAt.HasValue)
                .Where(t => DateOnly.FromDateTime(t.CompletedAt!.Value) >= startDate)
                .ToList();
        }

        public HeatMapData GetHeatMapData(int numWeeks)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            int daysFromMonday = ((int)today.DayOfWeek + 6) % 7;
            var currentMonday = today.AddDays(-daysFromMonday);
            var startDate = currentMonday.AddDays(-(numWeeks - 1) * 7);
            int totalDays = numWeeks * 7;

            var pointsByDay = _taskRepository.GetAll()
                .Where(t => t.IsCompleted && t.CompletedAt.HasValue)
                .Where(t => DateOnly.FromDateTime(t.CompletedAt!.Value) >= startDate
                         && DateOnly.FromDateTime(t.CompletedAt!.Value) <= today)
                .GroupBy(t => DateOnly.FromDateTime(t.CompletedAt!.Value))
                .ToDictionary(g => g.Key, g => g.Sum(t => t.StoryPoints));

            var days = new List<HeatMapDayData>(totalDays);
            int record = 0;

            for (int i = 0; i < totalDays; i++)
            {
                var date = startDate.AddDays(i);
                var points = pointsByDay.TryGetValue(date, out var sum) ? sum : 0;
                if (points > record) record = points;
                days.Add(new HeatMapDayData(date, points));
            }

            return new HeatMapData(record, days);
        }
    }
}
