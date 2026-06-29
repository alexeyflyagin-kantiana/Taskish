using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Taskish.Models;

namespace Taskish.Controls
{
    public class ScatterPlot : FrameworkElement
    {
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(nameof(Points), typeof(IReadOnlyList<ScatterPlotPointData>), typeof(ScatterPlot),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public IReadOnlyList<ScatterPlotPointData>? Points
        {
            get => (IReadOnlyList<ScatterPlotPointData>?)GetValue(PointsProperty);
            set => SetValue(PointsProperty, value);
        }

        private static double NiceMax(double rawMax)
        {
            if (rawMax <= 0) return 4;
            double[] steps = [0.5, 1, 2, 5, 10, 15, 20, 30, 50, 100, 150, 200, 365];
            foreach (var s in steps)
                if (s * 4 > rawMax * 1.1)
                    return s * 4;
            return Math.Ceiling(rawMax * 1.2 / 50) * 50;
        }

        protected override Size MeasureOverride(Size availableSize)
            => new(double.IsInfinity(availableSize.Width) ? 0 : availableSize.Width, 200);

        protected override void OnRender(DrawingContext dc)
        {
            var points = Points;
            if (points == null || points.Count == 0) return;

            double w = ActualWidth;
            double h = ActualHeight;

            const double marginLeft = 52;
            const double marginRight = 12;
            const double marginTop = 10;
            const double marginBottom = 40;

            double plotW = w - marginLeft - marginRight;
            double plotH = h - marginTop - marginBottom;
            if (plotW <= 0 || plotH <= 0) return;

            double maxX = Math.Max(points.Max(p => p.StoryPoints) + 1.0, 2);
            double rawMaxY = points.Max(p => p.DaysToComplete);
            double maxY = NiceMax(rawMaxY);

            double ToCanvasX(double sp) => marginLeft + sp / maxX * plotW;
            double ToCanvasY(double d) => marginTop + plotH * (1.0 - d / maxY);

            var axisBrush    = TryFindResource("OnSurfaceVariantBrush") as Brush ?? Brushes.Gray;
            var gridBrush    = TryFindResource("OnSurfaceBrush10") as Brush ?? Brushes.LightGray;
            var primaryBrush = TryFindResource("PrimaryBrush") as Brush ?? Brushes.Blue;
            var errorBrush   = TryFindResource("ErrorBrush") as Brush ?? Brushes.Red;
            var neutralBrush = TryFindResource("OnSurfaceVariantBrush") as Brush ?? Brushes.Gray;

            var axisPen  = new Pen(axisBrush, 1);  axisPen.Freeze();
            var gridPen  = new Pen(gridBrush, 1);  gridPen.Freeze();
            var trendPen = new Pen(primaryBrush, 1.5) { DashStyle = DashStyles.Dash }; trendPen.Freeze();

            var typeface = new Typeface("Segoe UI");
            double dpi = VisualTreeHelper.GetDpi(this).PixelsPerDip;

            // Y: gridlines + labels
            string yFmt = maxY < 5 ? "F1" : "F0";
            for (int i = 0; i <= 4; i++)
            {
                double val = maxY * i / 4.0;
                double cy = ToCanvasY(val);
                dc.DrawLine(gridPen, new Point(marginLeft, cy), new Point(w - marginRight, cy));

                var ft = new FormattedText(val.ToString(yFmt, CultureInfo.InvariantCulture),
                    CultureInfo.InvariantCulture, FlowDirection.LeftToRight, typeface, 10, axisBrush, dpi);
                dc.DrawText(ft, new Point(marginLeft - ft.Width - 4, cy - ft.Height / 2));
            }

            // X: evenly spaced ticks (max 7)
            int xSteps = 6;
            double xStep = maxX / xSteps;
            for (int i = 0; i <= xSteps; i++)
            {
                double val = Math.Round(xStep * i);
                if (val == 0) continue;
                double cx = ToCanvasX(val);
                var ft = new FormattedText(((int)val).ToString(), CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight, typeface, 10, axisBrush, dpi);
                dc.DrawLine(gridPen, new Point(cx, marginTop + plotH), new Point(cx, marginTop + plotH + 3));
                dc.DrawText(ft, new Point(cx - ft.Width / 2, marginTop + plotH + 5));
            }

            // Axes
            dc.DrawLine(axisPen, new Point(marginLeft, marginTop), new Point(marginLeft, marginTop + plotH));
            dc.DrawLine(axisPen, new Point(marginLeft, marginTop + plotH), new Point(w - marginRight, marginTop + plotH));

            // X axis label
            var xLabel = new FormattedText("Сложность (SP)", CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight, typeface, 10, axisBrush, dpi);
            dc.DrawText(xLabel, new Point(
                marginLeft + plotW / 2 - xLabel.Width / 2,
                marginTop + plotH + 20));

            // Y axis label (rotated)
            var yLabel = new FormattedText("Дней", CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight, typeface, 10, axisBrush, dpi);
            dc.PushTransform(new RotateTransform(-90, 10, marginTop + plotH / 2));
            dc.DrawText(yLabel, new Point(10 - yLabel.Width / 2, marginTop + plotH / 2 - yLabel.Height / 2));
            dc.Pop();

            // Trend line (linear regression)
            if (points.Count >= 2)
            {
                double n   = points.Count;
                double sx  = points.Sum(p => (double)p.StoryPoints);
                double sy  = points.Sum(p => p.DaysToComplete);
                double sxy = points.Sum(p => p.StoryPoints * p.DaysToComplete);
                double sx2 = points.Sum(p => (double)p.StoryPoints * p.StoryPoints);
                double den = n * sx2 - sx * sx;

                if (Math.Abs(den) > 1e-9)
                {
                    double a = (n * sxy - sx * sy) / den;
                    double b = (sy - a * sx) / n;

                    double y0 = Math.Clamp(b, 0, maxY);
                    double y1 = Math.Clamp(a * maxX + b, 0, maxY);

                    dc.DrawLine(trendPen,
                        new Point(ToCanvasX(0), ToCanvasY(y0)),
                        new Point(ToCanvasX(maxX), ToCanvasY(y1)));
                }
            }

            // Points
            const double r = 4;
            foreach (var p in points)
            {
                var brush = p.Category switch
                {
                    ScatterPointCategory.OnTime => primaryBrush,
                    ScatterPointCategory.Late   => errorBrush,
                    _                           => neutralBrush
                };
                dc.DrawEllipse(brush, null,
                    new Point(ToCanvasX(p.StoryPoints), ToCanvasY(p.DaysToComplete)), r, r);
            }
        }
    }
}
