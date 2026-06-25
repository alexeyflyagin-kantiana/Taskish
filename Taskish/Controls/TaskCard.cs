using System;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;

namespace Taskish.Controls
{
    public class TaskCard : System.Windows.Controls.ListBoxItem
    {
        private static readonly DispatcherTimer _refreshTimer;
        private static event EventHandler? RefreshTick;
        public static event EventHandler? MinuteTick;

        static TaskCard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TaskCard), new FrameworkPropertyMetadata(typeof(TaskCard)));

            _refreshTimer = new DispatcherTimer(DispatcherPriority.Background)
            {
                Interval = TimeSpan.FromMinutes(1)
            };
            _refreshTimer.Tick += (_, _) =>
            {
                RefreshTick?.Invoke(null, EventArgs.Empty);
                MinuteTick?.Invoke(null, EventArgs.Empty);
            };
            _refreshTimer.Start();
        }

        public TaskCard()
        {
            Loaded += (_, _) => RefreshTick += OnRefreshTick;
            Unloaded += (_, _) => RefreshTick -= OnRefreshTick;
        }

        private void OnRefreshTick(object? sender, EventArgs e) => UpdateDeadlineState();

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(TaskCard));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(nameof(Description), typeof(string), typeof(TaskCard),
                new PropertyMetadata(string.Empty, OnDescriptionChanged));

        private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TaskCard card && e.NewValue is string text)
                card.SetValue(DescriptionProperty, text.Replace("\n", " ").Replace("\r", ""));
        }

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public static readonly DependencyProperty StoryPointsProperty =
            DependencyProperty.Register(nameof(StoryPoints), typeof(int), typeof(TaskCard));

        public int StoryPoints
        {
            get => (int)GetValue(StoryPointsProperty);
            set => SetValue(StoryPointsProperty, value);
        }

        public static readonly DependencyProperty IsCompletedProperty =
            DependencyProperty.Register(nameof(IsCompleted), typeof(bool), typeof(TaskCard),
                new PropertyMetadata(false, OnDeadlineInputChanged));

        public bool IsCompleted
        {
            get => (bool)GetValue(IsCompletedProperty);
            set => SetValue(IsCompletedProperty, value);
        }

        public static readonly DependencyProperty DeadlineProperty =
            DependencyProperty.Register(nameof(Deadline), typeof(DateTime?), typeof(TaskCard),
                new PropertyMetadata(null, OnDeadlineInputChanged));

        public DateTime? Deadline
        {
            get => (DateTime?)GetValue(DeadlineProperty);
            set => SetValue(DeadlineProperty, value);
        }

        public static readonly DependencyProperty CompletedAtProperty =
            DependencyProperty.Register(nameof(CompletedAt), typeof(DateTime?), typeof(TaskCard),
                new PropertyMetadata(null, OnDeadlineInputChanged));

        public DateTime? CompletedAt
        {
            get => (DateTime?)GetValue(CompletedAtProperty);
            set => SetValue(CompletedAtProperty, value);
        }

        public static readonly DependencyProperty DeadlineStateProperty =
            DependencyProperty.Register(nameof(DeadlineState), typeof(DeadlineState), typeof(TaskCard),
                new PropertyMetadata(DeadlineState.Hidden));

        public DeadlineState DeadlineState
        {
            get => (DeadlineState)GetValue(DeadlineStateProperty);
            private set => SetValue(DeadlineStateProperty, value);
        }

        public static readonly DependencyProperty DeadlineTextProperty =
            DependencyProperty.Register(nameof(DeadlineText), typeof(string), typeof(TaskCard),
                new PropertyMetadata(string.Empty));

        public string DeadlineText
        {
            get => (string)GetValue(DeadlineTextProperty);
            private set => SetValue(DeadlineTextProperty, value);
        }

        private static void OnDeadlineInputChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TaskCard card) card.UpdateDeadlineState();
        }

        private void UpdateDeadlineState()
        {
            if (Deadline == null)
            {
                DeadlineState = DeadlineState.Hidden;
                DeadlineText = string.Empty;
                return;
            }

            DeadlineText = Deadline.Value.ToString("dd.MM.yyyy");

            bool isOverdue = IsCompleted
                ? CompletedAt.HasValue && CompletedAt.Value > Deadline.Value
                : Deadline.Value < DateTime.Now;

            DeadlineState = (IsCompleted, isOverdue) switch
            {
                (true, false)  => DeadlineState.DoneOnTime,
                (_, true)      => DeadlineState.Overdue,
                _              => DeadlineState.Normal
            };
        }
    }
}


