using Taskish.Commands;
using Taskish.Models;
using Taskish.Services;

namespace Taskish.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly TaskService _taskService;

        public TaskListViewModel TaskListViewModel { get; }
        public RelayCommand CloseDialogCommand { get; }

        private object _currentRightView = null!;
        public object CurrentRightView
        {
            get => _currentRightView;
            set => SetProperty(ref _currentRightView, value);
        }

        private TaskDialogViewModel? _currentDialog;
        public TaskDialogViewModel? CurrentDialog
        {
            get => _currentDialog;
            set => SetProperty(ref _currentDialog, value);
        }

        public MainViewModel(TaskService taskService)
        {
            _taskService = taskService;
            CloseDialogCommand = new RelayCommand(() => CurrentDialog = null, () => CurrentDialog != null);
            TaskListViewModel = new TaskListViewModel(taskService, OnTaskSelected, OnRefresh);
        }

        private void OnRefresh() => TaskListViewModel.Refresh();

        private void OnTaskSelected(TaskItem task)
        {
            CurrentDialog = new TaskDialogViewModel(
                task,
                _taskService,
                () => CurrentDialog = null,
                deleted =>
                {
                    TaskListViewModel.RemoveTask(deleted);
                    CurrentDialog = null;
                },
                OnRefresh);
        }
    }
}
