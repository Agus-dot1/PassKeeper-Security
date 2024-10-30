using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using PassKeeper.Views.Windows;
using System.Windows;

namespace PassKeeper.ViewModels.Pages
{
    public partial class PasswordViewModel : ObservableObject
    {
        
        [RelayCommand] 
        private void AddPassword()
        {
            AddPasswordWindow addPasswordWindow = App.GetService<AddPasswordWindow>;

            addPasswordWindow.ShowDialog();
        }


    }
}
