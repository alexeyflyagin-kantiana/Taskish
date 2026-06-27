using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Taskish.Models;

namespace Taskish.Controls
{
    public class TaskListBox : ListBox
    {
        public static readonly DependencyProperty TaskSelectedCommandProperty =
            DependencyProperty.Register(nameof(TaskSelectedCommand), typeof(ICommand), typeof(TaskListBox));

        public ICommand? TaskSelectedCommand
        {
            get => (ICommand?)GetValue(TaskSelectedCommandProperty);
            set => SetValue(TaskSelectedCommandProperty, value);
        }

        protected override DependencyObject GetContainerForItemOverride()
            => new TaskCard();

        protected override bool IsItemItsOwnContainerOverride(object item)
            => item is TaskCard;

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            if (element is TaskCard card && item is TaskItem task)
            {
                card.Title = task.Title;
                card.Description = task.Description;
                card.StoryPoints = task.StoryPoints;
                card.IsCompleted = task.IsCompleted;
                card.Deadline = task.Deadline;
                card.CompletedAt = task.CompletedAt;
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (e.AddedItems.Count > 0 && e.AddedItems[0] is TaskItem task)
            {
                SelectedItem = null;
                if (Mouse.LeftButton == MouseButtonState.Pressed &&
                    TaskSelectedCommand?.CanExecute(task) == true)
                    TaskSelectedCommand.Execute(task);
            }
        }
    }
}


