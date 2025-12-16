using System;
using MauiPhoneContactLibrary.Helper;

namespace MauiPhoneContactLibrary.Interface;

public interface IKKPhoneContactData
{
  // Task<List<ContactGroup>> GetAllContactFromLocalDb();
  Task<bool> CheckLocalDbFirstTimeSyncStatusAsync();
  Task<KKContactResulType> GetAllContactFromPhoneAndStoreToLocalDbAsync();
  void DisposeAllData();
}
