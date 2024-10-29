using CommunityToolkit.Mvvm.ComponentModel;
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

        [ObservableProperty]
        private ObservableCollection<Wpf.Ui.Controls.MenuItem> _trayMenuItems = new()
        {
            new Wpf.Ui.Controls.MenuItem { Header = "Contraseñas", Tag = "tray_Contraseñas" }
        };
    }
}
