using System;
using XamarinPhoneContact.Helper;

namespace XamarinPhoneContact.Interface;


public interface IKKContactPermissionRequest
{
   public Task<bool> GetContactAuthorizationStatus();
   void RequestPermissions();
   public event EventHandler? CustomPermissionStatus;

}
