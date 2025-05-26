using Android.App;
using Android.Content.PM;
using Android.OS;

namespace MauiPhoneContactLibrary.Platforms.Android;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    internal static MainActivity Instance { get; private set; }
        public ICallBackInterface callBackInterface;
       // internal event EventHandler PermissionStatus;
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            Instance = this;
            ContactList.Init(this);

        }
    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        if(callBackInterface!=null)
         callBackInterface.RequestPermissionsResults(requestCode, permissions, grantResults);
    }


}
