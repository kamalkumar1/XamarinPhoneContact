using Microsoft.Extensions.Logging;
using XamarinPhoneContact.Helper;

namespace XamarinPhoneContact;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			//SetKKContactControl is extension method to register all services of MauiPhoneContactLibrary
			//you must call this method to use maui phone contact library
			.SetKKContactControl()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		//builder.Services.AddTransient<IContact, ContactList>();
		builder.Services.AddSingleton<AppShell>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
