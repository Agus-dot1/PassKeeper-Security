using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using PassKeeper.Views.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using PassKeeper.Models;
using System.IO;
using PassKeeper.ViewModels.Windows;

namespace PassKeeper.ViewModels.Pages
{
    public partial class PasswordViewModel : ObservableObject
    {
        private LoginWindowViewModel loginWindowViewModel = new LoginWindowViewModel();
        public ObservableCollection<Passwords> PasswordsCollection { get; } = new ObservableCollection<Passwords>();

        public PasswordViewModel()
        {
            LoadPasswords();
        }

        private void LoadPasswords()
        {
            if(File.Exists(loginWindowViewModel.currentUser.FilePath) && loginWindowViewModel.currentUser.Passwords.Count > 0)
            PasswordsCollection.Clear();
            foreach (var password in loginWindowViewModel.currentUser.Passwords)
            {
                PasswordsCollection.Add(password);
            }
        }
        [RelayCommand]public void AddPassword()
        {
            AddPasswordWindow addPasswordWindow = new AddPasswordWindow();
            AddPasswordViewModel addPasswordViewModel = new AddPasswordViewModel();

            addPasswordViewModel.FilePath = loginWindowViewModel.currentUser.FilePath;
            addPasswordWindow.DataContext = addPasswordViewModel;

            addPasswordWindow.ShowDialog();

            if(addPasswordViewModel.PasswordAdded == true)
            {
                var newPassword = new Passwords
                { 
                    Id = PasswordsCollection.Count,
                    Name = addPasswordViewModel.Name, 
                    Username = addPasswordViewModel.Username, 
                    Password = addPasswordViewModel.GeneratedPassword,
                    Icon = addPasswordViewModel.Icon,
                };

                loginWindowViewModel.currentUser.Passwords.Add(newPassword);
                loginWindowViewModel.currentUser.SaveToFile();
                PasswordsCollection.Add(newPassword);
            }
        }

        [RelayCommand]public void DeletePassword(Passwords password)
        {
            
            loginWindowViewModel.currentUser.Passwords.Remove(password);
            loginWindowViewModel.currentUser.SaveToFile();
            PasswordsCollection.Remove(password);
        }

    }
}
