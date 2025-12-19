using UIKit;
using Contacts;
using Foundation;
using System.Diagnostics;
using XamarinPhoneContact.Helper;
using XamarinPhoneContact.Interface;

namespace XamarinPhoneContact.Platforms.iOS
{
    public class ContactPermissionRequest : IKKContactPermissionRequest
    {

        private UIViewController? GetRootViewController()
        {
            var window = UIApplication.SharedApplication.ConnectedScenes
                .OfType<UIWindowScene>()
                .SelectMany(s => s.Windows)
                .FirstOrDefault(w => w.IsKeyWindow);
            return window?.RootViewController;
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
        public async Task<bool> GetContactAuthorizationStatus()
        {
            try
            {
                CNAuthorizationStatus authStatus = CNContactStore.GetAuthorizationStatus(CNEntityType.Contacts);

                if (authStatus == CNAuthorizationStatus.Authorized)
                {

                    return true;
                }

                if (authStatus == CNAuthorizationStatus.Denied || authStatus == CNAuthorizationStatus.Restricted)
                {
                    Debug.WriteLine("Contacts Denied or Restricted");
                    await ShowPermissionAlertAsync("Alert", "Need permission to access contact");
                    return false;
                }

                if (authStatus == CNAuthorizationStatus.NotDetermined)
                {
                    var store = new CNContactStore();
                    var tcs = new TaskCompletionSource<bool>();

                    store.RequestAccess(CNEntityType.Contacts, (granted, error) =>
                    {
                        if (granted)
                        {
                            tcs.SetResult(true);
                        }
                        else
                        {
                            MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await ShowPermissionAlertAsync("Alert", "Need permission to access contact");
                            tcs.SetResult(false);
                        });
                        }
                    });

                    return await tcs.Task;
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetContactAuthorizationStatus: {ex.Message}");
                return false;
            }
        }

        private Task ShowPermissionAlertAsync(string title, string message)
        {
            var tcs = new TaskCompletionSource<bool>();

            var okCancelAlertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            okCancelAlertController.AddAction(UIAlertAction.Create("Setting", UIAlertActionStyle.Default, obj => MoveToSetting()));
            okCancelAlertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));

            GetRootViewController()?.PresentViewController(okCancelAlertController, true, () =>
            {
                tcs.SetResult(true);
            });

            return tcs.Task;
        }
    }
}
