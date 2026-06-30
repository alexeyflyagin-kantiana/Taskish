using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Taskish.Controls;
using Taskish.Models;
using Taskish.Services;

namespace Taskish.ViewModels
{
    public class StatisticsViewModel : BaseViewModel
    {
        private const int NumWeeks = 30;

        private readonly StatisticsService _statisticsService;

        private string _periodLabel = string.Empty;
        public string PeriodLabel
        {
            get => _periodLabel;
            private set => SetProperty(ref _periodLabel, value);
        }

        private IReadOnlyList<(int WeekIndex, string Text)> _monthLabels = [];
        public IReadOnlyList<(int WeekIndex, string Text)> MonthLabels
        {
            get => _monthLabels;
            private set => SetProperty(ref _monthLabels, value);
        }

        private IReadOnlyList<HeatMapCellData?> _heatMapCells = [];
        public IReadOnlyList<HeatMapCellData?> HeatMapCells
        {
            get => _heatMapCells;
            private set => SetProperty(ref _heatMapCells, value);
        }

        private int _dailyMax;
        public int DailyMax
        {
            get => _dailyMax;
            private set => SetProperty(ref _dailyMax, value);
        }

        private int _dailyAverage;
        public int DailyAverage
        {
            get => _dailyAverage;
            private set => SetProperty(ref _dailyAverage, value);
        }

        private string _todayProductivity = "0 / 0";
        public string TodayProductivity
        {
            get => _todayProductivity;
            private set => SetProperty(ref _todayProductivity, value);
        }

        private string _todayPercent = "0%";
        public string TodayPercent
        {
            get => _todayPercent;
            private set => SetProperty(ref _todayPercent, value);
        }

        private Brush _todayBrush = Brushes.Transparent;
        public Brush TodayBrush
        {
            get => _todayBrush;
            private set => SetProperty(ref _todayBrush, value);
        }

        private IReadOnlyList<ScatterPlotPointData> _scatterPoints = [];
        public IReadOnlyList<ScatterPlotPointData> ScatterPoints
        {
            get => _scatterPoints;
            private set => SetProperty(ref _scatterPoints, value);
        }

        private int _onTimeCount;
        public int OnTimeCount
        {
            get => _onTimeCount;
            private set => SetProperty(ref _onTimeCount, value);
        }

        private int _lateCount;
        public int LateCount
        {
            get => _lateCount;
            private set => SetProperty(ref _lateCount, value);
        }

        private int _noDeadlineCount;
        public int NoDeadlineCount
        {
            get => _noDeadlineCount;
            private set => SetProperty(ref _noDeadlineCount, value);
        }

        public StatisticsViewModel(StatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
            Load();
        }

        public void Refresh() => Load();

        private void Load()
        {
            var data = _statisticsService.GetHeatMapData(NumWeeks);

            PeriodLabel = BuildPeriodLabel(NumWeeks);
            MonthLabels = BuildMonthLabels(data.Days.Select(d => d.Date).ToList());

            var today = DateOnly.FromDateTime(DateTime.Today);
            var cells = new List<HeatMapCellData?>(data.Days.Count);
            foreach (var day in data.Days)
            {
                if (day.Date > today)
                {
                    cells.Add(null);
                    continue;
                }

                var points = day.TotalStoryPoints;
                cells.Add(new HeatMapCellData
                {
                    Tooltip = $"{day.Date.ToString("d MMM yyyy", new CultureInfo("ru-RU"))}: {points} SP",
                    OverlayBrush = GetBrush(points, data.DailyRecord)
                });
            }
            HeatMapCells = cells;

            var todayDay = data.Days.FirstOrDefault(d => d.Date == today);
            int todayPoints = todayDay?.TotalStoryPoints ?? 0;
            TodayBrush = GetBrush(todayPoints, data.DailyRecord);

            var pastActiveDays = data.Days.Where(d => d.Date < today && d.TotalStoryPoints > 0).ToList();
            DailyMax = data.DailyRecord;
            DailyAverage = pastActiveDays.Count > 0 ? Math.Max(1, (int)Math.Round(pastActiveDays.Average(d => d.TotalStoryPoints))) : 1;

            TodayProductivity = $"{todayPoints} / {DailyAverage}";
            TodayPercent = DailyAverage > 0
                ? $"{(double)todayPoints / DailyAverage * 100:F0}%"
                : "0%";

            var completed = _statisticsService.GetCompletedTasksInPeriod(NumWeeks);
            ScatterPoints = completed.Select(t =>
            {
                var days = (t.CompletedAt!.Value - t.CreatedAt).TotalDays;
                var category = t.Deadline.HasValue
                    ? (t.CompletedAt.Value <= t.Deadline.Value ? ScatterPointCategory.OnTime : ScatterPointCategory.Late)
                    : ScatterPointCategory.NoDeadline;
                return new ScatterPlotPointData { StoryPoints = t.StoryPoints, DaysToComplete = days, Category = category };
            }).ToList();

            OnTimeCount     = ScatterPoints.Count(p => p.Category == ScatterPointCategory.OnTime);
            LateCount       = ScatterPoints.Count(p => p.Category == ScatterPointCategory.Late);
            NoDeadlineCount = ScatterPoints.Count(p => p.Category == ScatterPointCategory.NoDeadline);
        }

        private static IReadOnlyList<(int WeekIndex, string Text)> BuildMonthLabels(IList<DateOnly> dates)
        {
            var result = new List<(int, string)>();
            string? lastKey = null;
            var ruCulture = new CultureInfo("ru-RU");

            for (int i = 0; i < dates.Count; i++)
            {
                var date = dates[i];
                string key = $"{date.Year}-{date.Month}";
                if (key == lastKey) continue;

                string label = date.ToString("MMM", ruCulture);
                label = char.ToUpper(label[0]) + label[1..];
                result.Add((i / 7, label));
                lastKey = key;
            }

            return result;
        }

        private static string BuildPeriodLabel(int numWeeks) => $"За последние 30 недель";

        private static Brush GetBrush(int count, int record)
        {
            if (count == 0 || record == 0)
                return GetResourceBrush("Contributions0");

            double percent = (double)count / record * 100;

            string key = percent switch
            {
                >= 100 => "Contributions100",
                >= 75  => "Contributions75",
                >= 50  => "Contributions50",
                >= 25  => "Contributions25",
                _      => "ContributionsMin"
            };

            return GetResourceBrush(key);
        }

        private static Brush GetResourceBrush(string key)
            => Application.Current.TryFindResource(key) as Brush ?? Brushes.Transparent;
    }
}


