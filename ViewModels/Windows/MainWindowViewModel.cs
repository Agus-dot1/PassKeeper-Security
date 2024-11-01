﻿using CommunityToolkit.Mvvm.ComponentModel;
using PassKeeper.Views.Pages;
using System.Collections.ObjectModel;
using System.Windows;
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
                TargetPageType = typeof(PasswordsPage)
            },
            new NavigationViewItem()
            {
                Content = "Colecciones",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Folder16 },
                TargetPageType = typeof(CollectionPage)
            }
        };

        [ObservableProperty]
        private ObservableCollection<object> _footerMenuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "Configuracion",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(SettingsPage)
            }
        };

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new()
        {
            new MenuItem { Header = "Contraseñas", Tag = "tray_Contraseñas" }
        };
    }
}
