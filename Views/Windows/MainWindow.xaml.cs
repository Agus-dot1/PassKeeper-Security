using PassKeeper.ViewModels.Windows;
using PassKeeper.Views.Pages;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace PassKeeper.Views.Windows
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INavigationWindow
    {
        public MainWindowViewModel ViewModel { get; }
        public MainWindow(
            MainWindowViewModel viewModel,
            INavigationService navigationService,
            IPageService pageService)
        {
            this.MaxHeight = SystemParameters.WorkArea.Height;

            ViewModel = viewModel;
            DataContext = this;

            SystemThemeWatcher.Watch(this);

            InitializeComponent();
            Loaded += OnLoaded;
            SetPageService(pageService);

            navigationService.SetNavigationControl(RootNavigation);

        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            RootNavigation.Navigate(typeof(PasswordsPage));
        }

        public INavigationView GetNavigation() => RootNavigation;

        public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);

        public void SetPageService(IPageService pageService) => RootNavigation.SetPageService(pageService);

        public void ShowWindow() => Show();

        public void CloseWindow() => Close();


        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Make sure that closing this window will begin the process of closing the application.
            Application.Current.Shutdown();
        }

        INavigationView INavigationWindow.GetNavigation()
        {
            throw new NotImplementedException();
        }

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }
    }
}
