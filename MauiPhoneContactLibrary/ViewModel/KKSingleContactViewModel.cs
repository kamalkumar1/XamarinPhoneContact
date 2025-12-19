using System;
using System.ComponentModel;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using MauiPhoneContactLibrary.Helper;
using MauiPhoneContactLibrary.Interface;
using MauiPhoneContactLibrary.Service.Interface;
using MauiPhoneContactLibrary.Interface.LocalDB;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using MauiPhoneContactLibrary.Model;

namespace MauiPhoneContactLibrary.ViewModel;

public partial class KKSingleContactViewModel : ObservableObject
{
  public GetSingleSelectedContactItem? getSingleSelectedContact;
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
    try
    {
      _currentPageSize++;
      var contacts = await _kKReadDataFromLocalDB.GetAllContactFromLocalDb(_currentPageSize);
      AddContactToGroup(contacts);
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error in LoadContactsAsync: {ex.Message}");
      _currentPageSize = 0;

    }

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
    try
    {
      _searchCts?.Cancel();
      _searchCts?.Dispose();
      _searchCts = null;
      _kKReadDataFromLocalDB = null;
      _kKContactPermissionRequest = null;
      Singlecontactitem.Clear();
      _currentPageSize = -1;
      _totalPagecount = 0;
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error in RestViewModel: {ex.Message}");
    }

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
        _currentPageSize = 0;
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
  public void UpdateSingleSelectedContact(ContactItem contact)
  {
    try
    {
      ContactItem previouslySelected = Singlecontactitem.FirstOrDefault(c => c.Itemselcted == true);
      if (previouslySelected != null)
      {
        var previouslySelectedIndex = previouslySelected != null ? Singlecontactitem.ToList().IndexOf(previouslySelected) : -1;
        var oldselecteditem = Singlecontactitem.ToList()[previouslySelectedIndex];
        if (oldselecteditem != null)
        {
          oldselecteditem.Itemselcted = false;
          if (oldselecteditem.Id == contact.Id)
          {
            contact.Itemselcted = false;
            return;
          }
          contact.Itemselcted = true;
          getSingleSelectedContact?.Invoke(contact);
        }
      }
      else
      {
        contact.Itemselcted = true;
        getSingleSelectedContact?.Invoke(contact);
      }

    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error in UpdateSingleSelectedContact: {ex.Message}");
    }


  }

  public void UpdateMultipleSelectedContacts(ContactItem currentselctedcontact)
  {
    try
    {
      currentselctedcontact.Itemselcted = !currentselctedcontact.Itemselcted;
      if (currentselctedcontact.Itemselcted)
      {
        if (!SelectedContacts.Contains(currentselctedcontact))
          SelectedContacts.Add(currentselctedcontact);
      }
      else
      {
        SelectedContacts.Remove(currentselctedcontact);
      }
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error in UpdateMultipleSelectedContacts: {ex.Message}");
    }
  }

  public List<ContactItem> GetSelectedContacts()
  {
    return Singlecontactitem.Where(c => c.Itemselcted).ToList();
  }
}
