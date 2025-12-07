using XamarinPhoneContact.Interface;
using XamarinPhoneContact.Model;

namespace XamarinPhoneContact;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage =  new NavigationPage(new MainPage());
	}
}
