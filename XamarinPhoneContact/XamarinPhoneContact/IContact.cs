using System;
using System.Collections.Generic;

namespace XamarinPhoneContact
{
    public interface IContact
    {
        Dictionary<string, object> GetAllContact();
        void CheckPermission();
        event EventHandler CustomPermissionStatus;
    }
}
