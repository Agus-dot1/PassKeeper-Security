using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassKeeper.Helpers;
using PassKeeper.Views.Windows;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Controls;
using MessageBox = Wpf.Ui.Controls.MessageBox;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;
using TextBlock = Wpf.Ui.Controls.TextBlock;

namespace PassKeeper.ViewModels
{
    public partial class AddPasswordViewModel : ObservableObject
    {
        [ObservableProperty] public string? generatedPassword;
        [ObservableProperty] public int passwordStrength;
        [ObservableProperty] public int id;
        [ObservableProperty] public string? name;
        [ObservableProperty] public string? username;
        [ObservableProperty] public string? url;
        [ObservableProperty] public string? note;
        [ObservableProperty] public int selectedIcon;
        [ObservableProperty] public string? icon;

        public ObservableCollection<SymbolIcon> IconOptions { get; set; } = Icons.IconOptions;

        public string FilePath { get; internal set; }
        public bool PasswordAdded { get; set; } = false;
        
        public AddPasswordViewModel()
        {
            selectedIcon = 0;
        }

        [RelayCommand] public void SavePassword()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(GeneratedPassword))
            {
                var messageBox = new MessageBox
                {
                    Title = "Error al crear la contraseña",
                    Content = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Children =
                        {
                            new TextBlock
                            {
                                Text = "Usuario y contraseña son requeridos",
                                VerticalAlignment = VerticalAlignment.Center
                            }
                        }
                    },
                    Background = new SolidColorBrush(Color.FromArgb(255, 16, 23, 41)),
                    CloseButtonText = "Aceptar",
                };
                messageBox.ShowDialogAsync();
                return;
            }

            var messageBox2 = new MessageBox
            {
                Title = "Contraseña creada con exito",
                Content = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "La contraseña ha sido creada con exito",
                            VerticalAlignment = VerticalAlignment.Center
                        }
                    },
                },
                Background = new SolidColorBrush(Color.FromArgb(255, 16, 23, 41)),
                CloseButtonText = "Aceptar"
            };
            Icon = IconOptions[SelectedIcon].Name;
            PasswordAdded = true;
            messageBox2.ShowDialogAsync();
            Application.Current.Windows.OfType<AddPasswordWindow>().FirstOrDefault()?.Close();
            return;
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
                return;
            }

            int score = 0;
            if (password.Length >= 8) score += 25;
            if (password.Any(char.IsUpper)) score += 25;
            if (password.Any(char.IsLower)) score += 25;
            if (password.Any(char.IsDigit)) score += 15;
            if (password.Any(ch => !char.IsLetterOrDigit(ch))) score += 10;

            PasswordStrength = score;
        }

        private static string CreatePassword()
        {
            const string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            const string specialChars = "!@#$%^&()-+?_=,<>/.;{}[]";

            StringBuilder charPool = new StringBuilder(letters + numbers + specialChars);
            StringBuilder password = new StringBuilder();

            Random random = new Random();

            for (int i = 0; i < 22; i++)
            {
                password.Append(charPool[random.Next(charPool.Length)]);
            }

            return password.ToString();
        }


        [RelayCommand]
        public void Cancel()
        {
            var messageBox = new MessageBox
            {
                Title = "¿Seguro que quieres cancelar?",
                Content = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children =
                    {
                        new SymbolIcon { Symbol = SymbolRegular.Warning12, Foreground = new SolidColorBrush(Colors.OrangeRed), FontSize = 24, Width = 20, Height = 28, Margin = new Thickness(0, 0, 10, 0) },
                        new TextBlock { Text = "Se perderán los cambios.", VerticalAlignment = VerticalAlignment.Center }
                    }
                },
                Background = new SolidColorBrush(Color.FromArgb(255, 16, 23, 41)),
                PrimaryButtonText = "Aceptar",
                CloseButtonText = "Cancelar",
                MinWidth = 300,
                MinHeight = 100,
            };
            var result = messageBox.ShowDialogAsync();
            if (result.Result == MessageBoxResult.Primary)
            {
                Application.Current.Windows.OfType<AddPasswordWindow>().FirstOrDefault()?.Close();
            }
        }
    }
}
