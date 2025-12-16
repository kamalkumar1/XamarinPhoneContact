using Android.App;
using Android.Content.PM;
using Android.OS;

namespace MauiPhoneContactLibrary.Platforms.Android;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{

    // internal event EventHandler PermissionStatus;
    protected override void OnCreate(Bundle savedInstanceState)
    {

        base.OnCreate(savedInstanceState);
        //  ContactList.Init(this);


    }

}
