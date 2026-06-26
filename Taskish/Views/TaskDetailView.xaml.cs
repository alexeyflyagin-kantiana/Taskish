using System.Windows.Controls;
using Taskish.ViewModels;

namespace Taskish.Views
{
    public partial class TaskDetailView : UserControl
    {
        public TaskDetailView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is TaskDetailViewModel vm && !vm.IsCompleted && string.IsNullOrEmpty(vm.Title))
                TitleTextBox.Focus();
        }
    }
}
