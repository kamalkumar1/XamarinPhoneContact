using MauiPhoneContactLibrary.View;

namespace MauiPhoneContactLibraryTestPjt
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(TestContactPage), typeof(TestContactPage));
        }
    }
}
