using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using PassKeeper.Views.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using PassKeeper.Models;

namespace PassKeeper.ViewModels.Pages
{
    public partial class PasswordViewModel : ObservableObject
    {
        public ObservableCollection<Passwords> PasswordsCollection { get; } = new ObservableCollection<Passwords>();



        [RelayCommand]public void AddPassword()
        {
            AddPasswordWindow addPasswordWindow = new AddPasswordWindow();
            AddPasswordViewModel addPasswordViewModel = new AddPasswordViewModel();

            addPasswordWindow.DataContext = addPasswordViewModel;

            var result = addPasswordWindow.ShowDialog();

            if(addPasswordViewModel.PasswordAdded == true)
            {
                PasswordsCollection.Add(new Passwords
                { 
                    Name = addPasswordViewModel.Name, 
                    Username = addPasswordViewModel.Username, 
                    Password = addPasswordViewModel.GeneratedPassword,
                    Icon = addPasswordViewModel.Icon
                });
            }
        }



    }
}
