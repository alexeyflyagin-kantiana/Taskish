using System.IO;
using System.Windows;
using Taskish.Data.Repositories;
using Taskish.Services;
using Taskish.ViewModels;

namespace Taskish
{
    public partial class App : Application
    {
        JsonStorage _storage = null!;
        TaskRepository _taskRepository = null!;
        TaskService _taskService = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            var appFolder = AppDomain.CurrentDomain.BaseDirectory;
            var tasksFilePath = System.IO.Path.Combine(appFolder, "tasks.json");

            _storage = new JsonStorage(tasksFilePath);
            _taskRepository = new TaskRepository(_storage);
            _taskService = new TaskService(_taskRepository);

            var mainViewModel = new MainViewModel(_taskService);

            var window = new MainWindow();
            window.DataContext = mainViewModel;
            window.Show();

            base.OnStartup(e);
        }
    }
}
