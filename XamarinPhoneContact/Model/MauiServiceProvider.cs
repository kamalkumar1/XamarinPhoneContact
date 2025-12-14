using System;

namespace XamarinPhoneContact.Model;

public static class MauiServiceProvider
    {
        public static TService GetService<TService>()
            => Current.GetService<TService>();

        public static IServiceProvider Current
            =>
               IPlatformApplication.Current.Services; 
// #if ANDROID

//               IPlatformApplication.Current.Services; 
             
// #elif IOS || MACCATALYST
// 			MauiUIApplicationDelegate.Current.Services;
// #else
// 			null;
//#endif
    }
