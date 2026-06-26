using System.Windows;
using System.Windows.Controls;

namespace Taskish.Controls
{
    public class TaskTitleTextBox : TextBox
    {
        static TaskTitleTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TaskTitleTextBox),
                new FrameworkPropertyMetadata(typeof(TaskTitleTextBox)));
        }

        public static readonly DependencyProperty IsBlockedProperty =
            DependencyProperty.Register(nameof(IsBlocked), typeof(bool), typeof(TaskTitleTextBox),
                new PropertyMetadata(false));

        public bool IsBlocked
        {
            get => (bool)GetValue(IsBlockedProperty);
            set => SetValue(IsBlockedProperty, value);
        }

        public static readonly DependencyProperty MaxCharactersProperty =
            DependencyProperty.Register(nameof(MaxCharacters), typeof(int), typeof(TaskTitleTextBox),
                new PropertyMetadata(255, OnMaxCharactersChanged));

        public int MaxCharacters
        {
            get => (int)GetValue(MaxCharactersProperty);
            set => SetValue(MaxCharactersProperty, value);
        }

        private static void OnMaxCharactersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TaskTitleTextBox box)
                box.MaxLength = (int)e.NewValue;
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