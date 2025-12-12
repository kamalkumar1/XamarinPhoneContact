using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Database;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using AndroidX.AppCompat.App;
using XamarinPhoneContact.Helper;
using XamarinPhoneContact.Interface;
using XamarinPhoneContact.Interface.LocalDB;

namespace XamarinPhoneContact.Platforms.Android
{
    public class ContactList : AppCompatActivity, IContact, ICallBackInterface
    {

        public IKKCurdOperation _kKCurdOperation;
        private static Activity? m_activity;
        public event EventHandler? CustomPermissionStatus;
        readonly string[] permissionscontact = { Manifest.Permission.ReadContacts };
        static int RequestPermissionCode;
        List<ContactGroup> totalContactList = new List<ContactGroup>(1000);

        public static void Init(Activity activity)
        {
            m_activity = activity;
        }
        public ContactList(IKKCurdOperation kKCurdOperation)
        {
            _kKCurdOperation = kKCurdOperation;
        }

        public void CheckPermission()
        {
            var check = GetcontactPermission();
            if (!check)
            {
                CustomPermissionStatus?.Invoke(ContactEnum.Denied, EventArgs.Empty);
                SetContactPermission();

            }
            else
            {
                CustomPermissionStatus?.Invoke(ContactEnum.Granted, EventArgs.Empty);
            }
        }
        public void SetContactPermission()
        {
            MainActivity.Instance.callBackInterface = this;

            ActivityCompat.RequestPermissions(m_activity, new string[] { Manifest.Permission.ReadContacts }, 1107);
        }
        private bool GetcontactPermission()
        {
            if ((int)Build.VERSION.SdkInt < 23)
            {
                return true;
            }
            var globalVariable = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
            var permissionCheck = ContextCompat.CheckSelfPermission(globalVariable, Manifest.Permission.ReadContacts);
            return permissionCheck == Permission.Granted;
        }


        public void MoveToSetting()
        {
            //throw new NotImplementedException();
        }

        public void RequestPermissionsResults(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch (requestCode)
            {

                case 1107:
                    {
                        if (grantResults[0] == Permission.Granted)
                        {
                            CustomPermissionStatus?.Invoke(ContactEnum.Granted, EventArgs.Empty);

                        }
                        else
                        {
                            CustomPermissionStatus?.Invoke(ContactEnum.Denied, EventArgs.Empty);

                        }
                    }
                    break;
            }
        }

        public void CheckPermission(object currentPage)
        {
            //throw new NotImplementedException();
        }

        public Task<List<ContactGroup>> GetAllContactFromPhoneAsync()
        {
            throw new NotImplementedException();
        }
    }
}
