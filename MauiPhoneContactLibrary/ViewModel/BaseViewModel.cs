using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using MauiPhoneContactLibrary.Helper;
using MauiPhoneContactLibrary.Interface;
using MauiPhoneContactLibrary.Service.Interface;

namespace KKPhone.ViewModel;

public partial class BaseViewModel : ObservableObject
{
  public IKKGetContact _kKReadDataFromLocalDB;
  public IKKContactPermissionRequest _kKContactPermissionRequest;
  public int _currentPageSize = -1;
  public int _totalPagecount;
  public bool _isLoadMoreInProgress = false;


  [ObservableProperty]
  private ObservableCollection<ContactItem> singlecontactitem = new();

  [ObservableProperty]
  bool isLoadingMore = false;

  [ObservableProperty]
  ContactItem? selectedContactItem;

  [ObservableProperty]
  string searchText = string.Empty;

  public bool isGrouping = false;
  public BaseViewModel(IKKGetContact kKReadDataFromLocalDB, IKKContactPermissionRequest kKContactPermissionRequest)
  {
    _kKReadDataFromLocalDB = kKReadDataFromLocalDB;
    _kKContactPermissionRequest = kKContactPermissionRequest;
    IsLoadingMore = false;
    Task.Run(CalulateAndGetTotalPageCount);
  }
  public async Task CalulateAndGetTotalPageCount()
  {
    var totalItems = await _kKReadDataFromLocalDB.TotalCount();
    _totalPagecount = (totalItems + ContactConfig.Instance.PageSize - 1) / ContactConfig.Instance.PageSize;
  }
  public async Task<List<ContactItem>?> LoadContactsAsync()
  {
    var permissionStatus = await _kKContactPermissionRequest.GetContactAuthorizationStatus();
    if (permissionStatus != true)
    {
      Debug.WriteLine("Contact permission not granted.");
      return null;
    }
    _currentPageSize++;
    var contacts = await _kKReadDataFromLocalDB.GetAllContactFromLocalDb(_currentPageSize);
    if (contacts == null || contacts.Count == 0)
    {
      return null;
    }
    return contacts;

  }


}
