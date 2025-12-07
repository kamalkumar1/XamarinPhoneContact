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

namespace XamarinPhoneContact.iOS
{
    public class PhoneContactData
    {
        List<ContactItem>? totalContactListWithoutGrouping;
        CNContactStore? store;
        CNContactFetchRequest? request;
        CNContactHelper cNContactHelper;
        readonly static string[] alphate = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "#" };
        List<ContactGroup> totalContactList = new List<ContactGroup>
        {
            new ContactGroup(alphate[0], alphate[0]){},new ContactGroup(alphate[1], alphate[1]){},
            new ContactGroup(alphate[2], alphate[2]){},new ContactGroup(alphate[3], alphate[3]){},
            new ContactGroup(alphate[4], alphate[4]){},new ContactGroup(alphate[5], alphate[5]){},
            new ContactGroup(alphate[6], alphate[6]){},new ContactGroup(alphate[7], alphate[7]){},
            new ContactGroup(alphate[8], alphate[8]){},new ContactGroup(alphate[9], alphate[9]){},
            new ContactGroup(alphate[10], alphate[10]){},new ContactGroup(alphate[11], alphate[11]){},
            new ContactGroup(alphate[12], alphate[12]){},new ContactGroup(alphate[13], alphate[13]){},
            new ContactGroup(alphate[14], alphate[14]){},new ContactGroup(alphate[15], alphate[15]){},
            new ContactGroup(alphate[16], alphate[16]){},new ContactGroup(alphate[17], alphate[17]){},
            new ContactGroup(alphate[18], alphate[18]){},new ContactGroup(alphate[19], alphate[19]){},
            new ContactGroup(alphate[20], alphate[20]){},new ContactGroup(alphate[21], alphate[21]){},
            new ContactGroup(alphate[22], alphate[22]){},new ContactGroup(alphate[23], alphate[23]){},
            new ContactGroup(alphate[24], alphate[24]){},new ContactGroup(alphate[25], alphate[25]){},
            new ContactGroup(alphate[26], alphate[26]){}

        };
        List<CNContact> totalcncontact = new List<CNContact>(1000);


        /// <summary>
        /// Gets all contact from phone.
        /// </summary>
        /// <returns>The all contact from phone.</returns>
        public Dictionary<string, object> GetAllContactFromPhone()
        {
            NSError error;
            store = new CNContactStore();
            request = new CNContactFetchRequest(cNContactHelper.GetCNcontactKey());
            request.SortOrder = CNContactSortOrder.GivenName;
            totalContactListWithoutGrouping = new List<ContactItem>();
            store.EnumerateContacts(request, out error, HandleCNContactStoreListContactsHandler);
            ProcessData();
            var dict = new Dictionary<string, object>
            {
                { "Group", totalContactList },
                { "List", totalContactListWithoutGrouping }
            };
            return dict;

        }
        void ProcessData()
        {

        }
        void HandleCNContactStoreListContactsHandler(CNContact contact, ref bool stop)
        {
            if (stop == false)
            {
                totalcncontact.Add(contact);
                // try
                // {
                //     ContactItem item = new ContactItem();
                //     item.ContactID = contact.Identifier ?? "";

                //     //DisplayName
                //     GetDisplayName(contact, item);
                //     //Name
                //     GetName(contact, item);
                //     //Phone
                //     GetPhoneNumber(contact, item);

                //     if (kkContactControl.ShowBithday)
                //     {
                //         //Birthday
                //         GetBirthDay(contact, item);
                //     }
                //     if (kkContactControl.ShowEmail)
                //     {
                //         //Email
                //         GetEmails(contact, item);
                //     }
                //     if (kkContactControl.ShowAddress)
                //     {
                //         //Address
                //         GetAddress(contact, item);
                //     }
                //     if (kkContactControl.ShowCompany)
                //     {
                //         //GetCompany
                //         GetCompany(contact, item);
                //     }
                //     if (kkContactControl.ShowUrl)
                //     {
                //         //GetUrls
                //         GetUrls(contact, item);
                //     }
                //     if (kkContactControl.GetDate)
                //     {
                //         //GetDate
                //         GetDate(contact, item);
                //     }
                //     totalContactListWithoutGrouping.Add(item);


                //     try
                //     {
                //         if (item.DisplayName != null && !string.IsNullOrEmpty(item.DisplayName))
                //         {
                //             var firstLetter = item.DisplayName.Substring(0, 1).ToUpper();
                //             var indexs = Array.IndexOf(alphate, firstLetter);
                //             totalContactList[indexs].Add(item);
                //         }
                //         else
                //         {
                //             totalContactList[26].Add(item);
                //         }
                //         //var vcvc = from s in totalContactList where s.Count > 0 select s.ToList();
                //     }
                //     catch (Exception ex)
                //     {
                //         Debug.WriteLine(ex);
                //     }
                // }
                // catch (Exception ex)
                // {
                //     Debug.WriteLine(ex);
                // }
            }
            else
            {
                Debug.WriteLine(stop);
            }
        }
    }

}
