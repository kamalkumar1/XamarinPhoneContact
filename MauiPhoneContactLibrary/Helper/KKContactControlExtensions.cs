using Microsoft.Maui.LifecycleEvents;
using MauiPhoneContactLibrary.Interface.LocalDB;
using MauiPhoneContactLibrary.Interface;
#if IOS
using MauiPhoneContactLibrary.Platforms.iOS;
#elif ANDROID
using MauiPhoneContactLibrary.Platforms.Android;
#endif
using MauiPhoneContactLibrary.Model;
using MauiPhoneContactLibrary.Model.LocalSql;
using MauiPhoneContactLibrary.Service.Interface;
using MauiPhoneContactLibrary.Service;
using MauiPhoneContactLibrary.View;
using MauiPhoneContactLibrary.ViewModel;
using System.Runtime.Versioning;

namespace MauiPhoneContactLibrary.Helper;

/// <summary>
/// Extension methods for configuring KK Contact Control in MAUI applications
/// </summary>
public static class KKContactControlExtensions
{
  /// <summary>
  /// Configures all services and lifecycle events required for KK Contact Control
  /// </summary>
  /// <param name="mauiAppBuilder">The MAUI app builder</param>
  /// <returns>The configured MAUI app builder</returns>
  [SupportedOSPlatform("android26.0")]
  [SupportedOSPlatform("ios15.0")]
  public static MauiAppBuilder SetKKContactControl(this MauiAppBuilder mauiAppBuilder)
  {
    // Dependency Injection for Service and Interface
    mauiAppBuilder.Services.AddTransient<IKKControlSetup, KKContactBaseControl>();
    mauiAppBuilder.Services.AddTransient<ISqlLiteSetup, SQlLiteSetup>();
    mauiAppBuilder.Services.AddTransient<IKKCurdOperation, KKCurdOperation>();
    mauiAppBuilder.Services.AddTransient<IKKContactControlDbOperation, KKContactControlDbOperation>();
    mauiAppBuilder.Services.AddTransient<IKKPhoneContactData, ReadPhoneContactData>();
    mauiAppBuilder.Services.AddTransient<IReadUpdatePhoneContactData, ReadUpdatePhoneContactData>();

    // Dependency Injection for View and ViewModel
    mauiAppBuilder.Services.AddTransient<KKGroupContactView>();
    mauiAppBuilder.Services.AddTransient<KKGroupContactViewModel>();
    mauiAppBuilder.Services.AddTransient<KKSingleContactView>();
    mauiAppBuilder.Services.AddTransient<KKSingleContactViewModel>();

    // Contact Permission and Get Contact Services
    mauiAppBuilder.Services.AddTransient<IKKContactPermissionRequest, ContactPermissionRequest>();
    mauiAppBuilder.Services.AddTransient<IKKGetContact, KKGetContact>();

    // Sample Content Page
    mauiAppBuilder.Services.AddTransient<SampleContentPage>();
    mauiAppBuilder.Services.AddTransient<KKSampleContentPageViewModel>();

    // Configure lifecycle events
    mauiAppBuilder.ConfigureLifecycleEvents(events =>
      {
#if IOS
        events.AddiOS(iOS => iOS.WillFinishLaunching((app, __) =>
          {
            // Initialize contact control asynchronously
            _ = Task.Run(async () => await InitializeContactControlAsync());
            return true;
          }));
#elif ANDROID
        events.AddAndroid(android => android
    .OnCreate(async (activity, bundle) =>
    {
      _ = Task.Run(async () => await InitializeContactControlAsync());
    })
);
#endif
      });

    return mauiAppBuilder;
  }

  /// <summary>
  /// Initializes contact control services and syncs phone contacts
  /// </summary>
  private static async Task InitializeContactControlAsync()
  {
    try
    {
      System.Diagnostics.Debug.WriteLine("🔵 Starting InitializeContactControlAsync...");
      // 1️⃣ First - Waits for completion
      System.Diagnostics.Debug.WriteLine("🔵 Step 1: Initializing KKControlSetup...");

      var controlSetup = KKMauiServiceProvider.GetService<IKKControlSetup>();
      if (controlSetup == null)
      {
        System.Diagnostics.Debug.WriteLine("❌ Error: IKKControlSetup service is null");
        return;
      }
      await controlSetup.Initialize();

      System.Diagnostics.Debug.WriteLine("✅ Step 1: KKControlSetup initialized successfully");

      // 2️⃣ Second - Only runs AFTER first completes
      System.Diagnostics.Debug.WriteLine("🔵 Step 2: Check for contact permissions to read data...");
      var contactPermissionGranted = await KKMauiServiceProvider.GetService<IKKContactPermissionRequest>().GetContactAuthorizationStatus();
      if (contactPermissionGranted)
      {
        System.Diagnostics.Debug.WriteLine("🔵 Step 3:Check if local db first time full sync done. Then only perform update sync..");
        var lastcheckDbSyncStatus = await KKMauiServiceProvider.GetService<IKKPhoneContactData>().CheckLocalDbFirstTimeSyncStatusAsync();
        if (lastcheckDbSyncStatus)
        {
          System.Diagnostics.Debug.WriteLine(":📱  Performing updated sync...");
          var result = await KKMauiServiceProvider.GetService<IReadUpdatePhoneContactData>().SyncContactChangesAsync();
          if (result == KKContactResulType.SyncTokenFailure)
          {
            await KKMauiServiceProvider.GetService<IKKContactControlDbOperation>().DeleteAllDataFromDbTable();
            await KKMauiServiceProvider.GetService<IKKPhoneContactData>().GetAllContactFromPhoneAndStoreToLocalDbAsync();
            System.Diagnostics.Debug.WriteLine("✅ Step 2: Phone updated contacts synced successfully");
            return;
          }
          System.Diagnostics.Debug.WriteLine("✅ Step 2: Phone updated contacts synced successfully");
          return;
        }
        else
        {
          System.Diagnostics.Debug.WriteLine("Step 4:📱 Performing first time full sync...");
          var result = await KKMauiServiceProvider.GetService<IKKPhoneContactData>().GetAllContactFromPhoneAndStoreToLocalDbAsync();
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
}
