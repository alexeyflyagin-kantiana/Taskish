using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Taskish.Views
{
    public partial class PropertyFieldRow : UserControl
    {
        public static readonly DependencyProperty IconDataProperty =
            DependencyProperty.Register(nameof(IconData), typeof(Geometry), typeof(PropertyFieldRow));
        public Geometry? IconData
        {
            get => (Geometry?)GetValue(IconDataProperty);
            set => SetValue(IconDataProperty, value);
        }

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(PropertyFieldRow));
        public string? Label
        {
            get => (string?)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public static readonly DependencyProperty FieldContentProperty =
            DependencyProperty.Register(nameof(FieldContent), typeof(object), typeof(PropertyFieldRow));
        public object? FieldContent
        {
            get => GetValue(FieldContentProperty);
            set => SetValue(FieldContentProperty, value);
        }

        public PropertyFieldRow() => InitializeComponent();
    }
}