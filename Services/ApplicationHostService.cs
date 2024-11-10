using Microsoft.Extensions.Hosting;
using PassKeeper.Views.Pages;
using PassKeeper.Views.Windows;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Appearance;

namespace PassKeeper.Services
{
    /// <summary>
    /// Managed host of the application.
    /// </summary>
    public class ApplicationHostService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private INavigationWindow? _navigationWindow;
        private LoginWindow _loginWindow;

        public ApplicationHostService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
             await HandleActivationAsync();
        }



        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public async Task StopAsync(CancellationToken cancellationToken)
        {                
            await Task.CompletedTask;
        }

        /// <summary>
        /// Creates main window during activation.
        /// </summary>
        private async Task HandleActivationAsync()
        {
            if (Application.Current.Windows.OfType<Window>().Any())
            {
                _navigationWindow = (
                    _serviceProvider.GetService(typeof(INavigationWindow)) as INavigationWindow
                )!;

                ApplicationThemeManager.Apply(ApplicationTheme.Dark);

                _navigationWindow.ShowWindow();
                ApplicationThemeManager.Apply(ApplicationTheme.Dark);
                _navigationWindow.Navigate(typeof(PasswordsPage));
            }

            await Task.CompletedTask;
        }
    }
}
