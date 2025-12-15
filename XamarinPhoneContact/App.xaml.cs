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
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}
