using System.Windows;
using System.Windows.Media;

namespace Taskish.Controls
{
    public class DoneButton : Button
    {
        static DoneButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DoneButton), new FrameworkPropertyMetadata(typeof(DoneButton)));
        }

        public static readonly DependencyProperty IsCompletedProperty =
            DependencyProperty.Register(nameof(IsCompleted), typeof(bool), typeof(DoneButton));

        public bool IsCompleted
        {
            get => (bool)GetValue(IsCompletedProperty);
            set => SetValue(IsCompletedProperty, value);
        }
    }
}
