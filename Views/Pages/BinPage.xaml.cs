using PassKeeper.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace PassKeeper.Views.Pages;

public partial class BinPage : INavigableView<BinViewModel> 
{
    public BinViewModel ViewModel { get; }
    public BinPage(BinViewModel viewModel) {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = ViewModel;
    }
}
