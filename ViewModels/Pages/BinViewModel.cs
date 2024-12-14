using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using PassKeeper.Models;
using PassKeeper.ViewModels.Windows;

namespace PassKeeper.ViewModels.Pages;

public partial class BinViewModel : ObservableObject {
    public ObservableCollection<PasswordModel> DeletedPasswordsCollection { get; set; } = new();
    public int OnUserLoaded { get; }

    public readonly LoginWindowViewModel? loginWindowViewModel = App.GetService<LoginWindowViewModel>();

    [ObservableProperty]
    private string? searchText;

    public BinViewModel() {
        var currentUser = App.GetService<UserModel>();
        if (currentUser != null) {
            LoadDeletedPasswords();
        }
        else {
            if (loginWindowViewModel != null) {
                loginWindowViewModel.UserLoaded += OnUserLoaded;
            }
        }
    }
    private void LoadDeletedPasswords() {
        var currentUser = App.GetService<UserModel>();

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
}