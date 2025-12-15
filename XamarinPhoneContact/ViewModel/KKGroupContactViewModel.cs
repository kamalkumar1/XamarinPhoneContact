using XamarinPhoneContact.Helper;
using XamarinPhoneContact.Interface;
using XamarinPhoneContact.Service.Interface;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using KKPhone.ViewModel;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace XamarinPhoneContact.ViewModel;

public partial class KKGroupContactViewModel : BaseViewModel
{

  [ObservableProperty]
  private ObservableCollection<ContactGroup> contactGroups = new();
  public int TotalCount = 0;
  public KKGroupContactViewModel(IKKGetContact kKReadDataFromLocalDB, IKKContactPermissionRequest kKContactPermissionRequest)
    : base(kKReadDataFromLocalDB, kKContactPermissionRequest)
  {
  }

  public async Task LoadGroupContactsAsync()
  {
    Debug.WriteLine("LoadGroupContactsAsync started");
    var contacts = await LoadContactsAsync();
    Debug.WriteLine($"LoadContactsAsync returned {contacts?.Count ?? 0} contacts");
    if (contacts != null)
    {
      AddContactsToGroups(contacts);
    }
    else
    {
      Debug.WriteLine("No contacts returned from LoadContactsAsync");
    }
  }

  void AddContactsToGroups(List<ContactItem> contacts)
  {
    if (contacts == null || contacts.Count == 0)
    {
      Debug.WriteLine("No contacts to add to groups");
      return;
    }
    TotalCount = TotalCount + contacts.Count;
    var groupedContacts = KKContactGroupHelper.CreateGroupsWithSections(contacts);
    Debug.WriteLine($"groupedContacts has {groupedContacts.Count} groups");

    ContactGroups = groupedContacts;
    isGrouping = true;

    Debug.WriteLine($"ContactGroups property now has {ContactGroups.Count} groups with total {contacts.Count} contacts");
    foreach (var group in ContactGroups)
    {

      Debug.WriteLine($"  Group '{group.Title}': {group.Count} contacts");
    }
  }

  [RelayCommand]
  private async Task LoadMore()
  {
    Debug.WriteLine($"LoadMore called - IsLoadingMore: {IsLoadingMore}, InProgress: {_isLoadMoreInProgress}");

    if (_isLoadMoreInProgress || IsLoadingMore)
    {
      Debug.WriteLine("LoadMore already in progress, returning");
      return;
    }

    if (_currentPageSize >= _totalPagecount)
    {
      Debug.WriteLine($"No more data to load - CurrentPage: {_currentPageSize}, TotalPages: {_totalPagecount}");
      return;
    }

    Debug.WriteLine($"LoadMore executing - CurrentPage: {_currentPageSize}, TotalPages: {_totalPagecount}");
    _isLoadMoreInProgress = true;
    IsLoadingMore = true;

    try
    {
      _currentPageSize++;
      var contacts = await _kKReadDataFromLocalDB.GetContactFromLocalDbWithPagantion(_currentPageSize);
      if (contacts != null && contacts.Any())
      {
        TotalCount = TotalCount + contacts.Count;
        foreach (var contact in contacts)
        {

          KKContactGroupHelper.AddContactToGroupedCollection(ContactGroups, contact);
        }
      }
      //_currentPageSize++;
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
    //_kKContactPermissionRequest.CustomPermissionStatus -= OnPermissionStatusChanged;
    ContactGroups?.Clear();
    Singlecontactitem?.Clear();
  }

  [RelayCommand]
  void OnSearchTextChanged(string value)
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
