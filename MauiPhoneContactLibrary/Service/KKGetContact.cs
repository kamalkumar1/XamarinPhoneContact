using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using MauiPhoneContactLibrary.Helper;
using MauiPhoneContactLibrary.Interface.LocalDB;
using MauiPhoneContactLibrary.Model;
using MauiPhoneContactLibrary.Service.Interface;
using MauiPhoneContactLibrary.View;

namespace MauiPhoneContactLibrary.Service;

public class KKGetContact : IKKGetContact
{
  public readonly IKKCurdOperation _kKCurdOperation;
  int _totalCountWithoutGrouping;
  public KKGetContact(IKKCurdOperation kKCurdOperation)
  {
    _kKCurdOperation = kKCurdOperation;
  }
  ContactItem GroupContact(KKSqlTableForContact item)
  {
    ContactItem contactItem = new ContactItem();
    try
    {
      Debug.WriteLine("Grouping in progress...");

      contactItem.Id = item.Id;
      contactItem.ContactID = item.ContactID;
      contactItem.Birthday = item.Birthday;
      contactItem.DisplayName = item.DisplayName;
      contactItem.GetNames = JsonSerializer.Deserialize<Name>(item.NameList ?? "");
      if (!string.IsNullOrEmpty(item.Emaillist))
        contactItem.GetEmails = JsonSerializer.Deserialize<List<Emailids>>(item.Emaillist ?? "");
      if (!string.IsNullOrEmpty(item.Urlslist))
        contactItem.GetUrls = JsonSerializer.Deserialize<List<Url>>(item.Urlslist ?? "");
      if (!string.IsNullOrEmpty(item.Phoneslist))
        contactItem.GetPhones = JsonSerializer.Deserialize<List<Phone>>(item.Phoneslist ?? "");
      if (!string.IsNullOrEmpty(item.Companylist))
        contactItem.GetCompany = JsonSerializer.Deserialize<Company>(item.Companylist ?? "");
      if (!string.IsNullOrEmpty(item.Addresslist))
        contactItem.GetAddress = JsonSerializer.Deserialize<List<Address>>(item.Addresslist ?? "");
      if (!string.IsNullOrEmpty(item.Datelist))
        contactItem.GetDateList = JsonSerializer.Deserialize<List<DateList>>(item.Datelist ?? "");

      return contactItem;
    }
    catch (Exception ex)
    {
      Debug.WriteLine(ex);
      return contactItem;

    }

  }

  async Task<List<ContactItem>> GroupDataWithList(List<KKSqlTableForContact> kKSqlTableForContactsList)
  {
    try
    {
      Debug.WriteLine("Grouping started...");
      var listitem = new List<ContactItem>();
      foreach (var item in kKSqlTableForContactsList)
      {
        var contact = GroupContact(item);
        listitem.Add(contact);
      }
      Debug.WriteLine("Grouping started...");
      return listitem;
    }
    catch (Exception ex)
    {
      Debug.WriteLine("Error in GroupDataWithList: " + ex.Message);
      return null;
    }


  }
  /// <summary>
  /// Gets all contact from phone.
  /// </summary>
  /// <returns>The all contact from phone.</returns>
  public async Task<List<ContactItem>> GetAllContactFromLocalDb(int currentPageSize)
  {
    try
    {
      Debug.WriteLine("Reading control db started...");
      var listitem = await _kKCurdOperation.ReadContactData(currentPageSize);
      if (listitem != null && listitem.Count > 0)
      {
        var result = await GroupDataWithList(listitem);
        return result;
      }
      Debug.WriteLine("Reading control db Completed...");
      return null;
    }
    catch (Exception ex)
    {
      Debug.WriteLine("Error in GetAllContactFromLocalDb: " + ex.Message);
      return null;
    }
  }
  public async Task<List<ContactItem>> GetContactFromLocalDbWithPagantion(int currentPageSize)
  {
    try
    {

      Debug.WriteLine("Reading control db started...");
      var listitem = await _kKCurdOperation.ReadContactData(currentPageSize);
      if (listitem != null && listitem.Count > 0)
      {
        _totalCountWithoutGrouping += listitem.Count;
        var result = await GroupDataWithList(listitem);
        return result;
      }
      Debug.WriteLine("Reading control db Completed...");
      return null;
    }
    catch (Exception ex)
    {
      Debug.WriteLine("Error in GetAllContactFromLocalDb: " + ex.Message);
      return null;
    }
  }
  public async Task<List<ContactItem>> GetContactFromLocalDbWithSearch(string query, int currentPageSize)
  {
    try
    {
      Debug.WriteLine("Reading control db started...");
      var listitem = await _kKCurdOperation.SearchAndReadContactData(query, currentPageSize);
      if (listitem != null && listitem.Count > 0)
      {
        var result = await GroupDataWithList(listitem);
        return result;
      }
      Debug.WriteLine("Reading control db Completed...");
      return null;
    }
    catch (Exception ex)
    {
      Debug.WriteLine("Error in GetAllContactFromLocalDb: " + ex.Message);
      return null;
    }
  }

  public Task<int> TotalCountwithSearch(string query, int currentPageSize)
  {
    throw new NotImplementedException();
  }

  public async Task<int> TotalCount()
  {
    return await _kKCurdOperation.TotalCount();
  }

  public int TotalContactWithoutGrouping()
  {
    return _totalCountWithoutGrouping;
  }
}
