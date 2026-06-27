using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace Taskish.Controls
{
    public class DateTimePropertyField : PropertyField
    {
        static DateTimePropertyField()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimePropertyField),
                new FrameworkPropertyMetadata(typeof(DateTimePropertyField)));
        }

        private static readonly string[] Formats =
            ["dd.MM.yyyy, HH:mm", "dd.MM.yyyy HH:mm", "dd.MM.yyyy"];

        private string _lastValidText = string.Empty;

        public static readonly DependencyProperty SelectedDateTimeProperty =
            DependencyProperty.Register(nameof(SelectedDateTime), typeof(DateTime?), typeof(DateTimePropertyField),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedDateTimeChanged));

        public DateTime? SelectedDateTime
        {
            get => (DateTime?)GetValue(SelectedDateTimeProperty);
            set => SetValue(SelectedDateTimeProperty, value);
        }

        public static readonly DependencyProperty MinDateProperty =
            DependencyProperty.Register(nameof(MinDate), typeof(DateTime?), typeof(DateTimePropertyField));

        public DateTime? MinDate
        {
            get => (DateTime?)GetValue(MinDateProperty);
            set => SetValue(MinDateProperty, value);
        }

        private static void OnSelectedDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DateTimePropertyField field)
                field.SyncTextFromValue();
        }

        private void SyncTextFromValue()
        {
            var text = SelectedDateTime.HasValue
                ? SelectedDateTime.Value.ToString("dd.MM.yyyy, HH:mm")
                : string.Empty;
            Text = text;
            _lastValidText = text;
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            TryCommit();
        }

        private void TryCommit()
        {
            var trimmed = Text.Trim();

            if (string.IsNullOrEmpty(trimmed))
            {
                SelectedDateTime = null;
                _lastValidText = string.Empty;
                return;
            }

            if (!DateTime.TryParseExact(trimmed, Formats,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            {
                Revert();
                return;
            }

            if (dt.Year < 2000)
            {
                Revert();
                return;
            }

            if (MinDate.HasValue && dt < MinDate.Value)
            {
                Revert();
                return;
            }

            SelectedDateTime = dt;
            _lastValidText = Text;
        }

        private void Revert()
        {
            Text = _lastValidText;
        }
    }
}
