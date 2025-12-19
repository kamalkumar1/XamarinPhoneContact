using System;
using XamarinPhoneContact.Helper;

namespace XamarinPhoneContact.Interface;

public interface IKKContactPermissionRequest
{
   /// <summary>
   /// This method request contact authorization status. IF permission granted it will return true else false  and request permission and take user to the settings page
   /// </summary>
   /// <returns></returns>
   public Task<bool> GetContactAuthorizationStatus();
}
