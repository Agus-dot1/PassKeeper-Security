using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using PassKeeper.Models;
using PassKeeper.Views.Windows;
using PassKeeper.Services;
using System.IO;
using System.Windows;
using Wpf.Ui;

namespace PassKeeper.ViewModels.Windows
{
    public class LoginWindowViewModel : ObservableObject
    {
        private UserModel _currentUser;
        private string? _masterKey;
        private string? _repeatMKey;
        public bool _isNewUser;
        private string? _createButtonContent;
        private string? _errorMessage;
        private string? _successMessage;
        private bool _errorMessageVisibility;
        private bool _successMessageVisibility;

        public string? CreateButtonContent
        {
            get { return _createButtonContent; }
            set
            {
                _createButtonContent = value;
                OnPropertyChanged(nameof(CreateButtonContent));
            }
        }
        public string? MasterKey
        {
            get { return _masterKey; }
            set
            {
                _masterKey = value;
                OnPropertyChanged(nameof(MasterKey));
            }
        }
        public string? RepeatKey
        {
            get { return _repeatMKey; }
            set
            {
                _repeatMKey = value;
                OnPropertyChanged(nameof(RepeatKey));
            }
        }

        public string? ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public string? SuccessMessage
        {
            get { return _successMessage; }
            set
            {
                _successMessage = value;
                OnPropertyChanged(nameof(SuccessMessage));
            }
        }

        public bool ErrorMessageVisibility
        {
            get { return _errorMessageVisibility; }
            set
            {
                _errorMessageVisibility = value;
                OnPropertyChanged(nameof(ErrorMessageVisibility));
            }
        }

        public bool SuccessMessageVisibility
        {
            get { return _successMessageVisibility; }
            set
            {
                _successMessageVisibility = value;
                OnPropertyChanged(nameof(SuccessMessageVisibility));
            }
        }

        public bool IsNewUser
        {
            get { return _isNewUser; }
            set
            {
                _isNewUser = value;
                OnPropertyChanged(nameof(IsNewUser));
                CreateButtonContent = IsNewUser ? "Crear" : "Ingresar";
            }
        }
        public LoginWindowViewModel()
        {
            _currentUser = new UserModel(string.Empty, new MasterKeyModel());
            
            CreateMasterKeyCommand = new RelayCommand(CreateOrLogin, CanCreateOrLogin);
            CloseLoginCommand = new RelayCommand(CloseLogin);
            CheckIfMasterKeyExists();
            CreateButtonContent = IsNewUser ? "Crear" : "Ingresar";

        }

        public RelayCommand CreateMasterKeyCommand { get; private set; }
        public RelayCommand CloseLoginCommand { get; private set; }

        private void CheckIfMasterKeyExists()
        {
            _currentUser.FilePath = "C:/Users/agusn/Desktop/database.txt";

            if (File.Exists(_currentUser.FilePath))
            {
                IsNewUser = false;
            }
            else
            {
                IsNewUser = true;
            }
        }


        private async void CreateOrLogin()
        {
            if (IsNewUser)
            {
                if (string.IsNullOrWhiteSpace(_masterKey) || string.IsNullOrWhiteSpace(RepeatKey))
                {
                    ErrorMessage = "No se pueden dejar campos vacíos.";
                    ErrorMessageVisibility = true;
                    SuccessMessageVisibility = false;
                    return;
                }

                if (_masterKey != RepeatKey)
                {
                    ErrorMessage = "Las claves no coinciden.";
                    ErrorMessageVisibility = true;
                    SuccessMessageVisibility = false;
                    return;
                }

                try
                {
                    _currentUser.MasterKey.SetMasterKey(_masterKey);


                    File.WriteAllText(_currentUser.FilePath, _currentUser.MasterKey.HashedKey);
                    SuccessMessage = "Clave maestra creada con éxito.";
                    SuccessMessageVisibility = true;
                    ErrorMessageVisibility = false;

                    UserLoggedIn();

                    await Task.Delay(2000);

                    MainWindow? mainWindow = App.GetService<MainWindow>();
                    mainWindow?.Show();

                    Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault()?.Close();
                    MessageBox.Show($"La base de datos fue guardada en {_currentUser.FilePath}");
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
                if (!string.IsNullOrEmpty(_currentUser.FilePath) && File.Exists(_currentUser.FilePath))
                {
                    string? storedHash = File.ReadLines(_currentUser.FilePath).FirstOrDefault();
                    _currentUser.MasterKey.InitializeHashedKey(storedHash ?? string.Empty);

                    bool isCorrect = _currentUser.MasterKey.CheckMasterKey(_masterKey ?? string.Empty);

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

                        UserLoggedIn();

                        await Task.Delay(2000);

                        MainWindow? mainWindow = App.GetService<MainWindow>();
                        mainWindow?.Show();

                        Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault()?.Close();
                    }
                    else if(string.IsNullOrWhiteSpace(MasterKey))
                    {
                        ErrorMessage = "No se pueden dejar campos vacíos.";
                        ErrorMessageVisibility = true;
                        SuccessMessageVisibility = false;
                        return;
                    }
                }
            }
        }
        
        private bool CanCreateOrLogin()
        {
            return true;
        }
        private void CloseLogin()
        {
            Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault()?.Close();
        }

        public bool UserLoggedIn()
        {
            return true;
        }
    }
}
