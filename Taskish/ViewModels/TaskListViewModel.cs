using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Taskish.Commands;
using Taskish.Models;
using Taskish.Services;

namespace Taskish.ViewModels
{
    public class TaskListViewModel : BaseViewModel
    {
        private readonly TaskService _taskService;
        private readonly Action<TaskItem> _onTaskSelected;
        private readonly Action _onRefresh;

        private readonly ObservableCollection<TaskItem> _tasks = new();
        public ListCollectionView TasksView { get; }

        public string TaskCounter => $"({_tasks.Count(t => !t.IsCompleted)}/{_tasks.Count})";
        public RelayCommand<TaskItem> TaskSelectedCommand { get; }

        public TaskListViewModel(TaskService taskService, Action<TaskItem> onTaskSelected, Action onRefresh)
        {
            _taskService = taskService;
            _onTaskSelected = onTaskSelected;
            _onRefresh = onRefresh;

            TasksView = new ListCollectionView(_tasks)
            {
                CustomSort = new TaskDeadlineComparer()
            };
            TasksView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(TaskItem.IsCompleted)));
            _tasks.CollectionChanged += (_, _) => OnPropertyChanged(nameof(TaskCounter));

            TaskSelectedCommand = new RelayCommand<TaskItem>(t => _onTaskSelected(t!));

            LoadTasks();
        }

        private void LoadTasks()
        {
            var tasks = _taskService.GetAll();
            foreach (var task in tasks)
                _tasks.Add(task);
        }

        public void RemoveTask(TaskItem task) => _tasks.Remove(task);

        public void UpdateTask(TaskItem task)
        {
            var existing = _tasks.FirstOrDefault(t => t.Id == task.Id);
            if (existing == null) return;
            _tasks[_tasks.IndexOf(existing)] = task;
        }

        public void Refresh()
        {
            _tasks.Clear();
            LoadTasks();
            TasksView.Refresh();
        }

        public void RefreshSort() => TasksView.Refresh();

        private sealed class TaskDeadlineComparer : IComparer
        {
            public int Compare(object? x, object? y)
            {
                var a = (TaskItem)x!;
                var b = (TaskItem)y!;

                if (a.IsCompleted != b.IsCompleted)
                    return a.IsCompleted ? 1 : -1;

                if (a.IsCompleted)
                    return b.CompletedAt.GetValueOrDefault().CompareTo(a.CompletedAt.GetValueOrDefault());

                bool aHas = a.Deadline.HasValue;
                bool bHas = b.Deadline.HasValue;

                if (aHas && bHas) return a.Deadline!.Value.CompareTo(b.Deadline!.Value);
                if (aHas) return -1;
                if (bHas) return 1;

                return b.CreatedAt.CompareTo(a.CreatedAt);
            }
        }
    }
}
