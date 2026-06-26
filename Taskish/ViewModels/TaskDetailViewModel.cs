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

        public string Title => _task.Title;
        public string Description => _task.Description;
        public int StoryPoints => _task.StoryPoints;
        public bool IsCompleted => _task.IsCompleted;
        public DateTime CreatedAt => _task.CreatedAt;
        public DateTime? CompletedAt => _task.CompletedAt;
        public DateTime? Deadline => _task.Deadline;

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
            _onDelete(_task);
            _onClose();
        }
    }
}
