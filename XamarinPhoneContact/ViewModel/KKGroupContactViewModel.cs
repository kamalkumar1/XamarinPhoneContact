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

public partial class KKGroupContactViewModel : ObservableObject
{
  IKKGetContact _kKReadDataFromLocalDB;
  IKKContactPermissionRequest _kKContactPermissionRequest;
  int _currentPageSize = -1;
  int _totalPagecount;
  private bool _isLoadMoreInProgress = false;

  [ObservableProperty]
  private ObservableCollection<ContactGroup> contactGroups = new();
  [ObservableProperty]
  private ObservableCollection<ContactItem> singlecontactitem = new();

  [ObservableProperty]
  bool isLoadingMore = false;

  [ObservableProperty]
  ContactItem? selectedContactItem;

  [ObservableProperty]
  string searchText = string.Empty;

  bool isGrouping = false;

  public KKGroupContactViewModel(IKKGetContact kKReadDataFromLocalDB, IKKContactPermissionRequest kKContactPermissionRequest)
  {
    _kKReadDataFromLocalDB = kKReadDataFromLocalDB;
    _kKContactPermissionRequest = kKContactPermissionRequest;
    IsLoadingMore = false;
  }

  public void CheckPermission()
  {
    _kKContactPermissionRequest.CustomPermissionStatus -= OnPermissionStatusChanged;
    _kKContactPermissionRequest.CustomPermissionStatus += OnPermissionStatusChanged;
    _kKContactPermissionRequest.RequestPermissions();
  }

  async void OnPermissionStatusChanged(object sender, EventArgs eventArgs)
  {
    var result = (ContactEnum)sender;
    if (result == ContactEnum.Granted)
    {
      var stopwatch = Stopwatch.StartNew();
      await LoadContactsAsync();
      stopwatch.Stop();
      Debug.WriteLine($"LoadContactsAsync took: {stopwatch.ElapsedMilliseconds} ms");
    }
    else
    {
      Debug.WriteLine("Permission denied to load the contact. Check your setting in phone");
    }
  }

  public async Task CalulateAndGetTotalPageCount()
  {
    var totalItems = await _kKReadDataFromLocalDB.TotalCount();
    _totalPagecount = (totalItems + ContactConfig.Instance.PageSize - 1) / ContactConfig.Instance.PageSize;
  }

  public async Task LoadContactsAsync()
  {
    ContactGroups = KKContactGroupHelper.CreateDefaultGroups();
    _currentPageSize++;
    var contacts = await _kKReadDataFromLocalDB.GetAllContactFromLocalDb(_currentPageSize);
    if (!isGrouping)
    {
      foreach (var contact in contacts)
      {
        Singlecontactitem.Add(contact);
      }
    }
    else
    {
      if (contacts?.Any() == true)
      {
        AddContactToGroup(contacts);
        //_currentPageSize++;
        // await LoadePreLoadItem();
      }

    }

  }

  void AddContactToGroup(List<ContactItem> contacts)
  {
    foreach (var contact in contacts)
    {
      if (isGrouping == false)
      {
        Singlecontactitem.Add(contact);
      }
      else
      {
        int targetIndex = KKContactGroupHelper.GetGroupIndex(contact.DisplayName);
        ContactGroups[targetIndex].Add(contact);

      }

    }
  }


  [RelayCommand]
  private async Task LoadMore()
  {
    if (_isLoadMoreInProgress || IsLoadingMore)
      return;

    if (_currentPageSize >= _totalPagecount)
    {
      Debug.WriteLine("No more data to load");
      return;
    }

    _isLoadMoreInProgress = true;
    IsLoadingMore = true;

    try
    {
      var contacts = await _kKReadDataFromLocalDB.GetContactFromLocalDbWithPagantion(_currentPageSize);
      if (contacts != null && contacts.Any())
      {
        AddContactToGroup(contacts);
        _currentPageSize++;
        // await LoadePreLoadItem();
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
      _isLoadMoreInProgress = false;
    }
  }

  public void RestViewModel()
  {
    _kKContactPermissionRequest.CustomPermissionStatus -= OnPermissionStatusChanged;
    _kKReadDataFromLocalDB = null;
    _kKContactPermissionRequest = null;
    KKContactGroupHelper.CreateDefaultGroups().Clear();
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
