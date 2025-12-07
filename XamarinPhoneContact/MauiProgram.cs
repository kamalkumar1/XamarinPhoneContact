using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using XamarinPhoneContact.Interface;
using XamarinPhoneContact.Model;
using XamarinPhoneContact.Model.LocalSql;
using XamarinPhoneContact.Platforms;
namespace XamarinPhoneContact;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.SetKKContactControl()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
			builder.Services.AddSingleton<IContact, ContactList>();
		  builder.Services.AddSingleton<IKKControlSetup, KKContactBaseControl>();
			builder.Services.AddSingleton<ISqlLiteSetup, SQlLiteSetup>();
			builder.Services.AddSingleton<IKKCreateSqlTable, CreateSqlTable>();
			builder.Services.AddSingleton<IKKContactControlDbOperation, KKContactControlDbOperation>();
			 
			

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
