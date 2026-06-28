using System;
using Taskish.Commands;
using Taskish.Models;
using Taskish.Services;

namespace Taskish.ViewModels
{
    public class TaskDetailViewModel : BaseViewModel
    {
        private readonly TaskService _taskService;
        private readonly Action _onClose;
        private readonly Action<TaskItem> _onDelete;
        private readonly Action _onRefresh;

        private TaskItem _task;

        public string Title
        {
            get => _task.Title;
            set
            {
                if (_task.Title == value) return;
                _task.Title = value;
                _taskService.Update(_task);
                _onRefresh();
            }
        }

        public string Description
        {
            get => _task.Description;
            set
            {
                if (_task.Description == value) return;
                _task.Description = value;
                _taskService.Update(_task);
                _onRefresh();
            }
        }
        public int StoryPoints
        {
            get => _task.StoryPoints;
            set
            {
                if (_task.StoryPoints == value) return;
                _task.StoryPoints = value;
                _taskService.Update(_task);
                _onRefresh();
            }
        }
        public bool IsCompleted => _task.IsCompleted;
        public DateTime CreatedAt => _task.CreatedAt;
        public DateTime? CompletedAt => _task.CompletedAt;
        public DateTime? Deadline
        {
            get => _task.Deadline;
            set
            {
                if (_task.Deadline == value) return;
                _task.Deadline = value;
                _taskService.Update(_task);
                _onRefresh();
            }
        }

        public RelayCommand CompleteCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand CloseCommand { get; }

        public TaskDetailViewModel(TaskItem task, TaskService taskService, Action onClose, Action<TaskItem> onDelete, Action onRefresh)
        {
            _task = task;
            _taskService = taskService;
            _onClose = onClose;
            _onDelete = onDelete;
            _onRefresh = onRefresh;

            CloseCommand = new RelayCommand(_onClose);
            CompleteCommand = new RelayCommand(Complete);
            DeleteCommand = new RelayCommand(Delete);
        }

        private void Complete()
        {
            if (!_task.IsCompleted)
                _taskService.Complete(_task.Id);
            else
                _taskService.Uncomplete(_task.Id);
            OnPropertyChanged(nameof(IsCompleted));
            OnPropertyChanged(nameof(CompletedAt));
            _onRefresh();
        }

        private void Delete()
        {
            _taskService.Delete(_task.Id);
            _onRefresh();
            _onDelete(_task);
            _onClose();
        }
    }
}


