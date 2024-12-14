using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassKeeper.Helpers;
using PassKeeper.Views.Windows;
using Wpf.Ui.Controls;
using MessageBox = Wpf.Ui.Controls.MessageBox;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;
using TextBlock = Wpf.Ui.Controls.TextBlock;

namespace PassKeeper.ViewModels.Windows;

public partial class AddPasswordViewModel : ObservableObject
{
    [ObservableProperty] private string? generatedPassword;
    [ObservableProperty] private int passwordStrength;
    [ObservableProperty] private string passwordStrengthColor;
    [ObservableProperty] private double animatedPasswordStrength;
    [ObservableProperty] private int id;
    [ObservableProperty] private string? name;
    [ObservableProperty] private string? username;
    [ObservableProperty] private string? url;
    [ObservableProperty] private string? note;
    [ObservableProperty] private int selectedIcon;
    [ObservableProperty] private string? icon;

    public ObservableCollection<SymbolIcon> IconOptions { get; set; } = Icons.IconOptions;
    
    public string FilePath { get; internal set; }
    public bool PasswordAdded { get; private set; }

    public AddPasswordViewModel()
    {
        selectedIcon = 0;
    }

    [RelayCommand]
    private void SavePassword()
    {
        if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(GeneratedPassword))
        {
            var messageBox = new MessageBox
            {
                Title = "Error creating the password",
                Content = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "Username and password are required",
                            VerticalAlignment = VerticalAlignment.Center
                        }
                    }
                },
                Background = new SolidColorBrush(Color.FromArgb(255, 16, 23, 41)),
                CloseButtonText = "Accept"
            };
            messageBox.ShowDialogAsync();
            return;
        }

        var messageBox2 = new MessageBox
        {
            Title = "Password created successfully.",
            Content = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new TextBlock
                    {
                        Text = "The password has been created successfully.",
                        VerticalAlignment = VerticalAlignment.Center
                    }
                }
            },
            Background = new SolidColorBrush(Color.FromArgb(255, 16, 23, 41)),
            CloseButtonText = "Accept"
        };
        Icon = IconOptions[SelectedIcon].Name;
        PasswordAdded = true;
        messageBox2.ShowDialogAsync();
        Application.Current.Windows.OfType<AddPasswordWindow>().FirstOrDefault()?.Close();
    }

    [RelayCommand]
    private void GeneratePassword()
    {
        GeneratedPassword = CreatePassword();
    }

    partial void OnGeneratedPasswordChanged(string? value)
    {
        CalculatePasswordStrength(value);
    }

    private void CalculatePasswordStrength(string? password)
    {
        if (string.IsNullOrEmpty(password))
        {
            PasswordStrength = 0;
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

        PasswordStrengthColor = score switch
        {
            < 25 => "#db2d3c",
            < 50 => "#e7aa3b",
            < 75 => "#fdd634",
            < 100 => "#358b5b",
            _ => "#378bf3"
        };
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

    private static string CreatePassword()
    {
        const string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string numbers = "0123456789";
        const string specialChars = "!@#$%^&()-+?_=,<>/.;{}[]";

        var charPool = new StringBuilder(letters + numbers + specialChars);
        var password = new StringBuilder();

        var random = new Random();

        for (var i = 0; i < 22; i++) password.Append(charPool[random.Next(charPool.Length)]);

        return password.ToString();
    }


    [RelayCommand]
    private static void Cancel()
    {
        var messageBox = new MessageBox
        {
            Title = "Are you sure you want to cancel?",
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
                    new TextBlock { Text = "Changes will be lost.", VerticalAlignment = VerticalAlignment.Center }
                }
            },
            Background = new SolidColorBrush(Color.FromArgb(255, 16, 23, 41)),
            PrimaryButtonText = "Accept",
            CloseButtonText = "Cancel",
            MinWidth = 300,
            MinHeight = 100
        };
        var result = messageBox.ShowDialogAsync();
        if (result.Result == MessageBoxResult.Primary)
            Application.Current.Windows.OfType<AddPasswordWindow>().FirstOrDefault()?.Close();
    }
}