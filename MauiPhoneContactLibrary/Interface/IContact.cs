using System;
using System.Collections.Generic;
using MauiPhoneContactLibrary.Helper;

namespace MauiPhoneContactLibrary
{
  
    public interface IContact
    {
        Task<List<ContactGroup>> GetAllContactFromPhoneAsync();
        void CheckPermission();
        event EventHandler CustomPermissionStatus;
    }
}
