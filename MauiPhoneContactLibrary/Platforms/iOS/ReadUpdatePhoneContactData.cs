using Foundation;
using Contacts;
using XamarinPhoneContact.Helper;
using System.Diagnostics;
using XamarinPhoneContact.Interface.LocalDB;
using XamarinPhoneContact.Model;
using XamarinPhoneContact.Interface;
using XamarinPhoneContact.Model.SecureKeyGenrator;
using ObjCRuntime;

namespace XamarinPhoneContact.Platforms.iOS
{
  public class ReadUpdatePhoneContactData : IReadUpdatePhoneContactData
  {
    CNContactStore? _store;
    CNContactHelper _cNContactHelper;
    IKKContactControlDbOperation _kKContactControlDbOperation;
    IKKCurdOperation? _kKCurdOperation;

    // Store the history token for incremental sync
    private NSData? _lastHistoryToken;

    public ReadUpdatePhoneContactData(IKKContactControlDbOperation kKContactControlDbOperation, IKKCurdOperation kKCurdOperation)
    {
      _kKContactControlDbOperation = kKContactControlDbOperation;
      _kKCurdOperation = kKCurdOperation;
    }

    /// <summary>
    /// Fetch only contacts that changed since last sync using CNChangeHistoryFetchRequest
    /// </summary>
    public async Task<KKContactResulType> SyncContactChangesAsync()
    {
      try
      {
        if (_store == null)
          _store = new CNContactStore();

        // Create change history fetch request
        var changeRequest = new CNChangeHistoryFetchRequest();

        _lastHistoryToken = _cNContactHelper.LoadHistoryToken();
        // If we have a previous token, use it to fetch only changes
        if (_lastHistoryToken != null)
        {
          changeRequest.StartingToken = _lastHistoryToken;
        }
        var descriptors = new List<ICNKeyDescriptor>
        {
          CNContactFormatter.GetDescriptorForRequiredKeys(CNContactFormatterStyle.FullName),
          CNContactFormatter.GetDescriptorForRequiredKeys(CNContactFormatterStyle.PhoneticFullName),
        };
        foreach (var key in _cNContactHelper.GetCNcontactKey())
        {
          var descriptor = Runtime.GetINativeObject<ICNKeyDescriptor>(key.Handle, false);
          if (descriptor != null)
          {
            descriptors.Add(descriptor);
          }
        }
        // Specify which contact properties you need
        changeRequest.AdditionalContactKeyDescriptors = descriptors.ToArray();
        NSError error;
        var result = _store.GetEnumeratorForChangeHistory(changeRequest, out error);

        if (error != null)
        {
          Debug.WriteLine($"Error fetching contact changes: {error.LocalizedDescription}");
          return KKContactResulType.UknownFailure;
        }

        if (result == null)
        {
          Debug.WriteLine("No change history result returned");
          return KKContactResulType.NoChangesFoundToUpdate;
        }

        var addedContacts = new List<CNContact>();
        var updatedContacts = new List<CNContact>();
        var deletedContactIds = new List<string>();

        // Get the enumerator from the fetch result
        var enumerator = result.Value as NSEnumerator;
        if (enumerator != null)
        {
          // Enumerate through all changes
          CNChangeHistoryEvent change;
          while ((change = enumerator.NextObject() as CNChangeHistoryEvent) != null)
          {
            if (change is CNChangeHistoryAddContactEvent addEvent)
            {
              Debug.WriteLine($"Contact Added: {addEvent.Contact.Identifier}");
              addedContacts.Add(addEvent.Contact);
            }
            else if (change is CNChangeHistoryUpdateContactEvent updateEvent)
            {
              Debug.WriteLine($"Contact Updated: {updateEvent.Contact.Identifier}");
              updatedContacts.Add(updateEvent.Contact);
            }
            else if (change is CNChangeHistoryDeleteContactEvent deleteEvent)
            {
              Debug.WriteLine($"Contact Deleted: {deleteEvent.ContactIdentifier}");
              deletedContactIds.Add(deleteEvent.ContactIdentifier);
            }
            else if (change is CNChangeHistoryDropEverythingEvent)
            {
              Debug.WriteLine("Everything dropped - need full resync");
              // Return false to indicate full resync is needed
              ResetSyncHistory();
              return KKContactResulType.SyncTokenFailure;
            }
          }
        }

        // Save the latest token for next sync
        _lastHistoryToken = result.CurrentHistoryToken;
        _cNContactHelper.SaveHistoryToken(_lastHistoryToken);

        // Process changes
        var resultProcess = await ProcessContactChanges(addedContacts, updatedContacts, deletedContactIds);
        if (!resultProcess)
        {
          return KKContactResulType.UknownFailure;
        }
        Debug.WriteLine($"Sync completed: {addedContacts.Count} added, {updatedContacts.Count} updated, {deletedContactIds.Count} deleted");
        return KKContactResulType.UpdateAsyncCompleted;
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"Error in SyncContactChangesAsync: {ex.Message}");
        return KKContactResulType.UknownFailure;
      }
    }

    private async Task<bool> ProcessContactChanges(
        List<CNContact> addedContacts,
        List<CNContact> updatedContacts,
        List<string> deletedContactIds)
    {
      try
      {
        if (_kKCurdOperation == null)
        {
          Debug.WriteLine("KKCurdOperation is null, cannot process contact changes");
          return false;
        }

        // Process added contacts
        if (addedContacts.Any())
        {
          var addedTasks = addedContacts.Select(c => _cNContactHelper.ProcessSingleContact(c));
          var processedAdded = await Task.WhenAll(addedTasks);
          await _kKCurdOperation.InsertContactData(processedAdded.ToList());
          Debug.WriteLine($"Inserted {processedAdded.Length} new contacts");
        }

        // Process updated contacts
        if (updatedContacts.Any())
        {
          var updateTasks = updatedContacts.Select(c => _cNContactHelper.ProcessSingleContact(c));
          var processedUpdated = await Task.WhenAll(updateTasks);
          await _kKCurdOperation.UpsertContactDataBulk(processedUpdated.ToList());
          Debug.WriteLine($"Updated {processedUpdated.Length} contacts");
        }

        // Process deleted contacts
        if (deletedContactIds.Any())
        {
          await DeleteContactsByIds(deletedContactIds);
          Debug.WriteLine($"Deleted {deletedContactIds.Count} contacts");
        }
        return true;
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"Error processing contact changes: {ex.Message}");
        return false;
      }
    }


    private async Task DeleteContactsByIds(List<string> contactIds)
    {
      try
      {
        var conn = _kKContactControlDbOperation.GetSQLiteAsyncConnection();
        foreach (var contactId in contactIds)
        {
          await conn.ExecuteAsync(
              "DELETE FROM KKSqlTableForContact WHERE ContactID = ?",
              contactId);
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"Error deleting contacts: {ex.Message}");
      }
    }

    /// <summary>
    /// Reset sync - clears the history token to force full sync next time
    /// </summary>
    public void ResetSyncHistory()
    {
      _lastHistoryToken = null;
      NSUserDefaults.StandardUserDefaults.RemoveObject("ContactHistoryToken");
      NSUserDefaults.StandardUserDefaults.Synchronize();
    }

    /// <summary>
    /// Check if we have a saved history token
    /// </summary>
    public bool HasHistoryToken()
    {
      return _lastHistoryToken != null;
    }

    public void Dispose()
    {
      _store?.Dispose();
      _store = null;
    }
  }
}
