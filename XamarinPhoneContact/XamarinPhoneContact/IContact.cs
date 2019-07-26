using System;
namespace XamarinPhoneContact
{
    public interface IContact
    {
        void GetAllContact();
        ContactEnum CheckPermission();
        void MoveToSetting();
    }
}
