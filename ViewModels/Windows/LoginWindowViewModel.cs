using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassKeeper.Models;
using PassKeeper.Views.Windows;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Controls;
using MessageBox = Wpf.Ui.Controls.MessageBox;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;
using TextBlock = Wpf.Ui.Controls.TextBlock;

namespace PassKeeper.ViewModels.Windows
{
    public partial class LoginWindowViewModel : ObservableObject
    {
        public UserModel currentUser;
        [ObservableProperty] private string masterKey;
        [ObservableProperty] private string repeatMKey;
        [ObservableProperty] public bool isNewUser;
        [ObservableProperty] private string? createButtonContent;
        [ObservableProperty] private string? errorMessage;
        [ObservableProperty] private string? successMessage;
        [ObservableProperty] private bool errorMessageVisibility;
        [ObservableProperty] private bool successMessageVisibility;

        public LoginWindowViewModel()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "databases", "database.pks");
            currentUser = UserModel.LoadFromFile(dbPath) ?? new UserModel(new MasterKeyModel());
            IsNewUser = string.IsNullOrEmpty(currentUser.MasterKey.HashedKey);
            CreateButtonContent = IsNewUser ? "Create" : "Open Vault";
        }


        [RelayCommand]
        private async Task TriggerCreate()
        {
            var messageBox = new MessageBox
            {
                Title = "Are you sure you want to create a new master key?",
                Content = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children =
                    {
                        new SymbolIcon { Symbol = SymbolRegular.Warning12, Foreground = new SolidColorBrush(Colors.OrangeRed), FontSize = 24, Width = 20, Height = 28, Margin = new Thickness(0, 0, 10, 0) },
                        new TextBlock { Text = "Current data will be deleted.", VerticalAlignment = VerticalAlignment.Center }
                    }
                },
                Background = new SolidColorBrush(Color.FromArgb(255, 16, 23, 41)),
                PrimaryButtonText = "Accept",
                CloseButtonText = "Cancel",
                MinWidth = 300,
                MinHeight = 100,
            };
            var result = await messageBox.ShowDialogAsync();

            if (result == MessageBoxResult.Primary)
            {
                string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "databases", "database.pks");

                if (File.Exists(dbPath))
                {
                    try
                    {
                        await Task.Run(() => File.Delete(dbPath));
                        SuccessMessage = "Previous data deleted successfully.";
                        SuccessMessageVisibility = true;
                        ErrorMessageVisibility = false;
                        MasterKey = RepeatMKey = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = $"Error deleting previous data: {ex.Message}";
                        ErrorMessageVisibility = true;
                        SuccessMessageVisibility = false;
                        return;
                    }
                }

                currentUser = new UserModel(new MasterKeyModel());
                IsNewUser = true;
                CreateButtonContent = "Create";
                MasterKey = RepeatMKey = string.Empty;
            }
        }

        [RelayCommand]
        private void CreateOrLogin()
        {
            if (IsNewUser)
            {
                if (string.IsNullOrEmpty(MasterKey) || string.IsNullOrEmpty(RepeatMKey))
                {
                    ErrorMessage = "Fields cannot be left empty.";
                    ErrorMessageVisibility = true;
                    SuccessMessageVisibility = false;
                    MasterKey = RepeatMKey = string.Empty;
                    return;
                }

                if (MasterKey != RepeatMKey)
                {
                    ErrorMessage = "The keys do not match.";
                    ErrorMessageVisibility = true;
                    SuccessMessageVisibility = false;
                    MasterKey = RepeatMKey = string.Empty;
                    return;
                }

                try
                {
                    currentUser.MasterKey.SetMasterKey(MasterKey);

                    SuccessMessage = "Master key created successfully.";
                    SuccessMessageVisibility = true;
                    ErrorMessageVisibility = false;

                    currentUser.SaveToFile();

                    var mainWindow = App.GetService<MainWindow>();
                    mainWindow?.Show();

                    OnExit();
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Error saving key: {ex.Message}";
                    ErrorMessageVisibility = true;
                    SuccessMessageVisibility = false;
                }

            }
            else
            {
                bool isCorrect = currentUser.MasterKey.CheckMasterKey(MasterKey ?? string.Empty);

                if (!isCorrect)
                {
                    ErrorMessage = "The key file was not found or the key is incorrect.";
                    ErrorMessageVisibility = true;
                    SuccessMessageVisibility = false;
                    MasterKey = RepeatMKey = string.Empty;
                    return;
                }
                else if (isCorrect)
                {
                    SuccessMessage = "Correct master key.";
                    SuccessMessageVisibility = true;
                    ErrorMessageVisibility = false;

                    var mainWindow = App.GetService<MainWindow>();
                    mainWindow?.Show();

                    OnExit();
                }
                else if (string.IsNullOrWhiteSpace(MasterKey))
                {
                    ErrorMessage = "Fields cannot be left blank.";
                    ErrorMessageVisibility = true;
                    SuccessMessageVisibility = false;
                    return;
                }
            }
        }

        private void OnExit()
        {
            Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault()?.Close();
        }

        [RelayCommand]
        private void CloseLogin()
        {
            Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault()?.Close();
        }
    }
}
