using System;
using System.Collections.Generic;

namespace MauiPhoneContactLibrary
{
    public interface IContact
    {
        Dictionary<string, object> GetAllContact();
        void CheckPermission();
        event EventHandler CustomPermissionStatus;
    }
}
