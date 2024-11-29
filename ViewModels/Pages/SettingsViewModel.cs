using CommunityToolkit.Mvvm.ComponentModel;
using Wpf.Ui.Controls;

namespace PassKeeper.ViewModels.Pages
{
    public partial class SettingsViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized;

        [ObservableProperty]
        private string _appVersion = string.Empty;

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom() { }

        private void InitializeViewModel()
        {
            AppVersion = $"PassKeeper v1.0.0";
            _isInitialized = true;
        }
    }
}
