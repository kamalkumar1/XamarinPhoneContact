using System;
using Foundation;
using Contacts;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using XamarinPhoneContact.Helper;
using System.Linq;
using System.Diagnostics;
using XamarinPhoneContact.Platforms.iOS;
using System.Threading.Tasks;
using XamarinPhoneContact.Model;
using XamarinPhoneContact.Interface;

namespace XamarinPhoneContact.Platforms.iOS
{
    public class PhoneContactData : KKPhoneContactBase, IKKPhoneContactData
    {
        CNContactStore? store;
        CNContactFetchRequest? request;
        CNContactHelper cNContactHelper;
        List<CNContact> mastertotalcncontact = new List<CNContact>(1000);

        /// <summary>
        /// Gets all contact from phone.
        /// </summary>
        /// <returns>The all contact from phone.</returns>
        public Dictionary<string, object> GetAllContactFromPhone()
        {
            return null;

        }
        /// <summary>
        /// Gets all contact from phone.
        /// </summary>
        /// <returns>The all contact from phone.</returns>
        public async Task<List<ContactGroup>> GetAllContactFromPhoneAsync()
        {
            List<ContactGroup>? contactItems = null;
            await Task.Run(async () =>
            {
                NSError error;
                store = new CNContactStore();
                request = new CNContactFetchRequest(cNContactHelper.GetCNcontactKey());
                request.SortOrder = CNContactSortOrder.GivenName;
                store.EnumerateContacts(request, out error, HandleCNContactStoreListContactsHandler);
                contactItems = await ProcessContactsParallel(mastertotalcncontact);
            });
            return contactItems;
        }
        void HandleCNContactStoreListContactsHandler(CNContact contact, ref bool stop)
        {
            if (stop == false)
            {
                mastertotalcncontact.Add(contact);
            }
            else
            {
                Debug.WriteLine(stop);
            }
        }
        async Task<List<ContactGroup>> ProcessContactsParallel(List<CNContact> rawContacts)
        {
            var processTasks = rawContacts.Select(contact => Task.Run(() => ProcessSingleContact(contact))).ToArray();
            var allItems = await Task.WhenAll(processTasks);
            // Parallel grouping (maintains original order)
            var groupTasks = allItems.Select(item => Task.Run(() => GroupContact(item))).ToList();
            await Task.WhenAll(groupTasks);
            return totalContactList;

        }
        private async Task<ContactItem> ProcessSingleContact(CNContact contact)
        {
            var item = new ContactItem { ContactID = contact.Identifier ?? "" };
            // Your existing methods - now run in parallel across contacts
            var taskdisplayt = Task.Run(() => cNContactHelper.GetDisplayName(contact, item));
            var tasknamet = Task.Run(() => cNContactHelper.GetName(contact, item));
            var taskphone = Task.Run(() => cNContactHelper.GetPhoneNumber(contact, item));
            var taskbithday = Task.Run(() =>
            {
                if (kkContactControl.ShowBithday) cNContactHelper.GetBirthDay(contact, item);
            });
            var taskemail = Task.Run(() =>
            {
                if (kkContactControl.ShowEmail) cNContactHelper.GetEmails(contact, item);
            });
            var taskaddress = Task.Run(() =>
            {
                if (kkContactControl.ShowAddress) cNContactHelper.GetAddress(contact, item);
            });
            var taskcompany = Task.Run(() =>
            {
                if (kkContactControl.ShowCompany) cNContactHelper.GetCompany(contact, item);
            });
            var taskshowurl = Task.Run(() =>
            {
                if (kkContactControl.ShowUrl) cNContactHelper.GetUrls(contact, item);
            });
            var taskshowdate = Task.Run(() =>
            {
                if (kkContactControl.GetDate) cNContactHelper.GetDate(contact, item);
            });

            // add optional ones similarly
            await Task.WhenAll(taskdisplayt, tasknamet, taskphone, taskbithday, taskemail, taskaddress, taskcompany, taskshowurl, taskshowdate);
            return item;
        }


    }

}
