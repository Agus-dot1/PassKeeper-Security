using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassKeeper.Helpers;
using PassKeeper.Models;
using PassKeeper.ViewModels.Windows;
using PassKeeper.Views.Windows;
using PassKeeper.Services;
using System.Collections.ObjectModel;
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
        public static LoginWindowViewModel? _loginWindowViewModel = App.GetService<LoginWindowViewModel>();
        
        private readonly PasswordService _passwordService;
        [ObservableProperty] private string? _searchText;
        
        private ObservableCollection<PasswordsModel> PasswordsCollection { get; set; } = new ObservableCollection<PasswordsModel>();
        public ObservableCollection<PasswordsModel> FilteredPasswordsCollection { get; set; } = new ObservableCollection<PasswordsModel>();


        public PasswordViewModel()
        {
            FilteredPasswordsCollection.CollectionChanged += (s, e) => OnPropertyChanged(nameof(IsPasswordsNull));
            _passwordService = new PasswordService();
            _ = LoadPasswords();
        }

        private async Task LoadPasswords()
        {
            var currentUser = _loginWindowViewModel?.CurrentUser.Id;
            var passwords = await _passwordService.LoadPasswordsAsync(currentUser ?? string.Empty);
            
            PasswordsCollection.Clear();
            foreach (var password in passwords)
            {
                PasswordsCollection.Add(password);
            }
            
            OnPropertyChanged(nameof(IsPasswordsNull));
        }

        private void FilterPasswords()
        {
            FilteredPasswordsCollection.Clear();
            foreach (var password in PasswordsCollection)
            {
                if (password.Name != null && (string.IsNullOrEmpty(SearchText) || password.Name.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase)))
                {
                    FilteredPasswordsCollection = new ObservableCollection<PasswordsModel>(PasswordsCollection);
                }
                else
                {
                    var filtered = PasswordsCollection.Where(p => 
                        !string.IsNullOrEmpty(p.Name) && SearchText != null && p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                    );
                    FilteredPasswordsCollection = new ObservableCollection<PasswordsModel>(filtered);
                }
                OnPropertyChanged(nameof(FilteredPasswordsCollection));
            }
        }

        partial void OnSearchTextChanged(string? value)
        {
            FilterPasswords();
        }


        [RelayCommand]
        private async Task AddPassword()
        {
            try
            {
                var addPasswordWindow = new AddPasswordWindow();
                var addPasswordViewModel = new AddPasswordViewModel();
                
                addPasswordWindow.DataContext = addPasswordViewModel;
                addPasswordWindow.ShowDialog();

                if (addPasswordViewModel.PasswordAdded)
                {
                    var newPassword = new PasswordsModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = addPasswordViewModel.Name,
                        Username = addPasswordViewModel.Username,
                        Password = addPasswordViewModel.GeneratedPassword,
                        Icon = addPasswordViewModel.Icon,
                        Url = addPasswordViewModel.Url,
                        Notes = addPasswordViewModel.Note,
                        CreationDate = DateTime.Now
                    };
                    var passwordService = new PasswordService();
                    var currentUser = _loginWindowViewModel?.CurrentUser.Id;
                    await passwordService.AddPassword(newPassword, currentUser ?? string.Empty);
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
                        Text = $"Error creating the password:\n {ex.Message}",
                    },
                    Background = new SolidColorBrush(Color.FromArgb(255, 16, 23, 41)),
                    CloseButtonText = "Accept"
                };
                await messageBox.ShowDialogAsync();
            }
        }

        [RelayCommand]
        private static void CopyUrl(PasswordsModel passwordModel)
        {
            if (passwordModel.Url == null) return;
            Clipboard.SetText(passwordModel.Url);
        }

        [RelayCommand]
        private static void CopyUsername(PasswordsModel passwordModel)
        {
            if (passwordModel.Username == null) return;
            Clipboard.SetText(passwordModel.Username);
        }


        [RelayCommand]
        private static void CopyPassword(PasswordsModel passwordModel)
        {
            if (passwordModel.Password == null) return;
            Clipboard.SetText(passwordModel.Password);
        }


        [RelayCommand]
        private async Task EditPassword(PasswordsModel passwordModel)
        {
            try
            {
                var addPasswordWindow = new AddPasswordWindow();
                var addPasswordViewModel = new AddPasswordViewModel();
                ObservableCollection<SymbolIcon> iconOptions = Icons.IconOptions;

                var icon = 0;
                foreach (var iconOption in iconOptions)
                {
                    if (iconOption.Uid == passwordModel.Icon)
                    {
                        icon = iconOptions.IndexOf(iconOption);
                        break;
                    }
                }


                iconOptions[icon].Name = passwordModel.Icon;
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
                    passwordModel.Icon = passwordModel.Icon == addPasswordViewModel.Icon ? iconOptions[icon].Name : addPasswordViewModel.Icon;
                    passwordModel.Url = addPasswordViewModel.Url;
                    passwordModel.Notes = addPasswordViewModel.Note;
                }
                await LoadPasswords();
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
                await messageBox.ShowDialogAsync();
            }
        }
        [RelayCommand]
        private void DeletePassword(PasswordsModel passwordModel)
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
                    PasswordsCollection.Remove(passwordModel);
                }
                catch (Exception ex)
                {
                    var messageBox2 = new MessageBox
                    {
                        Title = "Error deleting passwordModel",
                        Content = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Children =
                            {
                                new TextBlock
                                {
                                    Text = $"Error deleting passwordModel: {ex.Message}",
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
