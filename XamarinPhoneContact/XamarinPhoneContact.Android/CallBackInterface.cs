using System;
using Android.Content.PM;
using Android.Runtime;

namespace XamarinPhoneContact.Droid
{
    public interface ICallBackInterface
    {
        void RequestPermissionsResults(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults);
    }
}
