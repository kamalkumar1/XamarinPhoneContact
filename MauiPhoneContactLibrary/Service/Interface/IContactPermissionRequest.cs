using System;
using MauiPhoneContactLibrary.Helper;

namespace MauiPhoneContactLibrary.Interface;


public interface IKKContactPermissionRequest
{
   public Task<bool> GetContactAuthorizationStatus();
   //void RequestPermissions();
   //public event EventHandler? CustomPermissionStatus;

}
