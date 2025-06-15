using UIKit;
using MauiPhoneContactLibrary.iOS;
using Contacts;
using Foundation;
using System.Diagnostics;
using MauiPhoneContactLibrary.Helper;

namespace MauiPhoneContactLibrary.Platforms
{
    public class ContactList : IContact
    {
    public event EventHandler? CustomPermissionStatus;

       public ContactList ()
       {

       }
       private void MoveToSetting()
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

        public ContactEnum CheckPermissions()
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

        public Dictionary<string,object> GetAllContact()
        {
            PhoneContactData contactdata = new PhoneContactData();
            var totalContactListItem = contactdata.GetAllContactFromPhone();
            return totalContactListItem;
        }

        public void CheckPermission()
        {
            CNAuthorizationStatus authStatus = CNContactStore.GetAuthorizationStatus(CNEntityType.Contacts);
            if (authStatus == CNAuthorizationStatus.Denied || authStatus == CNAuthorizationStatus.Restricted)
            {
                Debug.WriteLine("Contacts Denied or Restricted");
                var okCancelAlertController = UIAlertController.Create("Alert", "Need permission to access contac", UIAlertControllerStyle.Alert);

                //Add Actions
                okCancelAlertController.AddAction(UIAlertAction.Create("Setting", UIAlertActionStyle.Default, (UIAlertAction obj) =>
                {
                    MoveToSetting();
                }));
                okCancelAlertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel,null));
              
                //Present Alert
               UIApplication.SharedApplication.KeyWindow.RootViewController. PresentViewController(okCancelAlertController, true, null);

            }
            else if(authStatus == CNAuthorizationStatus.NotDetermined)
            {
                     var store = new CNContactStore();
                    store.RequestAccess(CNEntityType.Contacts, (granted, error) =>
                    {
                        if (!granted)
                        {
                            var okCancelAlertController = UIAlertController.Create("Alert ", "Need permission to access contact", UIAlertControllerStyle.Alert);
                            //Add Actions
                            okCancelAlertController.AddAction(UIAlertAction.Create("Setting", UIAlertActionStyle.Default, (UIAlertAction obj) =>
                            {
                                MoveToSetting();
                            }));
                            okCancelAlertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                        }
                        else
                        {
                            //check = ContactEnum.Granted;
                               UIApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                 CustomPermissionStatus?.Invoke(ContactEnum.Granted, EventArgs.Empty);
                
            });
                        }
                    });
                }
                
            else
            {
                UIApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                 CustomPermissionStatus?.Invoke(ContactEnum.Granted, EventArgs.Empty);
                
            });
               

            }
        }
    }
}
