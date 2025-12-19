using MauiPhoneContactLibrary.Helper;
using MauiPhoneContactLibrary.Interface;
using MauiPhoneContactLibrary.Service.Interface;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using KKPhone.ViewModel;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading;
using System.Security.Cryptography.X509Certificates;

namespace MauiPhoneContactLibrary.ViewModel;

public delegate void GetSingleSelectedContactItem(ContactItem contactItem);
public partial class KKGroupContactViewModel : BaseViewModel
{
  public GetSingleSelectedContactItem? getSingleSelectedContact;

  [ObservableProperty]
  private ObservableCollection<ContactGroup> contactGroups = new ObservableCollection<ContactGroup>();

  [ObservableProperty]
  private ObservableCollection<ContactItem> selectedContacts = new ObservableCollection<ContactItem>();

  [ObservableProperty]
  private string searchText = string.Empty;

  private CancellationTokenSource? _searchCts;
  private string _lastSearchQuery = string.Empty;
  private bool _isSearchActive = false;
  public event Action<string>? ScrollToLetterRequested;
  public IEnumerable<string> AlphabetList { get; private set; } = Enumerable.Empty<string>();
  public KKGroupContactViewModel(IKKGetContact kKReadDataFromLocalDB, IKKContactPermissionRequest kKContactPermissionRequest)
    : base(kKReadDataFromLocalDB, kKContactPermissionRequest)
  {
    AlphabetList = Enumerable.Range('A', 26).Select(i => ((char)i).ToString()).ToList();
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
      var contacts = await FetchContactsAsync(_currentPageSize);

      if (contacts != null && contacts.Any())
      {
        foreach (var contact in contacts)
        {
          KKContactGroupHelper.AddContactToGroupedCollection(ContactGroups, contact);
        }
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

  private async Task<List<ContactItem>> FetchContactsAsync(int pageSize)
  {
    return _isSearchActive && !string.IsNullOrEmpty(_lastSearchQuery)
      ? await _kKReadDataFromLocalDB.GetContactFromLocalDbWithSearch(_lastSearchQuery, pageSize)
      : await _kKReadDataFromLocalDB.GetContactFromLocalDbWithPagantion(pageSize);
  }
  public void UpdateSingleSelectedContact(ContactItem contact)
  {
    ContactItem previouslySelected = (ContactItem)contactGroups.SelectMany(g => g).FirstOrDefault(c => c.Itemselcted == true);
    if (previouslySelected != null)
    {
      var previouslySelectedIndex = previouslySelected != null ? contactGroups.SelectMany(g => g).ToList().IndexOf(previouslySelected) : -1;
      var oldselecteditem = contactGroups.SelectMany(g => g).ToList()[previouslySelectedIndex];
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

  public void UpdateMultipleSelectedContacts(ContactItem currentselctedcontact)
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


  public List<ContactItem> GetSelectedContacts()
  {
    var allContacts = ContactGroups.SelectMany(g => g).ToList();
    return allContacts.Where(c => c.Itemselcted).ToList();
  }

  public void RestViewModel()
  {
    _searchCts?.Cancel();
    _searchCts?.Dispose();
    _searchCts = null;
    ContactGroups?.Clear();
    Singlecontactitem?.Clear();
    SelectedContacts?.Clear();
  }

  partial void OnSearchTextChanged(string value)
  {
    _ = PerformSearchWithDebounce(value);
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
  public async Task PerformSearch(string query, CancellationToken cancellationToken)
  {
    try
    {
      _currentPageSize = 0;
      _isSearchActive = !string.IsNullOrEmpty(query);

      var contacts = await FetchContactsAsync(_currentPageSize);

      if (cancellationToken.IsCancellationRequested)
        return;

      ContactGroups.Clear();

      if (contacts != null && contacts.Any())
      {
        AddContactsToGroups(contacts);
      }
    }
    catch (Exception ex)
    {
      if (ex is not TaskCanceledException)
        Debug.WriteLine($"Error in PerformSearch: {ex.Message}");
    }
  }

  [RelayCommand]
  private void ScrollToLetter(string letter)
  {
    if (string.IsNullOrWhiteSpace(letter)) return;
    ScrollToLetterRequested?.Invoke(letter);

  }



}
