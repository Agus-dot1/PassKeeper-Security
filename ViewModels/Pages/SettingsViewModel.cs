using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassKeeper.ViewModels.Windows;
using PassKeeper.Views.Windows;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace PassKeeper.ViewModels.Pages
{
    public partial class SettingsViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private string _appVersion = String.Empty;

        [ObservableProperty]
        private ApplicationTheme _currentTheme = ApplicationTheme.Unknown;

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom() { }

        private void InitializeViewModel()
        {
            CurrentTheme = ApplicationThemeManager.GetAppTheme();
            AppVersion = $"PassKeeper - {GetAssemblyVersion()}";

            _isInitialized = true;
        }

        private string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                ?? String.Empty;
        }

        [RelayCommand]
        private void OnChangeTheme(string parameter)
        {
            ApplicationTheme newTheme;

            switch (parameter)
            {
                case "theme_light":
                    newTheme = ApplicationTheme.Light;
                    break;
                case "theme_dark":
                    newTheme = ApplicationTheme.Dark;
                    break;
                default:
                    return; 
            }
            ApplicationThemeManager.Apply(newTheme);
            CurrentTheme = newTheme; 

            LoginWindowViewModel loginWindowViewModel = new LoginWindowViewModel();
            loginWindowViewModel.currentUser.CurrentTheme = newTheme;
            loginWindowViewModel.currentUser.SaveToFile();
        }


    }
}
