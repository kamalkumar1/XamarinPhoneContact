using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms;
using System.Collections;

namespace XamarinPhoneContact.Droid
{
    [Activity(Label = "XamarinPhoneContact", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        internal static MainActivity Instance { get; private set; }
        public ICallBackInterface callBackInterface;
       // internal event EventHandler PermissionStatus;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Instance = this;
            ContactList.Init(this);
           // GlobalApplication.getAppContext();
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (ContactConfig.Instance.ContactPermission == requestCode)
            {
                PhoneContactPermissionsResults.Instance.RequestPermissionsResults(requestCode, permissions, grantResults);
            }
           // MessagingCenter.Send(this, _tickContract, DateTime.Now);
           base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }

}