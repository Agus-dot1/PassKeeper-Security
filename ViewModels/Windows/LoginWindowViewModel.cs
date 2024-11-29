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
using PassKeeper.Helpers;

namespace PassKeeper.ViewModels.Windows
{
    public partial class LoginWindowViewModel : ObservableObject
    {
        public UserModel CurrentUser;
        [ObservableProperty] private string _email;
        [ObservableProperty] private string _masterKey;
        [ObservableProperty] private string _repeatMKey;
        [ObservableProperty] private bool _isNewUser;
        [ObservableProperty] private string? _createButtonContent;
        [ObservableProperty] private string? _errorMessage;
        [ObservableProperty] private string? _successMessage;
        [ObservableProperty] private bool _errorMessageVisibility;
        [ObservableProperty] private bool _successMessageVisibility;

        public LoginWindowViewModel()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "databases", "database.pks");
            CurrentUser = UserModel.LoadFromFile(dbPath) ?? new UserModel(new MasterKeyModel());
            IsNewUser = CurrentUser.IsRegistered;
            CreateButtonContent = IsNewUser ? "Create" : "Open Vault";
        }


        [RelayCommand]
        private async Task TriggerCreate()
        {
            if (IsNewUser)
            {
                IsNewUser = false;
                CreateButtonContent = "Open Vault";
                return;
            }
            var messageBox = new MessageBox
            {
                Title = "Create a new account",
                Content = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children =
                    {
                        new SymbolIcon { Symbol = SymbolRegular.Warning12, Foreground = new SolidColorBrush(Colors.OrangeRed), FontSize = 24, Width = 20, Height = 28, Margin = new Thickness(0, 0, 10, 0) },
                        new TextBlock { Text = "Are you sure you want to create a new account?", VerticalAlignment = VerticalAlignment.Center }
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

                CurrentUser = new UserModel(new MasterKeyModel());
                IsNewUser = true;
                CreateButtonContent = "Create";
                MasterKey = RepeatMKey = string.Empty;
            }
        }

        [RelayCommand]
        private async Task CreateOrLogin()
        {
            if (IsNewUser)
            {
                if (string.IsNullOrEmpty(MasterKey) || string.IsNullOrEmpty(RepeatMKey) || string.IsNullOrEmpty(Email))
                {
                    ErrorMessage = "Fields cannot be left empty.";
                    ErrorMessageVisibility = true;
                    SuccessMessageVisibility = false;
                    return;
                }
                
                if(Email.Contains(' '))
                {
                    ErrorMessage = "Email cannot contain spaces.";
                    ErrorMessageVisibility = true;
                    SuccessMessageVisibility = false;
                    return;
                }else if(!Email.Contains('@') || !Email.Contains('.'))
                {
                    ErrorMessage = "Invalid email.";
                    ErrorMessageVisibility = true;
                    SuccessMessageVisibility = false;
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
                
                if (MasterKey.Length < 8)
                {
                    ErrorMessage = "The password must be at least 8 characters long.";
                    ErrorMessageVisibility = true;
                    SuccessMessageVisibility = false;
                    MasterKey = RepeatMKey = string.Empty;
                    return;
                }

                try
                {
                    CurrentUser.Email = Email;
                    CurrentUser.PasswordHash = PasswordHasher.HashPassword(MasterKey);
                    CurrentUser.CreationDate = DateTime.Now;
                    CurrentUser.Id = Guid.NewGuid().ToString();
                    
                    
                    SuccessMessage = "Account created successfully.";
                    SuccessMessageVisibility = true;
                    ErrorMessageVisibility = false;

                    CurrentUser.IsRegistered = true;
                    await CurrentUser.Register();
                    

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
                CurrentUser.Email = Email;
                CurrentUser.PasswordHash = MasterKey;
                bool isCorrect = await CurrentUser.Login(Email, MasterKey);

                if (!isCorrect)
                {
                    ErrorMessage = "The account was not found check your email and password.";
                    ErrorMessageVisibility = true;
                    SuccessMessageVisibility = false;
                    MasterKey = RepeatMKey = string.Empty;
                }
                else if (isCorrect)
                {
                    SuccessMessage = "Correct credentials.";
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
                }
            }
        }

        private static void OnExit()
        {
            Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault()?.Close();
        }

        [RelayCommand]
        private static void CloseLogin()
        {
            Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault()?.Close();
        }
    }
}
