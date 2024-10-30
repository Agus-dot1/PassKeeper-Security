using PassKeeper.ViewModels;
using System.Windows;

namespace PassKeeper.Views.Windows
{
    /// <summary>
    /// Lógica de interacción para AddPassword.xaml
    /// </summary>
    public partial class AddPasswordWindow : Window
    {
        public AddPasswordWindow()
        {
            InitializeComponent();
            DataContext = new AddPasswordViewModel();
        }
    }
}
