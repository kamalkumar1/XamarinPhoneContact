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
using XamarinPhoneContact.Service.Interface;
using XamarinPhoneContact.Service;
using XamarinPhoneContact.View;
using XamarinPhoneContact.ViewModel;

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
		//builder.Services.AddTransient<IContact, ContactList>();
		builder.Services.AddSingleton<AppShell>();
		// Dependency Injection for Service and Interface
		builder.Services.AddTransient<IKKControlSetup, KKContactBaseControl>();
		builder.Services.AddTransient<ISqlLiteSetup, SQlLiteSetup>();
		builder.Services.AddTransient<IKKCurdOperation, KKCurdOperation>();
		builder.Services.AddTransient<IKKContactControlDbOperation, KKContactControlDbOperation>();
		builder.Services.AddTransient<IKKPhoneContactData, ReadPhoneContactData>();

		// Dependency Injection for View and ViewModel
		builder.Services.AddTransient<KKGroupContactView>();
		builder.Services.AddTransient<KKGroupContactViewModel>();
		builder.Services.AddTransient<KKSingleContactView>();
		builder.Services.AddTransient<KKSingleContactViewModel>();

		builder.Services.AddTransient<SampleContentPage>();
		builder.Services.AddTransient<KKSampleContentPageViewModel>();

		builder.Services.AddTransient<IKKContactPermissionRequest, ContactPermissionRequest>();
		builder.Services.AddTransient<IKKGetContact, KKGetContact>();

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
						Task.Run(() =>
						{
							MauiServiceProvider.GetService<IKKControlSetup>().Initialize();
							MauiServiceProvider.GetService<IKKPhoneContactData>().GetAllContactFromPhoneAndStoreToLocalDbAsync();

						});

						return true;
					}));
#elif ANDROID
				events.AddAndroid(android => android
		.OnCreate(async (activity, bundle) =>
		{
			await Task.Run(async () =>
				{
						// 1️⃣ First - Waits for completion
						await MauiServiceProvider.GetService<IKKControlSetup>().Initialize();
						
						// 2️⃣ Second - Only runs AFTER first completes
						await MauiServiceProvider.GetService<IKKPhoneContactData>().GetAllContactFromPhoneAndStoreToLocalDbAsync();
						
						//Debug.WriteLine("✅ Both steps completed sequentially");
				});
		})
);
#endif
			});

		return mauiAppBuilder;
	}
}
