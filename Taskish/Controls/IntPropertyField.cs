using System.Windows;
using System.Windows.Input;

namespace Taskish.Controls
{
    public class IntPropertyField : PropertyField
    {
        static IntPropertyField()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IntPropertyField),
                new FrameworkPropertyMetadata(typeof(IntPropertyField)));
        }

        private string _lastValidText = string.Empty;
        private bool _isUpdatingText;

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(int), typeof(IntPropertyField),
                new FrameworkPropertyMetadata(1,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnValueChanged));

        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register(nameof(MinValue), typeof(int), typeof(IntPropertyField),
                new PropertyMetadata(1));

        public int MinValue
        {
            get => (int)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(nameof(MaxValue), typeof(int), typeof(IntPropertyField),
                new PropertyMetadata(100));

        public int MaxValue
        {
            get => (int)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IntPropertyField field)
                field.SyncTextFromValue();
        }

        private void SyncTextFromValue()
        {
            _isUpdatingText = true;
            Text = Value.ToString();
            _lastValidText = Text;
            _isUpdatingText = false;
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            if (!_isUpdatingText)
                TryCommit();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TryCommit();
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Escape)
            {
                Revert();
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                e.Handled = true;
                return;
            }

            base.OnKeyDown(e);
        }

        private void TryCommit()
        {
            var trimmed = Text.Trim();

            if (!int.TryParse(trimmed, out int v) || v < MinValue || v > MaxValue)
            {
                Revert();
                return;
            }

            Value = v;
            _lastValidText = Text;
        }

        private void Revert()
        {
            _isUpdatingText = true;
            Text = string.IsNullOrEmpty(_lastValidText) ? Value.ToString() : _lastValidText;
            _isUpdatingText = false;
        }
    }
}
