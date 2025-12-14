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
using XamarinPhoneContact.Helper;

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
		builder.Services.AddTransient<IReadUpdatePhoneContactData, ReadUpdatePhoneContactData>();

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
	static async Task InitializeContactControlAsync()
	{
		try
		{
			System.Diagnostics.Debug.WriteLine("🔵 Starting InitializeContactControlAsync...");
			// 1️⃣ First - Waits for completion
			System.Diagnostics.Debug.WriteLine("🔵 Step 1: Initializing KKControlSetup...");

			await MauiServiceProvider.GetService<IKKControlSetup>().Initialize();

			System.Diagnostics.Debug.WriteLine("✅ Step 1: KKControlSetup initialized successfully");

			// 2️⃣ Second - Only runs AFTER first completes
			System.Diagnostics.Debug.WriteLine("🔵 Step 2: Check for contact permissions to read data...");
			var contactPermissionGranted = await MauiServiceProvider.GetService<IKKContactPermissionRequest>().GetContactAuthorizationStatus();
			if (contactPermissionGranted)
			{
				System.Diagnostics.Debug.WriteLine("🔵 Step 3:Check if local db first time full sync done. Then only perform update sync..");
				var lastcheckDbSyncStatus = await MauiServiceProvider.GetService<IKKPhoneContactData>().CheckLocalDbFirstTimeSyncStatusAsync();
				if (lastcheckDbSyncStatus)
				{
					System.Diagnostics.Debug.WriteLine(":📱  Performing updated sync...");
					var result = await MauiServiceProvider.GetService<IReadUpdatePhoneContactData>().SyncContactChangesAsync();
					if (result == KKContactResulType.SyncTokenFailure)
					{
						await MauiServiceProvider.GetService<IKKContactControlDbOperation>().DeleteAllDataFromDbTable();
						await MauiServiceProvider.GetService<IKKPhoneContactData>().GetAllContactFromPhoneAndStoreToLocalDbAsync();
						System.Diagnostics.Debug.WriteLine("✅ Step 2: Phone updated contacts synced successfully");
						return;
					}
					System.Diagnostics.Debug.WriteLine("✅ Step 2: Phone updated contacts synced successfully");
					return;
				}
				else
				{
					System.Diagnostics.Debug.WriteLine("Step 4:📱 Performing first time full sync...");
					var result = await MauiServiceProvider.GetService<IKKPhoneContactData>().GetAllContactFromPhoneAndStoreToLocalDbAsync();
					if (result == KKContactResulType.FirstSynCompleted)
					{
						System.Diagnostics.Debug.WriteLine("✅ Step 2: Phone first time contacts synced successfully");
						return;
					}
					else
					{
						System.Diagnostics.Debug.WriteLine("❌  Step 2: Phone first time contacts synced not completed successfully");
						return;
					}

				}
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("⚠️ Step 2: Contact permission denied, skipping phone contacts sync");
			}
			System.Diagnostics.Debug.WriteLine("✅ Both steps completed sequentially");
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"❌ Error in InitializeContactControlAsync: {ex.Message}");
			System.Diagnostics.Debug.WriteLine($"❌ Stack Trace: {ex.StackTrace}");
		}
	}
	public static MauiAppBuilder SetKKContactControl(this MauiAppBuilder mauiAppBuilder)
	{
		mauiAppBuilder.ConfigureLifecycleEvents(events =>
			{
#if IOS
				events.AddiOS(iOS => iOS.WillFinishLaunching((app, __) =>
					{
						// Initialize contact control asynchronously
						Task.Run(async () => await InitializeContactControlAsync());
						return true;
					}));
#elif ANDROID
				events.AddAndroid(android => android
		.OnCreate(async (activity, bundle) =>
		{
			Task.Run(async () => await InitializeContactControlAsync());
		})
);
#endif
			});

		return mauiAppBuilder;
	}
}
