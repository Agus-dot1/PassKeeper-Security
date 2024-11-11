using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassKeeper.Helpers;
using PassKeeper.Models;
using PassKeeper.ViewModels.Windows;
using PassKeeper.Views.Windows;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Controls;
using MessageBox = Wpf.Ui.Controls.MessageBox;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;
using TextBlock = Wpf.Ui.Controls.TextBlock;

namespace PassKeeper.ViewModels.Pages
{
    public partial class PasswordViewModel : ObservableObject
    {
        [ObservableProperty] public string? searchText;
        private LoginWindowViewModel loginWindowViewModel = new LoginWindowViewModel();
        public ObservableCollection<Passwords> PasswordsCollection { get; } = new ObservableCollection<Passwords>();
        public ObservableCollection<Passwords> FilteredPasswordsCollection { get; } = new ObservableCollection<Passwords>();


        public PasswordViewModel()
        {
            LoadPasswords();
        }

        private void LoadPasswords()
        {
            if (File.Exists(loginWindowViewModel.currentUser.FilePath) && loginWindowViewModel.currentUser.Passwords.Count > 0)
                PasswordsCollection.Clear();
            foreach (var password in loginWindowViewModel.currentUser.Passwords)
            {
                PasswordsCollection.Add(password);
            }
            FilterPasswords();
        }

        private void FilterPasswords()
        {
            FilteredPasswordsCollection.Clear();
            foreach (var password in PasswordsCollection)
            {
                if (string.IsNullOrEmpty(SearchText) || password.Name.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase))
                {
                    FilteredPasswordsCollection.Add(password);
                }
            }
        }

        partial void OnSearchTextChanged(string? value)
        {
            FilterPasswords();
        }


        [RelayCommand]
        public void AddPassword()
        {
            AddPasswordWindow addPasswordWindow = new AddPasswordWindow();
            AddPasswordViewModel addPasswordViewModel = new AddPasswordViewModel();

            addPasswordViewModel.FilePath = loginWindowViewModel.currentUser.FilePath;
            addPasswordWindow.DataContext = addPasswordViewModel;

            addPasswordWindow.ShowDialog();

            if (addPasswordViewModel.PasswordAdded == true)
            {
                var newPassword = new Passwords
                {
                    Id = PasswordsCollection.Count,
                    Name = addPasswordViewModel.Name,
                    Username = addPasswordViewModel.Username,
                    Password = addPasswordViewModel.GeneratedPassword,
                    Icon = addPasswordViewModel.Icon,
                    Url = addPasswordViewModel.Url,
                    Notes = addPasswordViewModel.Note
                };


                loginWindowViewModel.currentUser.Passwords.Add(newPassword);
                loginWindowViewModel.currentUser.SaveToFile();
                PasswordsCollection.Add(newPassword);
            }
            FilterPasswords();
        }

        [RelayCommand]
        public void CopyUrl(Passwords password)
        {
            if (password.Url == null) return;
            Clipboard.SetText(password.Url);
        }

        [RelayCommand]
        public void CopyUsername(Passwords password)
        {
            if (password.Username == null) return;
            Clipboard.SetText(password.Username);
        }


        [RelayCommand]
        public void CopyPassword(Passwords password)
        {
            if (password.Password == null) return;
            Clipboard.SetText(password.Password);
        }


        [RelayCommand]
        public void EditPassword(Passwords password)
        {
            AddPasswordWindow addPasswordWindow = new AddPasswordWindow();
            AddPasswordViewModel addPasswordViewModel = new AddPasswordViewModel();
            ObservableCollection<SymbolIcon> IconOptions = Icons.IconOptions;

            int icon = 0;
            foreach (var iconOption in IconOptions)
            {
                if (iconOption.Uid == password.Icon)
                {
                    icon = IconOptions.IndexOf(iconOption);
                    break;
                }
            }


            IconOptions[icon].Name = password.Icon;
            addPasswordViewModel.FilePath = loginWindowViewModel.currentUser.FilePath;
            addPasswordViewModel.GeneratedPassword = password.Password;
            addPasswordViewModel.Name = password.Name;
            addPasswordViewModel.Username = password.Username;
            addPasswordViewModel.Url = password.Url;
            addPasswordViewModel.Note = password.Notes;
            addPasswordWindow.DataContext = addPasswordViewModel;

            addPasswordWindow.ShowDialog();

            if (addPasswordViewModel.PasswordAdded == true)
            {
                password.Id = password.Id;
                password.Name = addPasswordViewModel.Name;
                password.Username = addPasswordViewModel.Username;
                password.Password = addPasswordViewModel.GeneratedPassword;
                if (password.Icon == addPasswordViewModel.Icon)
                {
                    password.Icon = IconOptions[icon].Name;
                }
                else
                {
                    password.Icon = addPasswordViewModel.Icon;
                }
                password.Url = addPasswordViewModel.Url;
                password.Notes = addPasswordViewModel.Note;
                loginWindowViewModel.currentUser.SaveToFile();
            }
            FilterPasswords();
        }
        [RelayCommand]
        public void DeletePassword(Passwords password)
        {
            var messageBox = new MessageBox
            {
                Title = "¿Seguro que quieres borrar?",
                Content = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children =
                    {
                        new SymbolIcon { Symbol = SymbolRegular.Warning12, Foreground = new SolidColorBrush(Colors.OrangeRed), FontSize = 24, Width = 20, Height = 28, Margin = new Thickness(0, 0, 10, 0) },
                        new TextBlock { Text = "No se puede deshacer esta operación.", VerticalAlignment = VerticalAlignment.Center }
                    }
                },
                PrimaryButtonText = "Borrar",
                CloseButtonText = "Cancelar",
                MinWidth = 300,
                MinHeight = 100,
            };
            var result = messageBox.ShowDialogAsync();
            if (result.Result == MessageBoxResult.Primary)
            {
                loginWindowViewModel.currentUser.Passwords.Remove(password);
                loginWindowViewModel.currentUser.SaveToFile();
                PasswordsCollection.Remove(password);
            }
            FilterPasswords();
        }

    }
}
