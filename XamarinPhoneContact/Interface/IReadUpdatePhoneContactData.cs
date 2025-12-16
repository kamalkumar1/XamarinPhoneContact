using System;
using XamarinPhoneContact.Helper;

namespace XamarinPhoneContact.Interface
{
  public interface IReadUpdatePhoneContactData
  {
    /// <summary>
    /// Fetch only contacts that changed since last sync using CNChangeHistoryFetchRequest
    /// </summary>
    /// <returns>True if sync was successful, false if full resync is needed</returns>
    Task<KKContactResulType> SyncContactChangesAsync();

    /// <summary>
    /// Reset sync - clears the history token to force full sync next time
    /// </summary>
    void ResetSyncHistory();

    /// <summary>
    /// Check if we have a saved history token
    /// </summary>
    /// <returns>True if history token exists, false otherwise</returns>
    bool HasHistoryToken();
  }
}
