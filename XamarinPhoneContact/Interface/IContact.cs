using System;
using System.Collections.Generic;
using XamarinPhoneContact.Helper;

namespace XamarinPhoneContact
{
    public interface IContact
    {
        Task<List<ContactGroup>> GetAllContactFromPhoneAsync();
        void CheckPermission();
        event EventHandler CustomPermissionStatus;
    }
}
