using PassKeeper.ViewModels.Windows;
using System.Windows;
using System.Windows.Input;

namespace PassKeeper.Views.Windows
{
    public partial class LoginWindow : Window
    {
        private LoginWindowViewModel ViewModel { get; } 
        public LoginWindow(LoginWindowViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = ViewModel;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

    }
}
