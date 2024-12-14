using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassKeeper.Helpers;
using PassKeeper.Models;
using PassKeeper.ViewModels.Windows;
using PassKeeper.Views.Windows;
using Wpf.Ui.Controls;
using MessageBox = Wpf.Ui.Controls.MessageBox;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;
using TextBlock = Wpf.Ui.Controls.TextBlock;

namespace PassKeeper.ViewModels.Pages;

public partial class PasswordViewModel : ObservableObject
{
    public bool IsPasswordsNull => FilteredPasswordsCollection.Any();
    [ObservableProperty] private string? searchText;
    private readonly LoginWindowViewModel loginWindowViewModel = new();
    private ObservableCollection<PasswordModel> PasswordsCollection { get; set; } = new();
    public ObservableCollection<PasswordModel> FilteredPasswordsCollection { get; set; } = new();
    public object Name { get; internal set; }

    public PasswordViewModel()
    {
        FilteredPasswordsCollection.CollectionChanged += (s, e) => OnPropertyChanged(nameof(IsPasswordsNull));
        LoadPasswords();
    }

    public void LoadPasswords()
    {
        if (File.Exists(loginWindowViewModel.CurrentUser.FilePath) &&
            loginWindowViewModel.CurrentUser.Passwords.Count > 0)
        {
            PasswordsCollection.Clear();
            foreach (var password in loginWindowViewModel.CurrentUser.Passwords) PasswordsCollection.Add(password);
        }

        FilterPasswords();
        OnPropertyChanged(nameof(IsPasswordsNull));
    }

    public void FilterPasswords()
    {
        FilteredPasswordsCollection.Clear();
        foreach (var password in PasswordsCollection)
            if (password.Name != null && (string.IsNullOrEmpty(SearchText) ||
                                          password.Name.Contains(SearchText,
                                              StringComparison.CurrentCultureIgnoreCase)))
                FilteredPasswordsCollection.Add(password);
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
                FilePath = loginWindowViewModel.CurrentUser.FilePath
            };

            addPasswordWindow.DataContext = addPasswordViewModel;
            addPasswordWindow.ShowDialog();

            if (addPasswordViewModel.PasswordAdded)
            {
                var newPassword = new PasswordModel
                {
                    Id = PasswordsCollection.Count,
                    Name = addPasswordViewModel.Name,
                    Username = addPasswordViewModel.Username,
                    Password = addPasswordViewModel.GeneratedPassword,
                    Icon = addPasswordViewModel.Icon,
                    Url = addPasswordViewModel.Url,
                    Notes = addPasswordViewModel.Note
                };


                loginWindowViewModel.CurrentUser.Passwords.Add(newPassword);
                loginWindowViewModel.CurrentUser.SaveToFile();
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
                    Text = $"Error creating the password: {ex.Message}"
                },
                Background = new SolidColorBrush(Color.FromArgb(255, 16, 23, 41)),
                CloseButtonText = "Accept"
            };
            messageBox.ShowDialogAsync();
        }
    }

    [RelayCommand]
    private static void CopyUrl(PasswordModel passwordModel)
    {
        if (passwordModel.Url == null) return;
        Clipboard.SetText(passwordModel.Url);
    }

    [RelayCommand]
    private static void CopyUsername(PasswordModel passwordModel)
    {
        if (passwordModel.Username == null) return;
        Clipboard.SetText(passwordModel.Username);
    }


    [RelayCommand]
    private static void CopyPassword(PasswordModel passwordModel)
    {
        if (passwordModel.Password == null) return;
        Clipboard.SetText(passwordModel.Password);
    }


    [RelayCommand]
    private void EditPassword(PasswordModel passwordModel)
    {
        try
        {
            var addPasswordWindow = new AddPasswordWindow();
            var addPasswordViewModel = new AddPasswordViewModel();
            var iconOptions = Icons.IconOptions;

            var icon = (from iconOption in iconOptions
                where iconOption.Uid == passwordModel.Icon
                select iconOptions.IndexOf(iconOption)).FirstOrDefault();


            iconOptions[icon].Name = passwordModel.Icon;
            addPasswordViewModel.FilePath = loginWindowViewModel.CurrentUser.FilePath;
            addPasswordViewModel.GeneratedPassword = passwordModel.Password;
            addPasswordViewModel.Name = passwordModel.Name;
            addPasswordViewModel.Username = passwordModel.Username;
            addPasswordViewModel.Url = passwordModel.Url;
            addPasswordViewModel.Note = passwordModel.Notes;
            addPasswordWindow.DataContext = addPasswordViewModel;

            addPasswordWindow.ShowDialog();

            if (addPasswordViewModel.PasswordAdded)
            {
                passwordModel.Id = passwordModel.Id;
                passwordModel.Name = addPasswordViewModel.Name;
                passwordModel.Username = addPasswordViewModel.Username;
                passwordModel.Password = addPasswordViewModel.GeneratedPassword;
                passwordModel.Icon = passwordModel.Icon == addPasswordViewModel.Icon
                    ? iconOptions[icon].Name
                    : addPasswordViewModel.Icon;
                passwordModel.Url = addPasswordViewModel.Url;
                passwordModel.Notes = addPasswordViewModel.Note;
                loginWindowViewModel.CurrentUser.SaveToFile();
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
    private void DeletePassword(PasswordModel passwordModel)
    {
        var messageBox = new MessageBox
        {
            Title = "Are you sure you want to delete?",
            Content = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new SymbolIcon
                    {
                        Symbol = SymbolRegular.Warning12, Foreground = new SolidColorBrush(Colors.OrangeRed),
                        FontSize = 24, Width = 20, Height = 28, Margin = new Thickness(0, 0, 10, 0)
                    },
                    new TextBlock
                        { Text = "This action cannot be undone.", VerticalAlignment = VerticalAlignment.Center }
                }
            },
            Background = new SolidColorBrush(Color.FromArgb(255, 16, 23, 41)),
            PrimaryButtonText = "Delete",
            CloseButtonText = "Cancel",
            MinWidth = 300,
            MinHeight = 100
        };
        var result = messageBox.ShowDialogAsync();
        if (result.Result == MessageBoxResult.Primary)
        {
            try
            {
                passwordModel.IsDeleted = true;
                passwordModel.DeletedDate = DateTime.Now;
                PasswordsCollection.Remove(passwordModel);
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
                    CloseButtonText = "Accept"
                };
                messageBox2.ShowDialogAsync();
            }
        }

        FilterPasswords();
    }
}