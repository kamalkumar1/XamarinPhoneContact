using Foundation;
using Contacts;
using XamarinPhoneContact.Helper;
using System.Diagnostics;
using XamarinPhoneContact.Interface.LocalDB;
using XamarinPhoneContact.Model;
using XamarinPhoneContact.Interface;

namespace XamarinPhoneContact.Platforms.iOS
{
    public class ReadPhoneContactData : IKKPhoneContactData
    {
        CNContactStore? _store;
        CNContactFetchRequest? _request;
        CNContactHelper _cNContactHelper;
        List<CNContact> _mastertotalcncontact = new List<CNContact>(1000);
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
        public async Task<KKContactResulType> GetAllContactFromPhoneAndStoreToLocalDbAsync()
        {
            try
            {
                try
                {
                    if (_kKCurdOperation == null)
                        return KKContactResulType.UknownFailure;

                    Debug.WriteLine("📱 Performing full sync...");
                    var result = await FetchAndProcessContactAsync();
                    return result;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in ProcessContactsWithPermission: {ex.Message}");
                    return KKContactResulType.UknownFailure;
                }
                finally
                {
                    DisposeAllData();
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetAllContactFromPhoneAndStoreToLocalDbAsync: {ex.Message}");
                return KKContactResulType.UknownFailure;
            }
            finally
            {
                DisposeAllData();
            }
        }

        async Task<KKContactResulType> FetchAndProcessContactAsync()
        {
            var result = KKContactResulType.UknownFailure;
            try
            {
                await Task.Run(async () =>
               {
                   NSError error;
                   _store = new CNContactStore();
                   _request = new CNContactFetchRequest(_cNContactHelper.GetCNcontactKey())
                   {
                       SortOrder = CNContactSortOrder.GivenName
                   };
                   var token = _store.CurrentHistoryToken;
                   if (token != null)
                   {
                       _cNContactHelper.SaveHistoryToken(token);
                   }
                   _store.EnumerateContacts(_request, out error, HandleCNContactStoreListContactsHandler);

                   Debug.Write("KKContontrol Sync in Progress...");
                   result = await ProcessContactsParallel(_mastertotalcncontact);

               });
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in GetAllContactFromPhoneAndStoreToLocalDbAsync: " + ex.Message);
                return KKContactResulType.UknownFailure;
            }
        }
        void HandleCNContactStoreListContactsHandler(CNContact contact, ref bool stop)
        {
            Debug.Write("KKContontrol Sync in Progress...");
            if (stop == false)
            {
                _mastertotalcncontact.Add(contact);
            }
            else
            {
                Debug.WriteLine("Contact Enumeration completed");
            }
        }
        async Task<KKContactResulType> ProcessContactsParallel(List<CNContact> rawContacts)
        {
            var processTasks = rawContacts.Select(contact => Task.Run(() => _cNContactHelper.ProcessSingleContact(contact))).ToArray();
            var allItems = await Task.WhenAll(processTasks);
            var getlistoftatalcontact = allItems.ToList();
            Debug.Write("KKContontrol Sync is Completed...");
            var kkContactControl = await _kKCurdOperation.InsertContactData(getlistoftatalcontact);
            await _kKCurdOperation.InsertSyncUpdate(true);
            Debug.Write("KKContontrol Sync is Insterted Successfully to LocalDB...");
            return KKContactResulType.FirstSynCompleted;
        }
        public void DisposeAllData()
        {
            _store?.Dispose();
            _request?.Dispose();
            _mastertotalcncontact?.Clear();
            _mastertotalcncontact = null;
        }
    }
}
