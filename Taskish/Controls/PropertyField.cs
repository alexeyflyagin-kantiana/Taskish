using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Taskish.Controls
{
    public class PropertyField : TextBox
    {
        static PropertyField()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyField), new FrameworkPropertyMetadata(typeof(PropertyField)));
        }

        public static readonly DependencyProperty ErrorIconDataProperty =
            DependencyProperty.Register(nameof(ErrorIconData), typeof(Geometry), typeof(PropertyField));

        public Geometry ErrorIconData
        {
            get => (Geometry)GetValue(ErrorIconDataProperty);
            set => SetValue(ErrorIconDataProperty, value);
        }

        public static readonly DependencyProperty ErrorTextProperty =
            DependencyProperty.Register(nameof(ErrorText), typeof(string), typeof(PropertyField));

        public string ErrorText
        {
            get => (string)GetValue(ErrorTextProperty);
            set => SetValue(ErrorTextProperty, value);
        }

        public static readonly DependencyProperty IsErrorProperty =
            DependencyProperty.Register(nameof(IsError), typeof(bool), typeof(PropertyField));

        public bool IsError
        {
            get => (bool)GetValue(IsErrorProperty);
            set => SetValue(IsErrorProperty, value);
        }

        public static readonly DependencyProperty IsBlockedProperty =
            DependencyProperty.Register(nameof(IsBlocked), typeof(bool), typeof(PropertyField),
                new PropertyMetadata(false, OnIsBlockedChanged));

        public bool IsBlocked
        {
            get => (bool)GetValue(IsBlockedProperty);
            set => SetValue(IsBlockedProperty, value);
        }

        private static void OnIsBlockedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PropertyField field)
                field.IsEnabled = !(bool)e.NewValue;
        }

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                GetBindingExpression(TextProperty)?.UpdateSource();
                MoveFocus(new System.Windows.Input.TraversalRequest(System.Windows.Input.FocusNavigationDirection.Next));
                e.Handled = true;
            }
        }
    }
}

