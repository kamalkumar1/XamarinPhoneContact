using System;
using System.Collections.ObjectModel;
using MauiPhoneContactLibrary.Helper;

namespace MauiPhoneContactLibrary.Service.Interface;

public interface IKKGetContact
{
  public Task<List<ContactItem>> GetAllContactFromLocalDb(int currentPageSize);
  public Task<List<ContactItem>> GetContactFromLocalDbWithPagantion(int currentPageSize);
  public Task<List<ContactItem>> GetContactFromLocalDbWithSearch(string query, int currentPageSize);
  public Task<int> TotalCountwithSearch(string query, int currentPageSize);
  public Task<int> TotalCount();
  public int TotalContactWithoutGrouping();


}
