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
            CreateButtonContent = IsNewUser ? "Crear" : "Ingresar";
        }


        [RelayCommand]
        private async Task TriggerCreate()
        {
            var messageBox = new MessageBox
            {
                Title = "¿Seguro que quieres crear una nueva clave maestra?",
                Content = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children =
                    {
                        new SymbolIcon { Symbol = SymbolRegular.Warning12, Foreground = new SolidColorBrush(Colors.OrangeRed), FontSize = 24, Width = 20, Height = 28, Margin = new Thickness(0, 0, 10, 0) },
                        new TextBlock { Text = "Se borrarán los datos actuales.", VerticalAlignment = VerticalAlignment.Center }
                    }
                },
                Background = new SolidColorBrush(Color.FromArgb(255, 16, 23, 41)),
                PrimaryButtonText = "Aceptar",
                CloseButtonText = "Cancelar",
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
                        SuccessMessage = "Datos anteriores eliminados con éxito.";
                        SuccessMessageVisibility = true;
                        ErrorMessageVisibility = false;
                        MasterKey = RepeatMKey = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = $"Error al eliminar datos anteriores: {ex.Message}";
                        ErrorMessageVisibility = true;
                        SuccessMessageVisibility = false;
                        return;
                    }
                }

                currentUser = new UserModel(new MasterKeyModel());
                IsNewUser = true;
                CreateButtonContent = "Crear";
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
                    ErrorMessage = "No se pueden dejar campos vacíos.";
                    ErrorMessageVisibility = true;
                    SuccessMessageVisibility = false;
                    MasterKey = RepeatMKey = string.Empty;
                    return;
                }

                if (MasterKey != RepeatMKey)
                {
                    ErrorMessage = "Las claves no coinciden.";
                    ErrorMessageVisibility = true;
                    SuccessMessageVisibility = false;
                    MasterKey = RepeatMKey = string.Empty;
                    return;
                }

                try
                {
                    currentUser.MasterKey.SetMasterKey(MasterKey);

                    SuccessMessage = "Clave maestra creada con éxito.";
                    SuccessMessageVisibility = true;
                    ErrorMessageVisibility = false;

                    currentUser.SaveToFile();

                    var mainWindow = App.GetService<MainWindow>();
                    mainWindow?.Show();

                    OnExit();
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Error al guardar la clave: {ex.Message}";
                    ErrorMessageVisibility = true;
                    SuccessMessageVisibility = false;
                }

            }
            else
            {
                bool isCorrect = currentUser.MasterKey.CheckMasterKey(MasterKey ?? string.Empty);

                if (!isCorrect)
                {
                    ErrorMessage = "No se encontró el archivo de la clave o la clave es incorrecta.";
                    ErrorMessageVisibility = true;
                    SuccessMessageVisibility = false;
                    MasterKey = RepeatMKey = string.Empty;
                    return;
                }
                else if (isCorrect)
                {
                    SuccessMessage = "Clave maestra correcta.";
                    SuccessMessageVisibility = true;
                    ErrorMessageVisibility = false;

                    var mainWindow = App.GetService<MainWindow>();
                    mainWindow?.Show();

                    OnExit();
                }
                else if (string.IsNullOrWhiteSpace(MasterKey))
                {
                    ErrorMessage = "No se pueden dejar campos vacíos.";
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
