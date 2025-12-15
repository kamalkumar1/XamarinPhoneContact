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
  private CancellationTokenSource? _searchCts;
  private string _lastSearchQuery = string.Empty;

  [ObservableProperty]
  private ObservableCollection<ContactItem> singlecontactitem = new ObservableCollection<ContactItem>();

  [ObservableProperty]
  bool isLoadingMore = false;

  [ObservableProperty]
  ContactItem? selectedContactItem;

  [ObservableProperty]
  string searchText = string.Empty;

  [ObservableProperty]
  private ObservableCollection<ContactItem> selectedContacts = new ObservableCollection<ContactItem>();

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
    _searchCts?.Cancel();
    _searchCts?.Dispose();
    _searchCts = null;
    _kKReadDataFromLocalDB = null;
    _kKContactPermissionRequest = null;
    Singlecontactitem.Clear();
  }

  async partial void OnSearchTextChanged(string value)
  {
    await PerformSearchWithDebounce(value);
  }

  private async Task PerformSearchWithDebounce(string query)
  {
    // Cancel previous search
    _searchCts?.Cancel();
    _searchCts = new CancellationTokenSource();
    var token = _searchCts.Token;

    try
    {
      // Debounce: wait 300ms for user to stop typing
      await Task.Delay(300, token);

      // Avoid duplicate searches
      if (_lastSearchQuery == query)
        return;

      _lastSearchQuery = query;
      await PerformSearch(query, token);
    }
    catch (TaskCanceledException)
    {
      // Search was cancelled by new input, ignore
    }
  }

  [RelayCommand]
  async Task Search()
  {
    _searchCts?.Cancel();
    await PerformSearch(SearchText, CancellationToken.None);
  }

  async Task PerformSearch(string query, CancellationToken cancellationToken)
  {
    try
    {
      if (string.IsNullOrEmpty(query))
      {
        _currentPageSize = 0;
        var contacts = await _kKReadDataFromLocalDB.GetAllContactFromLocalDb(_currentPageSize);

        if (cancellationToken.IsCancellationRequested)
          return;

        Singlecontactitem.Clear();
        AddContactToGroup(contacts);
      }
      else
      {
        var contacts = await _kKReadDataFromLocalDB.GetContactFromLocalDbWithSearch(query, _currentPageSize);

        if (cancellationToken.IsCancellationRequested)
          return;


        Singlecontactitem.Clear();
        if (contacts != null && contacts.Any())
        {
          AddContactToGroup(contacts);
        }
      }
    }
    catch (Exception ex)
    {
      if (ex is not TaskCanceledException)
        Debug.WriteLine($"Error in PerformSearch: {ex.Message}");
    }
  }

  public void UpdateSelectedContact(ContactItem contact)
  {
    if (contact.Itemselcted)
    {
      if (!SelectedContacts.Contains(contact))
        SelectedContacts.Add(contact);
    }
    else
    {
      SelectedContacts.Remove(contact);
    }
  }

  public List<ContactItem> GetSelectedContacts()
  {
    return Singlecontactitem.Where(c => c.Itemselcted).ToList();
  }
}
