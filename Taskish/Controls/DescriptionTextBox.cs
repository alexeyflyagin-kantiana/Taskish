using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Taskish.Resources;

namespace Taskish.Controls
{
    public class DescriptionTextBox : TextBox
    {
        private IconButton? _copyBtn;

        static DescriptionTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DescriptionTextBox),
                new FrameworkPropertyMetadata(typeof(DescriptionTextBox)));
        }

        public static readonly DependencyProperty IconDataProperty =
            DependencyProperty.Register(nameof(IconData), typeof(Geometry), typeof(DescriptionTextBox));

        public Geometry? IconData
        {
            get => (Geometry?)GetValue(IconDataProperty);
            set => SetValue(IconDataProperty, value);
        }

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(DescriptionTextBox),
                new PropertyMetadata(string.Empty));

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public static readonly DependencyProperty IsBlockedProperty =
            DependencyProperty.Register(nameof(IsBlocked), typeof(bool), typeof(DescriptionTextBox),
                new PropertyMetadata(false));

        public bool IsBlocked
        {
            get => (bool)GetValue(IsBlockedProperty);
            set => SetValue(IsBlockedProperty, value);
        }

        public static readonly DependencyProperty MaxCharactersProperty =
            DependencyProperty.Register(nameof(MaxCharacters), typeof(int), typeof(DescriptionTextBox),
                new PropertyMetadata(1000, OnMaxCharactersChanged));

        public int MaxCharacters
        {
            get => (int)GetValue(MaxCharactersProperty);
            set => SetValue(MaxCharactersProperty, value);
        }

        private static void OnMaxCharactersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DescriptionTextBox box)
                box.MaxLength = (int)e.NewValue;
        }

        public DescriptionTextBox()
        {
            MaxLength = 1000;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _copyBtn = GetTemplateChild("copyBtn") as IconButton;
            if (_copyBtn != null)
            {
                _copyBtn.PreviewMouseDown += (s, e) =>
                {
                    e.Handled = true;
                    Clipboard.SetText(Text);

                    var tooltip = new ToolTip { Content = Strings.Copied };
                    _copyBtn.ToolTip = tooltip;
                    tooltip.IsOpen = true;

                    var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
                    timer.Tick += (_, _) =>
                    {
                        tooltip.IsOpen = false;
                        _copyBtn.ToolTip = null;
                        timer.Stop();
                    };
                    timer.Start();
                };
            }
        }
    }
}