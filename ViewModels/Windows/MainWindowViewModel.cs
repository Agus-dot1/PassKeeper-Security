using CommunityToolkit.Mvvm.ComponentModel;
using PassKeeper.Views.Pages;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace PassKeeper.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<object> _menuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "Contraseñas",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Key16 },
                TargetPageType = typeof(Views.Pages.AddPasswordPage)
            }
        };

        [ObservableProperty]
        private ObservableCollection<object> _footerMenuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "Add",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Add12 },
                TargetPageType = typeof(Views.Pages.AddPasswordPage)
            }
        };

        private object _selectedPage;
        public object SelectedPage
        {
            get => _selectedPage;
            set => SetProperty(ref _selectedPage, value);
        }

        public MainWindowViewModel()
        {
            SelectedPage = new AddPasswordPage();
        }


        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new()
        {
            new MenuItem { Header = "Contraseñas", Tag = "tray_Contraseñas" }
        };
    }
}
