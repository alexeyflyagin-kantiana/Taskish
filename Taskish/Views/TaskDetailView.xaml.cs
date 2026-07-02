using System.Windows;
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

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is TaskDetailViewModel vm && !vm.IsCompleted && string.IsNullOrEmpty(vm.Title))
                TitleTextBox.Focus();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement btn)
            {
                btn.ContextMenu.PlacementTarget = btn;
                btn.ContextMenu.IsOpen = true;
            }
        }
    }
}
