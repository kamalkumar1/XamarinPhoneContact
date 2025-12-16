using Android;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.Content;
using MauiPhoneContactLibrary.Helper;
using MauiPhoneContactLibrary.Interface;

namespace MauiPhoneContactLibrary.Platforms.Android;

public class ContactPermissionRequest : IKKContactPermissionRequest
{
  public event EventHandler? CustomPermissionStatus;

  public async Task<bool> GetContactAuthorizationStatus()
  {
    var check = GetcontactPermission();
    if (check)
    {
      return true;
    }
    else
    {
      return await SetContactPermission();
    }
  }

  public async void RequestPermissions()
  {
    var check = GetcontactPermission();
    if (!check)
    {

    }
    else
    {
      CustomPermissionStatus?.Invoke(ContactEnum.Granted, EventArgs.Empty);
    }
  }


  public async Task<bool> SetContactPermission()
  {
    // var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity as Activity ?? throw new InvalidOperationException("No activity");
    // var obj = ActivityCompat.RequestPermissions(activity, new[] { Manifest.Permission.ReadContacts }, 1107);
    var status = await MainThread.InvokeOnMainThreadAsync(async () =>
      await Permissions.RequestAsync<Permissions.ContactsRead>());
    if (status == PermissionStatus.Granted)
    {
      return true;
    }
    else
    {
      return false;
    }

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
}
