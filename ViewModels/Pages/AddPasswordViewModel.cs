using System;
using System.Text;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PassKeeper.ViewModels
{
    public partial class AddPasswordViewModel : ObservableObject
    {
        [ObservableProperty]
        private int passwordLength = 12;

        [ObservableProperty]
        private bool includeSpecialChars = false;

        [ObservableProperty]
        private string generatedPassword;

        public ICommand GeneratePasswordCommand { get; }

        public AddPasswordViewModel()
        {
            GeneratePasswordCommand = new RelayCommand(GeneratePassword);
        }

        private void GeneratePassword()
        {
            GeneratedPassword = CreatePassword(PasswordLength, IncludeSpecialChars);
        }

        private string CreatePassword(int length, bool includeSpecialChars)
        {
            const string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            const string specialChars = "!@#$%^&*()";

            StringBuilder charPool = new StringBuilder(letters + numbers);
            if (includeSpecialChars)
            {
                charPool.Append(specialChars);
            }

            StringBuilder password = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                password.Append(charPool[random.Next(charPool.Length)]);
            }

            return password.ToString();
        }
    }
}
