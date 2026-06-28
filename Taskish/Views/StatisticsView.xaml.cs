using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Taskish.ViewModels;

namespace Taskish.Views
{
    public partial class StatisticsView : UserControl
    {
        private const double CellStep = 12.0;

        public StatisticsView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is not StatisticsViewModel vm) return;

            Render(vm);
            vm.PropertyChanged += OnVmPropertyChanged;
        }

        private void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StatisticsViewModel.HeatMapCells) &&
                DataContext is StatisticsViewModel vm)
                Render(vm);
        }

        private void Render(StatisticsViewModel vm)
        {
            myHeatMap.SetData(vm.HeatMapCells);

            MonthLabelsCanvas.Children.Clear();
            BuildMonthLabels(vm);
        }

        private void BuildMonthLabels(StatisticsViewModel vm)
        {
            var labelStyle = TryFindResource("LabelSmall") as Style;
            var foreground = TryFindResource("OnSurfaceVariantBrush") as Brush;

            foreach (var (weekIndex, text) in vm.MonthLabels)
            {
                var tb = new TextBlock
                {
                    Text = text,
                    Style = labelStyle,
                    Foreground = foreground,
                    VerticalAlignment = VerticalAlignment.Bottom
                };
                Canvas.SetLeft(tb, weekIndex * CellStep);
                MonthLabelsCanvas.Children.Add(tb);
            }
        }
    }
}
