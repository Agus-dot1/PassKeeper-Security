using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassKeeper.Models;
using PassKeeper.Views.Windows;
using System.IO;
using System.Windows;
using Wpf.Ui.Appearance;

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
        private void CreateOrLogin()
        {
            if (IsNewUser)
            {
                if (MasterKey.Length == 0 || RepeatMKey.Length == 0)
                {
                    ErrorMessage = "No se pueden dejar campos vacíos.";
                    ErrorMessageVisibility = true;
                    SuccessMessageVisibility = false;
                    return;
                }

                if (MasterKey != RepeatMKey)
                {
                    ErrorMessage = "Las claves no coinciden.";
                    ErrorMessageVisibility = true;
                    SuccessMessageVisibility = false;
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

                    Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault()?.Close();
                    MessageBox.Show($"La base de datos fue guardada en {currentUser.FilePath}");
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
                    return;
                }
                else if (isCorrect)
                {
                    SuccessMessage = "Clave maestra correcta.";
                    SuccessMessageVisibility = true;
                    ErrorMessageVisibility = false;

                    MainWindow? mainWindow = App.GetService<MainWindow>();
                    mainWindow?.Show();
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
        [RelayCommand]
        private static void CloseLogin()
        {
            Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault()?.Close();
        }
    }
}
