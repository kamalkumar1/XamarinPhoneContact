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
    public class ContactList : AppCompatActivity
    {

        public IKKCurdOperation _kKCurdOperation;

        public event EventHandler? CustomPermissionStatus;
        List<ContactGroup> totalContactList = new List<ContactGroup>(1000);

        public ContactList(IKKCurdOperation kKCurdOperation)
        {
            _kKCurdOperation = kKCurdOperation;
        }

        public void MoveToSetting()
        {
            //throw new NotImplementedException();
        }


    }
}
