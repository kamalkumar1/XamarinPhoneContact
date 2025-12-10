using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using XamarinPhoneContact.Interface.LocalDB;
using XamarinPhoneContact.Interface;
#if IOS
using XamarinPhoneContact.Platforms.iOS;
#elif ANDROID
using XamarinPhoneContact.Platforms.Android;
#endif
using XamarinPhoneContact.Model;
using XamarinPhoneContact.Model.LocalSql;

namespace XamarinPhoneContact;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		builder.Services.AddTransient<IContact, ContactList>();
		builder.Services.AddTransient<IKKControlSetup, KKContactBaseControl>();
		builder.Services.AddTransient<ISqlLiteSetup, SQlLiteSetup>();
		builder.Services.AddTransient<IKKCurdOperation, KKCurdOperation>();
		builder.Services.AddTransient<IKKContactControlDbOperation, KKContactControlDbOperation>();
		builder.Services.AddTransient<IKKPhoneContactData, ReadPhoneContactData>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
	public static MauiAppBuilder SetKKContactControl(this MauiAppBuilder mauiAppBuilder)
	{
		mauiAppBuilder.ConfigureLifecycleEvents(events =>
			{
#if IOS
				events.AddiOS(iOS => iOS.WillFinishLaunching((_, __) =>
					{
						MauiServiceProvider.GetService<IKKControlSetup>().Initialize();
						return true;
					}));
#elif ANDROID
				events.AddAndroid(android => android
		.OnCreate((activity, bundle) =>
		{
			// your init code here
			MauiServiceProvider.GetService<IKKControlSetup>().Initialize();
		})
);
#endif
			});

		return mauiAppBuilder;
	}
}
