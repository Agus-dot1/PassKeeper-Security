using CommunityToolkit.Mvvm.ComponentModel;
using PassKeeper.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;

namespace PassKeeper.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        private PasswordViewModel passwordViewModel = App.GetService<PasswordViewModel>();
        public ObservableCollection<Passwords> RecentPasswordsCollection { get; set; } = new ObservableCollection<Passwords>();
        public DashboardViewModel()
        {
            UpdatePasswordsCollection();
        }
        
        public void UpdatePasswordsCollection()
        {
            var recentPasswords = passwordViewModel.PasswordsCollection.
                Where(p => p.LastModified > DateTime.Now.AddDays(-7)).
                OrderByDescending(p => p.LastModified).Take(5).ToList();

            RecentPasswordsCollection.Clear();
            foreach (var password in recentPasswords)
            {
                RecentPasswordsCollection.Add(password);
            }

        }

    }
}
