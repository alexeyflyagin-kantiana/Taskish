using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Taskish.Controls
{
    public class Badge : Control
    {
        static Badge()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Badge), new FrameworkPropertyMetadata(typeof(Badge)));
        }

        public static readonly DependencyProperty IconDataProperty =
            DependencyProperty.Register(nameof(IconData), typeof(Geometry), typeof(Badge));

        public Geometry IconData
        {
            get => (Geometry)GetValue(IconDataProperty);
            set => SetValue(IconDataProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(Badge));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty IsCrossedProperty =
            DependencyProperty.Register(nameof(IsCrossed), typeof(bool), typeof(Badge));

        public bool IsCrossed
        {
            get => (bool)GetValue(IsCrossedProperty);
            set => SetValue(IsCrossedProperty, value);
        }
    }
}
