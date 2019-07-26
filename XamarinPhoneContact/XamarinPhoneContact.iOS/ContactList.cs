using System;
using UIKit;
using Xamarin.Forms;
using XamarinPhoneContact.iOS;
using Contacts;
using Foundation;
using System.Diagnostics;
using AddressBook;
[assembly: Dependency(typeof(ContactList))]
namespace XamarinPhoneContact.iOS
{
    public class ContactList : IContact
    {
        public void GetAllContact()
        {

            PhoneContactData contactdata = new PhoneContactData();
           var totalContactListItem = contactdata.GetAllContactFromPhone();
        }
       public void MoveToSetting()
        {
            UIApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                // manipulate UI controls
                UIApplication.SharedApplication.OpenUrl(new NSUrl(UIApplication.OpenSettingsUrlString), new NSDictionary(), (obj) =>
                {
                    if (obj)
                    {
                        Console.WriteLine("MovedSucessTosetting");
                    }
                    else
                    {
                        Console.WriteLine("MovedSucessToAppStorefailed");
                    }
                });
            });
        }

        public ContactEnum CheckPermission()
        {
            CNAuthorizationStatus authStatus = CNContactStore.GetAuthorizationStatus(CNEntityType.Contacts);
            if (authStatus == CNAuthorizationStatus.Denied || authStatus == CNAuthorizationStatus.Restricted)
            {
                Debug.WriteLine("Contacts Denied or Restricted");
                return ContactEnum.Denied;
            }
           
            //if user adds contact for 1st time
            ContactEnum check = ContactEnum.PermissionRequired;
           
            if (authStatus == CNAuthorizationStatus.NotDetermined)
            {
                var store = new CNContactStore();
                store.RequestAccess(CNEntityType.Contacts, (granted, error) =>
                {
                    if (!granted)
                    {
                        //throwAlertMethod("Contacts Permission Required", "This app requires permission to access your contacts, " +
                        //    "please go to Settings>FollowItUp and re-enable permissions.");

                        check = ContactEnum.PermissionRequired;
                        return;
                    }
                    else
                    {
                        check = ContactEnum.Granted;
                    }
                });
            }
            else
            {
                check = ContactEnum.Granted;
            }
            return check;
        }

        public static void Init()
        {

        }
    }
}
