using XamarinPhoneContact.View;
using XamarinPhoneContact.ViewModel;
namespace XamarinPhoneContact;

public partial class AppShell
{
	public AppShell()
	{
		InitializeComponent();

		Mainpage.ContentTemplate = new DataTemplate(typeof(MainPage));
		//	Routing.RegisterRoute("KKContactViewModel", typeof(MobileContact));
		Routing.RegisterRoute("MobileContact", typeof(MobileContact));
	}
}
