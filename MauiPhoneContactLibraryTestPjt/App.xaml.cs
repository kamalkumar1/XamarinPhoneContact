using MauiPhoneContactLibrary.Helper;
using MauiPhoneContactLibrary.Model;
using MauiPhoneContactLibrary.Examples;
// Add this if ContactConfig is in this namespace

namespace MauiPhoneContactLibraryTestPjt
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            // Apply theme at startup
            //ApplyTheme(Application.Current.RequestedTheme);

            // Subscribe to theme changes
            Application.Current.RequestedThemeChanged += OnRequestedThemeChanged;
        }

        private void OnRequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
        {
            ApplyTheme(e.RequestedTheme);
        }

        private void ApplyTheme(AppTheme theme)
        {
            var config = ContactConfig.Instance;
            if (theme == AppTheme.Dark)
            {
                KKContactThemeConfiguration.ApplyDarkTheme();
                config.CollectionViewItemSpacing = 5;
            }
            else
            {
                KKContactThemeConfiguration.ApplyLightTheme();
                config.CollectionViewItemSpacing = 3;
            }
        }
    }
}