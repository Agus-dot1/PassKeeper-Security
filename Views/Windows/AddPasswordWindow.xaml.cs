using PassKeeper.ViewModels;
using System.Windows;
using AddPasswordViewModel = PassKeeper.ViewModels.Windows.AddPasswordViewModel;

namespace PassKeeper.Views.Windows
{
    public partial class AddPasswordWindow : Window
    {
        public AddPasswordWindow()
        {
            InitializeComponent();
            DataContext = new AddPasswordViewModel();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
                if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                    DragMove();
        }
    }
}
