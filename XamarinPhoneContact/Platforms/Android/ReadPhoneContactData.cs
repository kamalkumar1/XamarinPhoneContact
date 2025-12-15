
using System.Diagnostics;
using Android.Content;
using Android.Database;
using Android.Provider;
using XamarinPhoneContact.Helper;
using XamarinPhoneContact.Interface;
using XamarinPhoneContact.Interface.LocalDB;
using XamarinPhoneContact.Model;

namespace XamarinPhoneContact.Platforms.Android;

public class ReadPhoneContactData : IKKPhoneContactData
{
  CNContactHelper cNContactHelper;
  IKKCurdOperation? _kKCurdOperation;
  public ReadPhoneContactData(IKKCurdOperation kKCurdOperation)
  {
    _kKCurdOperation = kKCurdOperation;
  }
  public async Task<bool> CheckLocalDbFirstTimeSyncStatusAsync()
  {
    try
    {
      if (_kKCurdOperation == null)
        return false;

      var checkDbSyncStatus = await _kKCurdOperation.GetFullSyncUpdate();
      return checkDbSyncStatus;
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error in CheckLocalDbSyncStatusAsync: {ex.Message}");
      return false;
    }
  }

  public void DisposeAllData()
  {
    _kKCurdOperation = null;
    GC.Collect();
  }

  public async Task<KKContactResulType> GetAllContactFromPhoneAndStoreToLocalDbAsync()
  {
    ICursor? myCursor = null;
    ContentResolver? contentResolver = null;
    //Android.Net.Uri? contentUris = null;

    try
    {
      Debug.Write("KKContontrol Sync is Started...");
      // Get current timestamp before starting the sync
      var currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

      contentResolver = Platform.CurrentActivity?.ContentResolver;
      if (contentResolver == null)
        return KKContactResulType.UknownFailure;

      var contentUris = ContactsContract.Contacts.ContentUri;
      if (contentUris == null)
        return KKContactResulType.UknownFailure;


      myCursor = contentResolver.Query(
        contentUris,
        null,
        null,
        null,
        "upper(" + ContactsContract.CommonDataKinds.Phone.InterfaceConsts.DisplayName + ") ASC");
      if (myCursor == null)
        return KKContactResulType.UknownFailure;
      if (myCursor.Count > 0)
      {
        var result = await LoadCursorData(myCursor, contentResolver);
        if (!result)
        {
          return KKContactResulType.UknownFailure;
        }
        Debug.Write("KKContontrol Sync is Completed Successfully...");
        // Save the latest timestamp for next sync
        cNContactHelper.SaveSyncTimestamp(currentTimestamp);
        return KKContactResulType.FirstSynCompleted;
      }
      else
      {
        Debug.Write("KKContontrol: No contacts found");
        return KKContactResulType.NoChangesFoundToUpdate;
      }
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error reading contacts: {ex.Message}");
      return KKContactResulType.UknownFailure;
    }
    finally
    {
      myCursor?.Close();
      contentResolver = null;
      DisposeAllData();
    }
  }
  async Task<bool> LoadCursorData(ICursor myCursor, ContentResolver contentResolver)
  {
    try
    {
      var contactIds = new List<(string Id, string DisplayName)>();
      // First pass: collec all contact IDs (fast, sequential)
      while (myCursor.MoveToNext())
      {
        var id = myCursor.GetString(myCursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.Id)) ?? string.Empty;
        var displayName = myCursor.GetString(myCursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.DisplayName)) ?? string.Empty;
        contactIds.Add((id, displayName));
      }
      Debug.WriteLine($"KKControl: Processing {contactIds.Count} contacts in parallel...");
      var processTasks = contactIds.Select(contact => Task.Run(() => cNContactHelper.ProcessSingleContact(contact.Id, contact.DisplayName, contentResolver))).ToArray();
      var allItems = await Task.WhenAll(processTasks);
      var getlistoftatalcontact = allItems.ToList();

      Debug.Write("KKContontrol Sync is Completed...");
      if (_kKCurdOperation != null)
      {
        var kkContactControl = await _kKCurdOperation.InsertContactData(getlistoftatalcontact);
        await _kKCurdOperation.InsertSyncUpdate(true);
      }
      Debug.Write("KKContontrol Sync is Insterted Successfully to LocalDB...");
      Debug.WriteLine("KKControl: Parallel processing completed");
      return true;
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error loading cursor data: {ex.Message}");
      return false;
    }
    finally
    {
      myCursor.Close();
      Debug.WriteLine("KKControl: LoadCursorData finished");
    }
  }

}
