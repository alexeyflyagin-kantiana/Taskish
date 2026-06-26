using System;
using Taskish.Models;
using Taskish.Services;

namespace Taskish.ViewModels
{
    public class TaskDialogViewModel : BaseViewModel
    {
        private readonly TaskItem _task;
        private readonly TaskService _taskService;
        private readonly Action _onClose;
        private readonly Action<TaskItem> _onDelete;

        private object _currentContent = null!;
        public object CurrentContent
        {
            get => _currentContent;
            private set => SetProperty(ref _currentContent, value);
        }

        private readonly Action _onRefresh;

        public TaskDialogViewModel(TaskItem task, TaskService taskService, Action onClose, Action<TaskItem> onDelete, Action onRefresh)
        {
            _task = task;
            _taskService = taskService;
            _onClose = onClose;
            _onDelete = onDelete;
            _onRefresh = onRefresh;

            CurrentContent = new TaskDetailViewModel(_task, _taskService, _onClose, _onDelete, _onRefresh);
        }
    }
}
