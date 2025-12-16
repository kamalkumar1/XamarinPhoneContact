using XamarinPhoneContact.Interface;
using XamarinPhoneContact.Model;
using XamarinPhoneContact.Examples;
using XamarinPhoneContact.Helper;

namespace XamarinPhoneContact;


public partial class App : Application
{
	//readonly AppShell appShell;
	public App()
	{
		InitializeComponent();

		// Apply theme based on current system theme
		ApplyTheme(Application.Current.RequestedTheme);

		// Subscribe to theme changes
		Application.Current.RequestedThemeChanged += OnRequestedThemeChanged;

		//appShell = mainPage;
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
			ContactThemeConfiguration.ApplyDarkTheme();
			config.CollectionViewItemSpacing = 5;
		}
		else
		{
			ContactThemeConfiguration.ApplyLightTheme();
			config.CollectionViewItemSpacing = 3;
		}
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}
