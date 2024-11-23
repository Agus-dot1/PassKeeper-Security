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
        public bool IsPasswordsNull => !FilteredPasswordsCollection.Any();
        [ObservableProperty] private string? searchText;
        private readonly LoginWindowViewModel loginWindowViewModel = new LoginWindowViewModel();
        public ObservableCollection<Passwords> PasswordsCollection { get; set; } = new ObservableCollection<Passwords>();
        public ObservableCollection<Passwords> FilteredPasswordsCollection { get; set; } = new ObservableCollection<Passwords>();


        public PasswordViewModel()
        {
            FilteredPasswordsCollection.CollectionChanged += (s, e) => OnPropertyChanged(nameof(IsPasswordsNull));
            LoadPasswords();
        }

        public void LoadPasswords()
        {
            if (File.Exists(loginWindowViewModel.currentUser.FilePath) && loginWindowViewModel.currentUser.Passwords.Count > 0) {
                PasswordsCollection.Clear();
                foreach (var password in loginWindowViewModel.currentUser.Passwords)
                {
                    PasswordsCollection.Add(password);
                }
            }
            FilterPasswords();
            OnPropertyChanged(nameof(IsPasswordsNull));
            
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
        private void AddPassword()
        {
            try
            {
                var addPasswordWindow = new AddPasswordWindow();
                var addPasswordViewModel = new AddPasswordViewModel
                {
                    FilePath = loginWindowViewModel.currentUser.FilePath
                };

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
                OnPropertyChanged(nameof(IsPasswordsNull));
            }
            catch (Exception ex)
            {
                var messageBox = new MessageBox
                {
                    Title = "Error creating the password",
                    Content = new TextBlock
                    {
                        Text = $"Error creating the password: {ex.Message}",
                    },
                    Background = new SolidColorBrush(Color.FromArgb(255, 16, 23, 41)),
                    CloseButtonText = "Accept"
                };
                messageBox.ShowDialogAsync();
            }
        }

        [RelayCommand]
        private static void CopyUrl(Passwords password)
        {
            if (password.Url == null) return;
            Clipboard.SetText(password.Url);
        }

        [RelayCommand]
        private static void CopyUsername(Passwords password)
        {
            if (password.Username == null) return;
            Clipboard.SetText(password.Username);
        }


        [RelayCommand]
        private static void CopyPassword(Passwords password)
        {
            if (password.Password == null) return;
            Clipboard.SetText(password.Password);
        }


        [RelayCommand]
        private void EditPassword(Passwords password)
        {
            try
            {
                var addPasswordWindow = new AddPasswordWindow();
                var addPasswordViewModel = new AddPasswordViewModel();
                ObservableCollection<SymbolIcon> iconOptions = Icons.IconOptions;

                var icon = 0;
                foreach (var iconOption in iconOptions)
                {
                    if (iconOption.Uid == password.Icon)
                    {
                        icon = iconOptions.IndexOf(iconOption);
                        break;
                    }
                }


                iconOptions[icon].Name = password.Icon;
                addPasswordViewModel.FilePath = loginWindowViewModel.currentUser.FilePath;
                addPasswordViewModel.GeneratedPassword = password.Password;
                addPasswordViewModel.Name = password.Name;
                addPasswordViewModel.Username = password.Username;
                addPasswordViewModel.Url = password.Url;
                addPasswordViewModel.Note = password.Notes;
                addPasswordWindow.DataContext = addPasswordViewModel;

                addPasswordWindow.ShowDialog();

                if (addPasswordViewModel.PasswordAdded)
                {
                    password.Id = password.Id;
                    password.Name = addPasswordViewModel.Name;
                    password.Username = addPasswordViewModel.Username;
                    password.Password = addPasswordViewModel.GeneratedPassword;
                    password.Icon = password.Icon == addPasswordViewModel.Icon ? iconOptions[icon].Name : addPasswordViewModel.Icon;
                    password.Url = addPasswordViewModel.Url;
                    password.Notes = addPasswordViewModel.Note;
                    loginWindowViewModel.currentUser.SaveToFile();
                }
                LoadPasswords();
                FilterPasswords();
            }
            catch (Exception ex)
            {
                var messageBox = new MessageBox
                {
                    Title = "Error",
                    Content = new TextBlock
                    {
                        Text = ex.Message,
                        VerticalAlignment = VerticalAlignment.Center
                    },
                    Background = new SolidColorBrush(Color.FromArgb(255, 16, 23, 41)),
                    CloseButtonText = "Accept"
                };
                messageBox.ShowDialogAsync();
            }
        }
        [RelayCommand]
        private void DeletePassword(Passwords password)
        {
            var messageBox = new MessageBox
            {
                Title = "Are you sure you want to delete?",
                Content = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children =
                    {
                        new SymbolIcon { Symbol = SymbolRegular.Warning12, Foreground = new SolidColorBrush(Colors.OrangeRed), FontSize = 24, Width = 20, Height = 28, Margin = new Thickness(0, 0, 10, 0) },
                        new TextBlock { Text = "This action cannot be undone.", VerticalAlignment = VerticalAlignment.Center }
                    }
                },
                Background = new SolidColorBrush(Color.FromArgb(255, 16, 23, 41)),
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                MinWidth = 300,
                MinHeight = 100,
            };
            var result = messageBox.ShowDialogAsync();
            if (result.Result == MessageBoxResult.Primary)
            {
                try
                {
                    loginWindowViewModel.currentUser.Passwords.Remove(password);
                    loginWindowViewModel.currentUser.SaveToFile();
                    PasswordsCollection.Remove(password);
                }
                catch (Exception ex)
                {
                    var messageBox2 = new MessageBox
                    {
                        Title = "Error deleting password",
                        Content = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Children =
                            {
                                new TextBlock
                                {
                                    Text = $"Error deleting password: {ex.Message}",
                                    VerticalAlignment = VerticalAlignment.Center
                                }
                            }
                        },
                        Background = new SolidColorBrush(Color.FromArgb(255, 16, 23, 41)),
                        CloseButtonText = "Accept",
                    };
                    messageBox2.ShowDialogAsync();
                }
            }
            FilterPasswords();
        }

    }
}
