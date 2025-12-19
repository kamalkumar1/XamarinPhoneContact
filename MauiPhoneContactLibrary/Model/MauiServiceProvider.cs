using System;

namespace MauiPhoneContactLibrary.Model;

public static class KKMauiServiceProvider
{
    public static TService? GetService<TService>()
        => Current.GetService<TService>();

    public static IServiceProvider Current
        =>
           IPlatformApplication.Current?.Services ?? throw new InvalidOperationException("Service provider not available");
    // #if ANDROID

    //               IPlatformApplication.Current.Services; 

    // #elif IOS || MACCATALYST
    // 			MauiUIApplicationDelegate.Current.Services;
    // #else
    // 			null;
    //#endif
}
