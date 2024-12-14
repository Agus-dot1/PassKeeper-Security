using PassKeeper.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace PassKeeper.Views.Pages; 
public partial class PasswordsPage : INavigableView<PasswordViewModel>
{
    public PasswordViewModel ViewModel { get; }

    public PasswordsPage(PasswordViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = ViewModel;
        InitializeComponent();
    }
}
