using System;
using XamarinPhoneContact.Helper;

namespace XamarinPhoneContact.Interface;

public interface IKKPhoneContactData
{
  // Task<List<ContactGroup>> GetAllContactFromLocalDb();
  Task<bool> CheckLocalDbFirstTimeSyncStatusAsync();
  Task<KKContactResulType> GetAllContactFromPhoneAndStoreToLocalDbAsync();
  void DisposeAllData();
}
