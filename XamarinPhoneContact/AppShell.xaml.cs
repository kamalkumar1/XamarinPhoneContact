using XamarinPhoneContact.View;
using XamarinPhoneContact.ViewModel;
namespace XamarinPhoneContact;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Mainpage.ContentTemplate = new DataTemplate(typeof(MainPage));
		// Register routes
		Routing.RegisterRoute(nameof(SampleContentPage), typeof(SampleContentPage));
		//Routing.RegisterRoute("KKContactViewModel", typeof(MobileContact));
		//Routing.RegisterRoute("MobileContact", typeof(SampleContentPage));
	}
}
