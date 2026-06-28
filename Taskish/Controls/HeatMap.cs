using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Taskish.Controls
{
    public class HeatMap : Control
    {
        private const int Rows = 7;
        private const string GridPartName = "PART_Grid";

        private Grid? _grid;

        static HeatMap()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HeatMap),
                new FrameworkPropertyMetadata(typeof(HeatMap)));
        }

        public static readonly DependencyProperty CellSizeProperty =
            DependencyProperty.Register(nameof(CellSize), typeof(double), typeof(HeatMap),
                new PropertyMetadata(10.0));

        public double CellSize
        {
            get => (double)GetValue(CellSizeProperty);
            set => SetValue(CellSizeProperty, value);
        }

        public static readonly DependencyProperty CellGapProperty =
            DependencyProperty.Register(nameof(CellGap), typeof(double), typeof(HeatMap),
                new PropertyMetadata(2.0));

        public double CellGap
        {
            get => (double)GetValue(CellGapProperty);
            set => SetValue(CellGapProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _grid = GetTemplateChild(GridPartName) as Grid;
        }

        public void SetData(IReadOnlyList<HeatMapCellData?> cells)
        {
            if (_grid == null) return;

            _grid.Children.Clear();
            _grid.RowDefinitions.Clear();
            _grid.ColumnDefinitions.Clear();

            if (cells.Count == 0) return;

            int cols = (int)System.Math.Ceiling(cells.Count / (double)Rows);
            double gap = CellGap;

            for (int r = 0; r < Rows; r++)
                _grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            for (int c = 0; c < cols; c++)
                _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var baseBrush = TryFindResource("OnSurfaceBrush10") as Brush
                            ?? new SolidColorBrush(Color.FromArgb(26, 33, 33, 33));
            var cornerRadius = TryFindResource("Corner2") is CornerRadius cr ? cr : new CornerRadius(2);
            double cellSize = CellSize;

            for (int i = 0; i < cells.Count; i++)
            {
                int col = i / Rows;
                int row = i % Rows;
                var data = cells[i];

                if (data == null)
                    continue;

                var container = new Grid
                {
                    Width = cellSize,
                    Height = cellSize,
                    Margin = new Thickness(gap / 2)
                };

                container.Children.Add(new Border
                {
                    Background = baseBrush,
                    CornerRadius = cornerRadius
                });

                if (data.OverlayBrush != null)
                {
                    container.Children.Add(new Border
                    {
                        Background = data.OverlayBrush,
                        CornerRadius = cornerRadius
                    });
                }

                if (!string.IsNullOrEmpty(data.Tooltip))
                    ToolTipService.SetToolTip(container, data.Tooltip);

                Grid.SetRow(container, row);
                Grid.SetColumn(container, col);
                _grid.Children.Add(container);
            }
        }
    }
}
