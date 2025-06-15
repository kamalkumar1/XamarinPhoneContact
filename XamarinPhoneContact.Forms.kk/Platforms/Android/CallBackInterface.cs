using System;
using Android.Content.PM;
using Android.Runtime;

namespace MauiPhoneContactLibrary.Platforms
{
    public interface ICallBackInterface
    {
        void RequestPermissionsResults(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults);
    }
    public sealed class PhoneContactPermissionsResults
    {
        public ICallBackInterface callBackInterface;
        static PhoneContactPermissionsResults()
        {
        }
        private PhoneContactPermissionsResults()
        {
        }
        public static PhoneContactPermissionsResults Instance { get; } = new PhoneContactPermissionsResults();
        public void RequestPermissionsResults(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if(callBackInterface!=null)
            callBackInterface.RequestPermissionsResults(requestCode, permissions, grantResults);
        }
    
    }
}
