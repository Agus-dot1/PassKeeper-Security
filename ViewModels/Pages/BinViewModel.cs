using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassKeeper.Models;
using PassKeeper.ViewModels.Windows;
using Wpf.Ui.Controls;
using MessageBox = Wpf.Ui.Controls.MessageBox;
using TextBlock = Wpf.Ui.Controls.TextBlock;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;

namespace PassKeeper.ViewModels.Pages;

public partial class BinViewModel : ObservableObject
{
    public ObservableCollection<PasswordModel> DeletedPasswordsCollection { get; set; } = new();
    public int OnUserLoaded { get; }
    public bool IsVisible { get; }

    public readonly LoginWindowViewModel? loginWindowViewModel = App.GetService<LoginWindowViewModel>();
     
    // PRIMERO QUYE NADA ASEGURATE DE QUE LA LISTA DE MIERDA SE CARGUE BIEN Y SE ACTUALICE. SEGUNDO AGREGA FUNCIONALIDAD DE RESTAURAR Y ELIMINAR POR COMPLETO EL PASSWORD                   

    [ObservableProperty]
    private string? searchText;

    public BinViewModel() {
        var currentUser = loginWindowViewModel?.CurrentUser;
        if (loginWindowViewModel != null) {
            loginWindowViewModel.UserLoaded += OnUserLoaded;
        }
        if (currentUser == null) {
            IsVisible = false;
        }
        LoadDeletedPasswords();
    }
    public void LoadDeletedPasswords() {
        var currentUser = loginWindowViewModel?.CurrentUser;

        if (currentUser == null || string.IsNullOrEmpty(currentUser.FilePath)) {
            return;
        }

        DeletedPasswordsCollection.Clear();

        foreach (var password in currentUser.Passwords.Where(p => p.IsDeleted)) {
            DeletedPasswordsCollection.Add(password);
        }
    }


    partial void OnSearchTextChanged(string? value) {
        FilterPasswords();
    }

    private void FilterPasswords() {
        if (string.IsNullOrEmpty(SearchText)) {
            foreach (var password in DeletedPasswordsCollection) {
                password.IsVisible = true;
            }
        }
        else {
            foreach (var password in DeletedPasswordsCollection) {
                password.IsVisible = password.Name != null &&
                                     password.Name.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase);
            }
        }
    }

    [RelayCommand]
    private void RestorePassword(PasswordModel passwordModel)
    {
        if (passwordModel == null) {
            return;
        }
        passwordModel.IsDeleted = false;
        loginWindowViewModel?.CurrentUser?.SaveToFile();
        LoadDeletedPasswords();
        FilterPasswords();
    }

    [RelayCommand]
    private void DeletePassword(PasswordModel passwordModel) {
        var messageBox = new MessageBox {
            Title = "Are you sure you want to delete?",
            Content = new StackPanel {
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
        if (result.Result == MessageBoxResult.Primary) {
            try {
                DeletedPasswordsCollection.Remove(passwordModel);
                loginWindowViewModel?.CurrentUser?.SaveToFile();
                LoadDeletedPasswords();
                FilterPasswords();
            }
            catch (Exception ex) {
                var messageBox2 = new MessageBox {
                    Title = "Error deleting password",
                    Content = new StackPanel {
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