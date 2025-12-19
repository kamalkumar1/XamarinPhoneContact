using System;
using System.Diagnostics;
using Android.Content;
using Android.Database;
using Android.Provider;
using MauiPhoneContactLibrary.Helper;
using MauiPhoneContactLibrary.Interface;
using MauiPhoneContactLibrary.Interface.LocalDB;
using MauiPhoneContactLibrary.Model;
using MauiPhoneContactLibrary.Model.SecureKeyGenrator;

namespace MauiPhoneContactLibrary.Platforms.Android;

public class ReadUpdatePhoneContactData : IReadUpdatePhoneContactData
{
  CNContactHelper _cNContactHelper;
  IKKContactControlDbOperation _kKContactControlDbOperation;
  IKKCurdOperation? _kKCurdOperation;
  private long _lastSyncTimestamp;

  public ReadUpdatePhoneContactData(IKKContactControlDbOperation kKContactControlDbOperation, IKKCurdOperation kKCurdOperation)
  {
    _kKContactControlDbOperation = kKContactControlDbOperation;
    _kKCurdOperation = kKCurdOperation;
    _lastSyncTimestamp = _cNContactHelper.LoadSyncTimestamp();
  }

  /// <summary>
  /// Fetch only contacts that changed since last sync using CONTACT_LAST_UPDATED_TIMESTAMP
  /// </summary>
  public async Task<KKContactResulType> SyncContactChangesAsync()
  {
    try
    {
      var globalVariable = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
      if (globalVariable == null)
      {
        Debug.WriteLine("Current activity is null");
        return KKContactResulType.UknownFailure;
      }

      var contentResolver = globalVariable.ContentResolver;
      if (contentResolver == null)
      {
        Debug.WriteLine("ContentResolver is null");
        return KKContactResulType.UknownFailure;
      }

      var addedContacts = new List<KKSqlTableForContact>();
      var updatedContacts = new List<KKSqlTableForContact>();
      var deletedContactIds = new List<string>();

      // Get current timestamp before starting the sync
      var currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

      // Query for modified contacts since last sync
      string[] projection = {
        ContactsContract.Contacts.InterfaceConsts.Id,
        ContactsContract.Contacts.InterfaceConsts.DisplayName,
        ContactsContract.Contacts.InterfaceConsts.ContactLastUpdatedTimestamp
      };

      string? selection = null;
      string[]? selectionArgs = null;

      // If we have a previous timestamp, filter by it
      if (_lastSyncTimestamp > 0)
      {
        selection = ContactsContract.Contacts.InterfaceConsts.ContactLastUpdatedTimestamp + " > ?";
        selectionArgs = new string[] { _lastSyncTimestamp.ToString() };
        Debug.WriteLine($"Querying contacts modified after timestamp: {_lastSyncTimestamp}");
      }
      else
      {
        Debug.WriteLine("No previous sync timestamp - performing full sync");
      }

      ICursor? cursor = null;
      try
      {
        var contactsUri = ContactsContract.Contacts.ContentUri;
        if (contactsUri == null)
        {
          Debug.WriteLine("Contacts URI is null");
          return KKContactResulType.UknownFailure;
        }

        cursor = contentResolver.Query(
          contactsUri,
          projection,
          selection,
          selectionArgs,
          ContactsContract.Contacts.InterfaceConsts.ContactLastUpdatedTimestamp + " ASC");

        await Task.Run(() => RemovedDeletedContactsFromDb(contentResolver));

        if (cursor == null || cursor.Count == 0)
        {
          Debug.WriteLine("No changes found since last sync");
          return KKContactResulType.NoChangesFoundToUpdate;
        }

        Debug.WriteLine($"Found {cursor.Count} modified contacts");

        // Process changed contacts
        while (cursor.MoveToNext())
        {
          var contactId = cursor.GetString(cursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.Id));
          if (string.IsNullOrEmpty(contactId)) continue;

          var displayName = cursor.GetString(cursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.DisplayName)) ?? "";

          // Contact was added or updated
          // Check if contact exists in our database
          if (_kKCurdOperation == null) continue;
          var existsInDb = await _kKCurdOperation.CheckContactExistsInDb(contactId);

          var contact = _cNContactHelper.ProcessSingleContact(contactId, displayName, contentResolver);
          if (contact != null)
          {
            if (existsInDb)
            {
              Debug.WriteLine($"Contact Updated: {contactId}");
              updatedContacts.Add(contact);
            }
            else
            {
              Debug.WriteLine($"Contact Added: {contactId}");
              addedContacts.Add(contact);
            }
          }
        }
        // Process changes
        var resultProcess = await ProcessContactChanges(addedContacts, updatedContacts);
        if (!resultProcess)
        {
          return KKContactResulType.UknownFailure;
        }

        // Save the current timestamp for next sync
        _lastSyncTimestamp = _cNContactHelper.SaveSyncTimestamp(currentTimestamp);

        Debug.WriteLine($"Sync completed: {addedContacts.Count} added, {updatedContacts.Count} updated)");
        return KKContactResulType.UpdateAsyncCompleted;
      }
      finally
      {
        cursor?.Close();
      }
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error in SyncContactChangesAsync: {ex.Message}");
      Debug.WriteLine($"Stack trace: {ex.StackTrace}");
      return KKContactResulType.UknownFailure;
    }
  }
  async void RemovedDeletedContactsFromDb(ContentResolver? contentResolver)
  {
    ICursor? deletedCursor = null;
    try
    {
      if (contentResolver == null)
      {
        Debug.WriteLine("ContentResolver is null");
        return;
      }

      deletedCursor = null;
      var delteuri = ContactsContract.DeletedContacts.ContentUri;
      if (delteuri == null)
      {
        Debug.WriteLine("Deleted contacts URI is null");
        return;
      }

      string[] projection = {
                    ContactsContract.IDeletedContactsColumns.ContactId,
                    ContactsContract.IDeletedContactsColumns.ContactDeletedTimestamp
                };

      string selection =
          ContactsContract.IDeletedContactsColumns.ContactDeletedTimestamp + " > ?";

      string[] selectionArgs = { _lastSyncTimestamp.ToString() };

      deletedCursor = contentResolver.Query(
           delteuri,
           projection,
           selection,
           selectionArgs,
           ContactsContract.IDeletedContactsColumns.ContactDeletedTimestamp + " ASC");

      if (deletedCursor == null || deletedCursor.Count == 0)
      {
        Debug.WriteLine("No deleted changes found since last sync");
        return;
      }
      Debug.WriteLine($"Found {deletedCursor.Count} Deleted contacts");

      // Process changed contacts
      while (deletedCursor.MoveToNext())
      {
        var contactId = deletedCursor.GetString(deletedCursor.GetColumnIndex(ContactsContract.IDeletedContactsColumns.ContactId));
        if (string.IsNullOrEmpty(contactId)) continue;

        var contactDeletedTimestamp = deletedCursor.GetString(deletedCursor.GetColumnIndex(ContactsContract.IDeletedContactsColumns.ContactDeletedTimestamp)) ?? "";

        // Contact was added or updated
        // Check if contact exists in our database
        var existsInDb = await _kKCurdOperation.CheckContactExistsInDb(contactId);
        if (existsInDb)
        {
          Debug.WriteLine($"Contact Deleted: {contactId}");
          await _kKCurdOperation.DeleteContactsByIds(contactId);
        }

      }
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error deleting contacts: {ex.Message}");
    }
    finally
    {
      deletedCursor?.Close();
    }
  }

  private async Task<bool> ProcessContactChanges(List<KKSqlTableForContact> addedContacts,
    List<KKSqlTableForContact> updatedContacts)
  {
    try
    {
      if (_kKCurdOperation == null)
      {
        Debug.WriteLine("KKCurdOperation is null, cannot process contact changes");
        return false;
      }

      var insertedData = Task.Run(async () =>
      {
        // Process added contacts
        if (addedContacts.Any())
        {
          await _kKCurdOperation.InsertContactData(addedContacts);
          Debug.WriteLine($"Inserted {addedContacts.Count} new contacts");
        }
      });

      var updatedData = Task.Run(async () =>
      {
        // Process updated contacts
        if (updatedContacts.Any())
        {
          await _kKCurdOperation.UpsertContactDataBulk(updatedContacts);
          Debug.WriteLine($"Updated {updatedContacts.Count} contacts");
        }
      });
      await Task.WhenAll(insertedData, updatedData);

      return true;
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error processing contact changes: {ex.Message}");
      return false;
    }
  }



  /// <summary>
  /// Reset sync - clears the timestamp to force full sync next time
  /// </summary>
  public void ResetSyncHistory()
  {
    try
    {
      var globalVariable = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
      if (globalVariable == null) return;

      var prefs = globalVariable.GetSharedPreferences("ContactSync", FileCreationMode.Private);
      if (prefs == null) return;

      var editor = prefs.Edit();
      if (editor == null) return;

      editor.Remove("ContactSyncTimestamp");
      editor.Apply();

      _lastSyncTimestamp = 0;
      Debug.WriteLine("Sync history reset - next sync will be full");
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error resetting sync history: {ex.Message}");
    }
  }

  /// <summary>
  /// Check if we have a saved sync timestamp
  /// </summary>
  public bool HasHistoryToken()
  {
    return _lastSyncTimestamp > 0;
  }

  public void Dispose()
  {
    // No resources to dispose for Android
  }
}
