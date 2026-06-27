using System.Windows;
using System.Windows.Input;
using Taskish.ViewModels;

namespace Taskish
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.Escape && DataContext is MainViewModel vm)
            {
                if (vm.CloseDialogCommand.CanExecute(null))
                    vm.CloseDialogCommand.Execute(null);
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            if (WindowState == WindowState.Maximized)
                RootGrid.Margin = new Thickness(8);
            else
                RootGrid.Margin = new Thickness(0);
        }
    }
}


