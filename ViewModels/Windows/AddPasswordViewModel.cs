using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassKeeper.Views.Windows;
using Wpf.Ui.Controls;
using MessageBox = Wpf.Ui.Controls.MessageBox;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;
using TextBlock = Wpf.Ui.Controls.TextBlock;
using PassKeeper.Models;

namespace PassKeeper.ViewModels
{
    public partial class AddPasswordViewModel : ObservableObject
    {
        [ObservableProperty] public string? generatedPassword;
        [ObservableProperty] public string? name;
        [ObservableProperty] public string? username;
        [ObservableProperty] public string? url;
        [ObservableProperty] public string? note;
        [ObservableProperty] public string? icon;
        public bool PasswordAdded { get; set; } = false;
        


        [RelayCommand] public void SavePassword()
        {
            if (string.IsNullOrEmpty(Name))
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
                                Text = "Todos los campos son obligatorios",
                                VerticalAlignment = VerticalAlignment.Center
                            }
                        }
                    }
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
                    }
                }
            };


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

        private static string CreatePassword()
        {
            const string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            const string specialChars = "!@#$%^&*()";

            StringBuilder charPool = new StringBuilder(letters + numbers);
            charPool.Append(specialChars);

            StringBuilder password = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < 20; i++)
            {
                password.Append(charPool[random.Next(charPool.Length)]);
            }

            return password.ToString();
        }


        [RelayCommand]
        private static void Cancel()
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
