using System;
using System.Collections.Generic;
using MauiPhoneContactLibrary.Helper;

namespace MauiPhoneContactLibrary
{
    public interface IKKContactPermission
    {
        void CheckPermission();
        event EventHandler CustomPermissionStatus;
    }
    public interface IContact
    {
        Task<List<ContactGroup>> GetAllContactFromPhoneAsync();
        void CheckPermission();
        event EventHandler CustomPermissionStatus;
    }
}
