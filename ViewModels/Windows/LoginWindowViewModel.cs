using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassKeeper.Models;
using PassKeeper.Views.Windows;
using Wpf.Ui.Controls;
using MessageBox = Wpf.Ui.Controls.MessageBox;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;
using TextBlock = Wpf.Ui.Controls.TextBlock;

namespace PassKeeper.ViewModels.Windows;

public partial class LoginWindowViewModel : ObservableObject
{
    public UserModel? CurrentUser = App.GetService<UserModel>();
    [ObservableProperty] private string? masterKey;
    [ObservableProperty] private string? repeatMKey;
    [ObservableProperty] private bool isNewUser;
    [ObservableProperty] private string? createButtonContent;
    [ObservableProperty] private string? errorMessage;
    [ObservableProperty] private string? successMessage;
    [ObservableProperty] private bool errorMessageVisibility;
    [ObservableProperty] private bool successMessageVisibility;
    [ObservableProperty] private int passwordStrength;
    [ObservableProperty] private string? passwordStrengthText;
    [ObservableProperty] private string? passwordStrengthColor;
    [ObservableProperty] private double animatedPasswordStrength;
    [ObservableProperty] private bool isLoginVisible;

    public int UserLoaded { get; internal set; }

    public LoginWindowViewModel()
    {
        var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "databases", "database.pks");
        CurrentUser = UserModel.LoadFromFile(dbPath) ?? new UserModel(new MasterKeyModel());
        isLoginVisible = string.IsNullOrEmpty(CurrentUser.MasterKey.HashedKey);
        CreateButtonContent = isLoginVisible ? "Create" : "Open Vault";
        IsNewUser = !isLoginVisible;
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
                    new SymbolIcon
                    {
                        Symbol = SymbolRegular.Warning12, Foreground = new SolidColorBrush(Colors.OrangeRed),
                        FontSize = 24, Width = 20, Height = 28, Margin = new Thickness(0, 0, 10, 0)
                    },
                    new TextBlock
                        { Text = "Current data will be deleted.", VerticalAlignment = VerticalAlignment.Center }
                }
            },
            Background = new SolidColorBrush(Color.FromArgb(255, 16, 23, 41)),
            PrimaryButtonText = "Accept",
            CloseButtonText = "Cancel",
            MinWidth = 300,
            MinHeight = 100
        };
        var result = await messageBox.ShowDialogAsync();

        if (result == MessageBoxResult.Primary)
        {
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "databases", "database.pks");

            if (File.Exists(dbPath))
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

            CurrentUser = new UserModel(new MasterKeyModel());
            IsNewUser = false;
            IsLoginVisible = true;
            CreateButtonContent = "Create";
            MasterKey = RepeatMKey = string.Empty;
        }
    }

    [RelayCommand]
    private async Task CreateOrLogin()
    {
        if (IsLoginVisible)
        {
            if (string.IsNullOrEmpty(MasterKey) || string.IsNullOrEmpty(RepeatMKey))
            {
                ErrorMessage = "Fields cannot be left empty.";
                ErrorMessageVisibility = true;
                SuccessMessageVisibility = false;
                MasterKey = RepeatMKey = string.Empty;
                return;
            }
            
            if (MasterKey.Any(ch => ch > 127))
            {
                ErrorMessage = "The master key contains invalid characters.";
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

            if (PasswordStrengthText is "Weak" or "Very Weak" or "Medium")
            {
                var messageBox = new MessageBox
                {
                    Title = "Weak password warning",
                    Content = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Children =
                        {
                            new SymbolIcon
                            {
                                Symbol = SymbolRegular.Warning12, Foreground = new SolidColorBrush(Colors.OrangeRed),
                                FontSize = 24, Width = 20, Height = 28, Margin = new Thickness(0, 0, 10, 0)
                            },
                            new TextBlock
                            {
                                Text = "The current password is weak \nare you sure you want to continue?",
                                TextWrapping = TextWrapping.Wrap, VerticalAlignment = VerticalAlignment.Center
                            }
                        }
                    },
                    Background = new SolidColorBrush(Color.FromArgb(255, 16, 23, 41)),
                    PrimaryButtonText = "Accept",
                    CloseButtonText = "Cancel",
                    MinWidth = 100,
                    MinHeight = 200
                };
                var result = await messageBox.ShowDialogAsync();

                if (result == MessageBoxResult.Primary) {
                    try
                    {
                        if(CurrentUser is not null) {
                            CurrentUser.MasterKey.SetMasterKey(MasterKey);
                            CurrentUser.SaveToFile();
                        }
                        SuccessMessage = "Master key created successfully.";
                        SuccessMessageVisibility = true;
                        ErrorMessageVisibility = false;
                        

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
            }
            else
            {
                try
                {
                    if (CurrentUser is not null) {
                        CurrentUser.MasterKey.SetMasterKey(MasterKey);
                        CurrentUser.SaveToFile();
                    }
                    
                    SuccessMessage = "Master key created successfully.";
                    SuccessMessageVisibility = true;
                    ErrorMessageVisibility = false;

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
        }
        else
        {

            var isCorrect = CurrentUser?.MasterKey.CheckMasterKey(MasterKey);

            switch (isCorrect)
            {
                case false:
                    ErrorMessage = "The key file was not found or the key is incorrect.";
                    ErrorMessageVisibility = true;
                    SuccessMessageVisibility = false;
                    MasterKey = RepeatMKey = string.Empty;
                    return;
                case true:
                    SuccessMessage = "Correct master key.";
                    SuccessMessageVisibility = true;
                    ErrorMessageVisibility = false;

                    var mainWindow = App.GetService<MainWindow>();
                    mainWindow?.Show();

                    OnExit();
                    break;
            }
        }
    }

    partial void OnMasterKeyChanged(string? value)
    {
        CalculatePasswordStrength(value);
    }

    private void CalculatePasswordStrength(string? password)
    {
        if (string.IsNullOrEmpty(password))
        {
            PasswordStrength = 0;
            PasswordStrengthText = "";
            AnimatedPasswordStrength = 0;
            PasswordStrengthColor = "#db2d3c";
            return;
        }

        var score = 0;
        if (password.Length >= 8) score += 25;
        if (password.Any(char.IsUpper)) score += 25;
        if (password.Any(char.IsLower)) score += 25;
        if (password.Any(char.IsDigit)) score += 15;
        if (password.Any(ch => !char.IsLetterOrDigit(ch))) score += 10;

        PasswordStrength = score;
        if (score > 0)
            AnimatePasswordStrength(score);
        else
            AnimatedPasswordStrength = 0;


        switch (score)
        {
            case < 25:
                PasswordStrengthText = "Very Weak";
                PasswordStrengthColor = "#db2d3c";
                break;
            case < 50:
                PasswordStrengthText = "Weak";
                PasswordStrengthColor = "#e7aa3b";
                break;
            case < 75:
                PasswordStrengthText = "Medium";
                PasswordStrengthColor = "#fdd634";
                break;
            case < 100:
                PasswordStrengthText = "Strong";
                PasswordStrengthColor = "#358b5b";
                break;
            default:
                PasswordStrengthText = "Very Strong";
                PasswordStrengthColor = "#378bf3";
                break;
        }
    }

    private void AnimatePasswordStrength(int newStrength)
    {
        if (newStrength == 0)
        {
            AnimatedPasswordStrength = 0;
            return;
        }

        var animation = new DoubleAnimation
        {
            From = AnimatedPasswordStrength,
            To = newStrength,
            Duration = TimeSpan.FromSeconds(0.2),
            EasingFunction = new QuadraticEase()
        };

        var animationClock = animation.CreateClock();
        animationClock.CurrentTimeInvalidated += (s, e) =>
        {
            if (animationClock.CurrentProgress.HasValue)
                AnimatedPasswordStrength = animation.From.Value +
                                           (animation.To.Value - animation.From.Value) *
                                           animationClock.CurrentProgress.Value;
        };

        animationClock.Controller?.Begin();
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