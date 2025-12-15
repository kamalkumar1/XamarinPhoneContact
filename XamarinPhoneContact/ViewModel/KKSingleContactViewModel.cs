using System;
using System.ComponentModel;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using XamarinPhoneContact.Helper;
using XamarinPhoneContact.Interface;
using XamarinPhoneContact.Service.Interface;
using XamarinPhoneContact.Interface.LocalDB;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using XamarinPhoneContact.Model;

namespace XamarinPhoneContact.ViewModel;

public partial class KKSingleContactViewModel : ObservableObject
{
  IKKGetContact _kKReadDataFromLocalDB;
  IKKContactPermissionRequest _kKContactPermissionRequest;
  int _currentPageSize = -1;
  int _totalPagecount;

  private bool _isLoadMoreInProgress = false;
  [ObservableProperty]
  private ObservableCollection<ContactItem> singlecontactitem = new ObservableCollection<ContactItem>();

  [ObservableProperty]
  bool isLoadingMore = false;

  [ObservableProperty]
  ContactItem? selectedContactItem;

  [ObservableProperty]
  string searchText = string.Empty;

  public KKSingleContactViewModel(IKKGetContact kKReadDataFromLocalDB, IKKContactPermissionRequest kKContactPermissionRequest)
  {
    _kKReadDataFromLocalDB = kKReadDataFromLocalDB;
    _kKContactPermissionRequest = kKContactPermissionRequest;
    IsLoadingMore = false;
  }

  public async Task CalulateAndGetTotalPageCount()
  {
    var totalItems = await _kKReadDataFromLocalDB.TotalCount();
    _totalPagecount = (totalItems + ContactConfig.Instance.PageSize - 1) / ContactConfig.Instance.PageSize;
  }

  public async Task LoadContactsAsync()
  {
    var permissionStatus = await _kKContactPermissionRequest.GetContactAuthorizationStatus();
    if (permissionStatus != true)
    {
      Debug.WriteLine("Contact permission not granted.");
      return;
    }
    _currentPageSize++;
    var contacts = await _kKReadDataFromLocalDB.GetAllContactFromLocalDb(_currentPageSize);
    AddContactToGroup(contacts);
  }
  void AddContactToGroup(List<ContactItem> contacts)
  {
    foreach (var contact in contacts)
    {
      Singlecontactitem.Add(contact);
    }
  }


  [RelayCommand]
  private async Task LoadMore()
  {
    if (_isLoadMoreInProgress || IsLoadingMore)
      return;
    if (_currentPageSize >= _totalPagecount)
      return;
    _isLoadMoreInProgress = true;
    IsLoadingMore = true;
    try
    {
      _currentPageSize++;
      var contacts = await _kKReadDataFromLocalDB.GetContactFromLocalDbWithPagantion(_currentPageSize);
      if (contacts != null && contacts.Any())
      {
        AddContactToGroup(contacts);
        _isLoadMoreInProgress = false;

      }
      await Task.Delay(100);
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error in LoadMore: {ex.Message}");
    }
    finally
    {
      IsLoadingMore = false;

    }
  }

  public void RestViewModel()
  {
    _kKReadDataFromLocalDB = null;
    _kKContactPermissionRequest = null;
    Singlecontactitem.Clear();
  }

  partial void OnSearchTextChanged(string value)
  {
    PerformSearch(value);
  }

  [RelayCommand]
  void Search()
  {
    PerformSearch(SearchText);
  }

  void PerformSearch(string query)
  {
    if (string.IsNullOrEmpty(query))
    {
      // Show all contacts
    }
    else
    {
      // Filter contacts
    }
  }
}
