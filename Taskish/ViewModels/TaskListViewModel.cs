using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Taskish.Commands;
using Taskish.Controls;
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
        public bool IsEmpty => _tasks.Count == 0 && !_isSearchActive;

        private bool _isSearchActive;
        public bool IsSearchActive
        {
            get => _isSearchActive;
            private set { _isSearchActive = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsEmpty)); }
        }

        private string _searchQuery = string.Empty;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
                TasksView.Refresh();
                OnPropertyChanged(nameof(IsSearchNoResults));
            }
        }

        public bool IsSearchNoResults =>
            _isSearchActive && !string.IsNullOrWhiteSpace(_searchQuery) && TasksView.Count == 0;

        public RelayCommand SearchCommand { get; }
        public RelayCommand CloseSearchCommand { get; }
        public RelayCommand AddTaskCommand { get; }
        public RelayCommand<TaskItem> ToggleCompleteCommand { get; }
        public RelayCommand<TaskItem> TaskSelectedCommand { get; }

        public TaskListViewModel(TaskService taskService, Action<TaskItem> onTaskSelected, Action onRefresh)
        {
            _taskService = taskService;
            _onTaskSelected = onTaskSelected;
            _onRefresh = onRefresh;

            TasksView = new ListCollectionView(_tasks)
            {
                CustomSort = new TaskDeadlineComparer(),
                Filter = FilterTask
            };
            TasksView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(TaskItem.IsCompleted)));
            _tasks.CollectionChanged += (_, _) => { OnPropertyChanged(nameof(TaskCounter)); OnPropertyChanged(nameof(IsEmpty)); OnPropertyChanged(nameof(IsSearchNoResults)); };

            SearchCommand = new RelayCommand(() => { IsSearchActive = true; });
            CloseSearchCommand = new RelayCommand(() =>
            {
                IsSearchActive = false;
                SearchQuery = string.Empty;
            });
            AddTaskCommand = new RelayCommand(AddTask);
            ToggleCompleteCommand = new RelayCommand<TaskItem>(ToggleComplete);
            TaskSelectedCommand = new RelayCommand<TaskItem>(t => _onTaskSelected(t!));

            TaskCard.MinuteTick += (_, _) => TasksView.Refresh();

            LoadTasks();
        }

        private bool FilterTask(object obj)
        {
            if (string.IsNullOrWhiteSpace(_searchQuery)) return true;
            if (obj is not TaskItem task) return false;
            var q = _searchQuery.Trim();
            return (task.Title?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false)
                || (task.Description?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        private void ToggleComplete(TaskItem task)
        {
            if (task.IsCompleted)
                _taskService.Uncomplete(task.Id);
            else
                _taskService.Complete(task.Id);

            TasksView.Refresh();
            OnPropertyChanged(nameof(TaskCounter));
            _onRefresh();
        }

        private void LoadTasks()
        {
            var tasks = _taskService.GetAll();
            foreach (var task in tasks)
                _tasks.Add(task);
        }

        private void AddTask()
        {
            var task = new TaskItem { Title = string.Empty };
            _taskService.Add(task);
            _tasks.Add(task);
            _onTaskSelected(task);
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


