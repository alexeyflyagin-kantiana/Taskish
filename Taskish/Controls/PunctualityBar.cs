using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Taskish.Controls
{
    public class PunctualityBar : FrameworkElement
    {
        public static readonly DependencyProperty OnTimeCountProperty =
            DependencyProperty.Register(nameof(OnTimeCount), typeof(int), typeof(PunctualityBar),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty LateCountProperty =
            DependencyProperty.Register(nameof(LateCount), typeof(int), typeof(PunctualityBar),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty NoDeadlineCountProperty =
            DependencyProperty.Register(nameof(NoDeadlineCount), typeof(int), typeof(PunctualityBar),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));

        public int OnTimeCount { get => (int)GetValue(OnTimeCountProperty); set => SetValue(OnTimeCountProperty, value); }
        public int LateCount { get => (int)GetValue(LateCountProperty); set => SetValue(LateCountProperty, value); }
        public int NoDeadlineCount { get => (int)GetValue(NoDeadlineCountProperty); set => SetValue(NoDeadlineCountProperty, value); }

        protected override Size MeasureOverride(Size availableSize)
            => new(double.IsInfinity(availableSize.Width) ? 0 : availableSize.Width, 28);

        protected override void OnRender(DrawingContext dc)
        {
            int total = OnTimeCount + LateCount + NoDeadlineCount;
            if (total == 0) return;

            double w = ActualWidth;
            double h = ActualHeight;
            double radius = 6;

            var primaryBrush = TryFindResource("PrimaryBrush") as Brush ?? Brushes.Green;
            var errorBrush   = TryFindResource("ErrorBrush")   as Brush ?? Brushes.Red;
            var neutralBrush = TryFindResource("OnSurfaceVariantBrush") as Brush ?? Brushes.Gray;
            var onPrimaryBrush = TryFindResource("OnPrimaryBrush") as Brush ?? Brushes.White;

            var typeface = new Typeface("Segoe UI");
            double dpi = VisualTreeHelper.GetDpi(this).PixelsPerDip;

            double[] ratios = [
                (double)OnTimeCount     / total,
                (double)LateCount       / total,
                (double)NoDeadlineCount / total
            ];
            Brush[] brushes = [primaryBrush, errorBrush, neutralBrush];
            int[]   counts  = [OnTimeCount, LateCount, NoDeadlineCount];

            double x = 0;
            for (int i = 0; i < 3; i++)
            {
                if (counts[i] == 0) continue;

                double segW = ratios[i] * w;

                // Rounded corners только у первого и последнего сегментов
                bool isFirst = (x == 0);
                bool isLast  = (i == 2 || (i < 2 && counts[(i + 1)..].Sum() == 0));

                double rLeft  = isFirst ? radius : 0;
                double rRight = isLast  ? radius : 0;

                var geo = BuildSegment(x, 0, segW, h, rLeft, rRight);
                dc.DrawGeometry(brushes[i], null, geo);

                // Метка внутри сегмента
                string label = $"{counts[i]} ({ratios[i] * 100:F0}%)";
                var ft = new FormattedText(label, CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight, typeface, 10, onPrimaryBrush, dpi);

                if (ft.Width + 8 <= segW)
                    dc.DrawText(ft, new Point(x + segW / 2 - ft.Width / 2, h / 2 - ft.Height / 2));

                x += segW;
            }
        }

        private static Geometry BuildSegment(double x, double y, double w, double h, double rLeft, double rRight)
        {
            var geo = new StreamGeometry();
            using var ctx = geo.Open();

            ctx.BeginFigure(new Point(x + rLeft, y), true, true);
            ctx.LineTo(new Point(x + w - rRight, y), true, false);
            if (rRight > 0) ctx.ArcTo(new Point(x + w, y + rRight), new Size(rRight, rRight), 0, false, SweepDirection.Clockwise, true, false);
            ctx.LineTo(new Point(x + w, y + h - rRight), true, false);
            if (rRight > 0) ctx.ArcTo(new Point(x + w - rRight, y + h), new Size(rRight, rRight), 0, false, SweepDirection.Clockwise, true, false);
            ctx.LineTo(new Point(x + rLeft, y + h), true, false);
            if (rLeft > 0) ctx.ArcTo(new Point(x, y + h - rLeft), new Size(rLeft, rLeft), 0, false, SweepDirection.Clockwise, true, false);
            ctx.LineTo(new Point(x, y + rLeft), true, false);
            if (rLeft > 0) ctx.ArcTo(new Point(x + rLeft, y), new Size(rLeft, rLeft), 0, false, SweepDirection.Clockwise, true, false);

            geo.Freeze();
            return geo;
        }
    }
}
