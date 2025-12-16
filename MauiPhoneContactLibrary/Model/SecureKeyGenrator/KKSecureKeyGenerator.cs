using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Maui.Storage;  // FileSystem

#if IOS
using UIKit;
#elif ANDROID
using Android.Provider;
using Android.Content;
#endif

namespace MauiPhoneContactLibrary.Model.SecureKeyGenrator;

public static class KKSecureKeyGenerator  // ✅ Static class
{

    public static KKSecureKey GenerateSecureKey()  // ✅ Static method
    {
        var deviceFingerprint = GetDeviceFingerprint();
        var password = Encoding.UTF8.GetBytes(deviceFingerprint);
        var salt = Encoding.UTF8.GetBytes(GetAppSalt());

        var keyBytes = Rfc2898DeriveBytes.Pbkdf2(
            password: password,
            salt: salt,
            iterations: 100_000,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: 32
        );

        return new KKSecureKey(keyBytes);
    }

    private static string GetDeviceFingerprint()
    {
#if IOS
        var id = UIKit.UIDevice.CurrentDevice.IdentifierForVendor;
        return id?.AsString() ?? "ios-fallback";  // MAUI/Xamarin helper
#elif ANDROID
              var androidId = Android.Provider.Settings.Secure.GetString(
                Platform.CurrentActivity.ContentResolver,  // ✅ Full path
                  Android.Provider.Settings.Secure.AndroidId
              );
              return androidId ?? "android-fallback";
#endif
    }

    private static string GetAppSalt()
    {
        using var rng = RandomNumberGenerator.Create();
        var entropy = new byte[32];
        rng.GetBytes(entropy);  // Hardware RNG

        var appPath = FileSystem.AppDataDirectory.GetHashCode();
        var deviceId = GetDeviceFingerprint().GetHashCode();

        var saltBytes = SHA256.HashData(
            entropy.Concat(BitConverter.GetBytes(appPath))
                  .Concat(BitConverter.GetBytes(deviceId))
                  .ToArray()
        );

        return Convert.ToBase64String(saltBytes);
    }

    private const string SECURE_KEY_PREF = "sqlite_secure_key";

    public static string GetOrCreateSecureKey()
    {
        // Try load existing
        if (Preferences.ContainsKey(SECURE_KEY_PREF))
        {
            var key = Preferences.Get(SECURE_KEY_PREF, "");
            Debug.WriteLine("🔑 Loaded key from Preferences");
            return key;
        }

        // Generate + save new
        var newKey = GenerateSecureKey().Value;
        Preferences.Set(SECURE_KEY_PREF, newKey);
        Debug.WriteLine("🔑 Generated + saved new key");

        return newKey;
    }

    // Reset (one-time cleanup)
    public static void ResetKey()
    {
        Preferences.Remove(SECURE_KEY_PREF);
        Debug.WriteLine("🗑️  Secure key reset");
    }

}
