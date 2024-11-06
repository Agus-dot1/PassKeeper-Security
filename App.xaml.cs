using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PassKeeper.Views.Windows;
using PassKeeper.ViewModels.Windows;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Wpf.Ui;
using PassKeeper.Views.Pages;
using PassKeeper.ViewModels;
using PassKeeper.Services;
using PassKeeper.ViewModels.Pages;

namespace PassKeeper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
        // https://docs.microsoft.com/dotnet/core/extensions/generic-host
        // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
        // https://docs.microsoft.com/dotnet/core/extensions/configuration
        // https://docs.microsoft.com/dotnet/core/extensions/logging
        private static readonly IHost _host = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration(c => { c.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)); })
            .ConfigureServices((context, services) =>
            {
                services.AddHostedService<ApplicationHostService>();

                // Page resolver service
                services.AddSingleton<IPageService, PageService>();

                // Theme manipulation
                services.AddSingleton<IThemeService, ThemeService>();

                // TaskBar manipulation
                services.AddSingleton<ITaskBarService, TaskBarService>();

                // Service containing navigation, same as INavigationWindow... but without window
                services.AddSingleton<INavigationService, NavigationService>();

                // Main window with navigation
                services.AddSingleton<MainWindow>();
                services.AddSingleton<INavigationWindow>(sp => sp.GetRequiredService<MainWindow>()); // Registro como INavigationWindow
                services.AddSingleton<MainWindowViewModel>();

                //Windows
                services.AddSingleton<LoginWindow>();
                services.AddSingleton<LoginWindowViewModel>();


                //Pages
                services.AddSingleton<AddPasswordWindow>();
                services.AddSingleton<AddPasswordViewModel>();
                services.AddSingleton<PasswordsPage>();
                services.AddSingleton<PasswordViewModel>();
                services.AddSingleton<SettingsPage>();
                services.AddSingleton<SettingsViewModel>();

            }).Build();

        /// <summary>
        /// Gets registered service.
        /// </summary>
        /// <typeparam name="T">Type of the service to get.</typeparam>
        /// <returns>Instance of the service or <see langword="null"/>.</returns>
        public static T? GetService<T>()
            where T : class
        {
            return _host.Services.GetService(typeof(T)) as T;
        }

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        private void OnStartup(object sender, StartupEventArgs e)
        {
            _host.Start();

            _host.Services.GetService<LoginWindowViewModel>();
            LoginWindow loginWindow = _host.Services.GetRequiredService<LoginWindow>();
            loginWindow.ShowDialog();
        }

        /// <summary>
        /// Occurs when the application is closing.
        /// </summary>
        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();

            _host.Dispose();
        }

        /// <summary>
        /// Occurs when an exception is thrown by an application but not handled.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
        }
    }
}
