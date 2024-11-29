using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using PassKeeper.Views.Pages;
using Wpf.Ui.Controls;

namespace PassKeeper.ViewModels.Windows;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<object> _menuItems = new()
    {
        new NavigationViewItem
        {
            Content = "Passwords",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Key16 },
            TargetPageType = typeof(PasswordsPage)
        }
    };

    [ObservableProperty] private ObservableCollection<object> _footerMenuItems = new()
    {
        new NavigationViewItem
        {
            Content = "Settings",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
            TargetPageType = typeof(SettingsPage)
        }
    };

    [ObservableProperty] private ObservableCollection<MenuItem> _trayMenuItems = new()
    {
        new MenuItem { Header = "PasswordsPage", Tag = "tray_PasswordsPage" }
    };
}