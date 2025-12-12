using XamarinPhoneContact.Interface;
using XamarinPhoneContact.Model;

namespace XamarinPhoneContact;


public partial class App : Application
{
	//readonly AppShell appShell;
	public App()
	{
		InitializeComponent();

		//appShell = mainPage;
		MainPage = new AppShell();
	}
}
