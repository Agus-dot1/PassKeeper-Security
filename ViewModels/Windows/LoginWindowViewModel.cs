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
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "databases", "database.json");
            currentUser = UserModel.LoadFromFile(dbPath) ?? new UserModel(new MasterKeyModel());
            IsNewUser = string.IsNullOrEmpty(currentUser.MasterKey.HashedKey);
            CreateButtonContent = IsNewUser ? "Crear" : "Ingresar";
        }


        [RelayCommand]
        private void TriggerCreate()
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
                PrimaryButtonText = "Aceptar",
                CloseButtonText = "Cancelar",
                MinWidth = 300,
                MinHeight = 100,
            };
            var result = messageBox.ShowDialogAsync();
            if (result.Result == MessageBoxResult.Primary)
            {
                string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "databases", "database.json");

                if (File.Exists(dbPath))
                {
                    try
                    {
                        File.Delete(dbPath);
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
            }
        }
    
        [RelayCommand]
        private void CreateOrLogin()
        {
            if (IsNewUser)
            {
                if (MasterKey.Length == 0 || RepeatMKey.Length == 0)
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

                    MainWindow? mainWindow = App.GetService<MainWindow>();
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

                    MainWindow? mainWindow = App.GetService<MainWindow>();
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

        private async void OnExit()
        {
            await Task.Delay(500);
            Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault()?.Close();
        }

        [RelayCommand]
        private async static Task CloseLogin()
        {
            await Task.Delay(500);
            Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault()?.Close();
        }
    }
}
