using PassKeeper.ViewModels.Windows;
using System.Windows;
using System.Windows.Input;

namespace PassKeeper.Views.Windows
{
    /// <summary>
    /// Lógica de interacción para LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new LoginWindowViewModel();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void MasterKeyInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginWindowViewModel viewModel)
            {
                viewModel.MasterKey = MasterKeyInput.Password;
            }
        }

        private void RepeatKeyInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginWindowViewModel viewModel)
            {
                viewModel.RepeatKey = RepeatKeyInput.Password;
            }
        }
    }
}
