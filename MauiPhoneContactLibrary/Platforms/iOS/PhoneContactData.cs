using System;
using Foundation;
using Contacts;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MauiPhoneContactLibrary.Helper;
using System.Linq;

namespace MauiPhoneContactLibrary.iOS
{
    public class PhoneContactData
    {
        //  string[] chars = { "_", "$", "!", "<", ">" };
        List<ContactItem> totalContactListWithoutGrouping;
        List<ContactGroup> totalContactList = GroupContactHelper.CreateDefaultGroups();

        public static NSString[] AllKeys = new NSString[]
        {
        CNContactKey.Birthday,
        CNContactKey.ThumbnailImageData,
        CNContactKey.SocialProfiles,
        CNContactKey.Relations,
        CNContactKey.PreviousFamilyName,
        CNContactKey.PostalAddresses,
        CNContactKey.PhoneticOrganizationName,
        CNContactKey.PhoneticMiddleName,
        CNContactKey.PhoneticGivenName,
        CNContactKey.PhoneticFamilyName,
        CNContactKey.PhoneNumbers,
        CNContactKey.OrganizationName,
        CNContactKey.NonGregorianBirthday,
        CNContactKey.Nickname,
        CNContactKey.NameSuffix,
        CNContactKey.NamePrefix,
        CNContactKey.MiddleName,
        CNContactKey.JobTitle,
        CNContactKey.InstantMessageAddresses,
        CNContactKey.ImageDataAvailable,
        CNContactKey.ImageData,
        CNContactKey.Identifier,
        CNContactKey.GivenName,
        CNContactKey.FamilyName,
        CNContactKey.EmailAddresses,
        CNContactKey.DepartmentName,
        CNContactKey.Dates,
        CNContactKey.Type,
        CNContactKey.UrlAddresses
        };

        /// <summary>
        /// Gets all contact from phone.
        /// </summary>
        /// <returns>The all contact from phone.</returns>
        public Dictionary<string, object> GetAllContactFromPhone()
        {
            NSError error;
            CNContactStore store = new CNContactStore();
            CNContactFetchRequest request = new CNContactFetchRequest(AllKeys);
            request.SortOrder = CNContactSortOrder.GivenName;
            totalContactListWithoutGrouping = new List<ContactItem>();
            store.EnumerateContacts(request, out error, HandleCNContactStoreListContactsHandler);
            var dict = new Dictionary<string, object>
            {
                { "Group", totalContactList },
                { "List", totalContactListWithoutGrouping }
            };
            return dict;

        }
        public async Task<Dictionary<string, object>> GetAllContactFromPhoneAsync()
        {
            var rawContacts = new List<CNContact>(1000); // Pre-allocate capacity
            NSError error;

            using var store = new CNContactStore();
            var request = new CNContactFetchRequest(AllKeys); // Keep AllKeys for full data
            request.SortOrder = CNContactSortOrder.GivenName;

            // Phase 1: Collect raw CNContact objects only (milliseconds)
            store.EnumerateContacts(request, out error, (contact, ref stop) =>
            {
                rawContacts.Add(contact);  // ~1μs per contact
            });

            if (error != null) throw new Exception($"Contact fetch failed: {error.LocalizedDescription}");

            // Phase 2: Parallel processing (see below)
            return await ProcessContactsParallel(rawContacts);
        }
        private async Task<Dictionary<string, object>> ProcessContactsParallel(List<CNContact> rawContacts)
        {
            // Parallel extraction of ALL properties
            var processTasks = rawContacts.Select(contact => Task.Run(() => ProcessSingleContact(contact))).ToArray();
            var allItems = await Task.WhenAll(processTasks);

            // Parallel grouping (maintains original order)
            var groupTasks = allItems.Select(item => Task.Run(() => GroupContact(item))).ToArray();
            var groupedItems = await Task.WhenAll(groupTasks);

            return new Dictionary<string, object>
            {
                { "Group", totalContactList },  // Your alphabetized groups
                { "List", allItems.ToList() }   // Flat list
            };
        }

        private ContactItem ProcessSingleContact(CNContact contact)
        {
            var item = new ContactItem { ContactID = contact.Identifier ?? "" };
            // Your existing methods - now run in parallel across contacts
            GetDisplayName(contact, item);
            GetName(contact, item);
            GetPhoneNumber(contact, item);
            // ... all other conditional extractions
            //Birthday
            if (kkContactControl.ShowBithday) GetBirthDay(contact, item);
            //Email
            if (kkContactControl.ShowEmail) GetEmails(contact, item);
            //Address
            if (kkContactControl.ShowAddress) GetAddress(contact, item);
            //GetCompany
            if (kkContactControl.ShowCompany) GetCompany(contact, item);
            //GetUrls
            if (kkContactControl.ShowUrl) GetUrls(contact, item);
            //GetDate
            if (kkContactControl.GetDate) GetDate(contact, item);

            return item;
        }

        private ContactItem GroupContact(ContactItem item)
        {
            var index = GroupContactHelper.GetGroupIndex(item.DisplayName);
            lock (totalContactList) // Thread-safe grouping
            {
                totalContactList[index].Add(item);
            }
            return item;
        }



        void GetBirthDay(CNContact contact, ContactItem item)
        {
            if (contact.Birthday != null)
            {
                var month = contact.Birthday.Month.ToString();
                var day = contact.Birthday.Day.ToString();
                var year = contact.Birthday.Year.ToString();
                item.Birthday = day + "/" + month + "/" + year;
            }
        }
        void GetDisplayName(CNContact contact, ContactItem item)
        {
            if (contact.GivenName.Length > 0 || contact.FamilyName.Length > 0)
            {
                //Displayname
                item.DisplayName = contact.GivenName + " " + contact.FamilyName;
            }
            else
            {
                item.DisplayName = "";
            }
        }
        void GetName(CNContact contact, ContactItem item)
        {
            Name name = new Name();
            name.FirstName = contact.GivenName;
            name.LastName = contact.FamilyName;
            name.Prefix = contact.NamePrefix;
            name.Suffix = contact.NameSuffix;
            name.MiddleName = contact.MiddleName;
            item.GetNames = name;
            Console.WriteLine(contact.FamilyName ?? "");
        }
        void GetPhoneNumber(CNContact contact, ContactItem item)
        {
            List<Phone> phoneslist = new List<Phone>();
            foreach (var number in contact.PhoneNumbers)
            {
                string phonenumber = number.Value.ValueForKey(new NSString("digits")).ToString() ?? "";
                string phonenumberType = CNLabeledValue<NSString>.LocalizeLabel(new NSString(number.Label ?? "")) ?? "";
                Phone phone = new Phone
                {
                    Type = phonenumberType,
                    PhoneNumber = phonenumber
                };
                phoneslist.Add(phone);
                Console.WriteLine(phonenumber);

            }
            item.GetPhones = phoneslist;
        }
        void GetEmails(CNContact contact, ContactItem item)
        {
            List<Emailids> emailslist = new List<Emailids>();
            foreach (var obj in contact.EmailAddresses)
            {
                var type = Regex.Replace(obj.Label, @"[^0-9a-zA-Z]+", "") ?? "";
                var email = Regex.Replace(obj.Value, @"[^0-9a-zA-Z@]+", "") ?? "";
                Emailids email1 = new Emailids();
                email1.Emailid = email;
                email1.Type = type;
                emailslist.Add(email1);
            }
            item.GetEmails = emailslist;
        }
        void GetAddress(CNContact contact, ContactItem item)
        {
            List<Address> addresseslist = new List<Address>();
            foreach (var obj in contact.PostalAddresses)
            {
                Address address = new Address();
                var type = Regex.Replace(obj.Label, @"[^0-9a-zA-Z]+", "") ?? "";
                var street = obj.Value.Street;
                var city = obj.Value.City;
                var state = obj.Value.State;
                var postalcode = obj.Value.PostalCode;
                var country = obj.Value.Country;
                address.Type = type;
                address.FullAddress = street + "," + city + "," + "," + state + "," + postalcode + "," + country;
                addresseslist.Add(address);
            }
            item.GetAddress = addresseslist;
        }
        void GetCompany(CNContact contact, ContactItem item)
        {
            Company company = new Company
            {
                CompanyName = contact.OrganizationName ?? "",
                Role = contact.JobTitle
            };
            item.GetCompany = company;

        }
        void GetUrls(CNContact contact, ContactItem item)
        {
            List<Url> url = new List<Url>();
            foreach (var obj in contact.UrlAddresses)
            {
                Url url1 = new Url();
                url1.URL = Regex.Replace(obj.Value, @"[^0-9a-zA-Z@]+", "") ?? "";
                url.Add(url1);
            }
            item.GetUrls = url;
        }
        void GetDate(CNContact contact, ContactItem item)
        {
            //Date
            List<DateList> dateLists = new List<DateList>();
            foreach (var obj in contact.Dates)
            {
                var type = Regex.Replace(obj.Label, @"[^0-9a-zA-Z]+", "") ?? "";
                var datestring = obj.Value;
                DateList list = new DateList();
                var month = contact.Birthday.Month.ToString() ?? "";
                var day = contact.Birthday.Day.ToString() ?? "";
                var year = contact.Birthday.Year.ToString() ?? "";
                list.Date = day + "/" + month + "/" + year;
                list.type = type;
                dateLists.Add(list);
            }
            item.GetDateList = dateLists;

        }
        void HandleCNContactStoreListContactsHandler(CNContact contact, ref bool stop)
        {
            if (stop == false)
            {
                try
                {
                    ContactItem item = new ContactItem();
                    item.ContactID = contact.Identifier ?? "";
                    //DisplayName
                    GetDisplayName(contact, item);
                    //Name
                    GetName(contact, item);
                    //Phone
                    GetPhoneNumber(contact, item);

                    if (kkContactControl.ShowBithday)
                    {
                        //Birthday
                        GetBirthDay(contact, item);
                    }
                    if (kkContactControl.ShowEmail)
                    {
                        //Email
                        GetEmails(contact, item);
                    }
                    if (kkContactControl.ShowAddress)
                    {
                        //Address
                        GetAddress(contact, item);
                    }
                    if (kkContactControl.ShowCompany)
                    {
                        //GetCompany
                        GetCompany(contact, item);
                    }
                    if (kkContactControl.ShowUrl)
                    {
                        //GetUrls
                        GetUrls(contact, item);
                    }
                    if (kkContactControl.GetDate)
                    {
                        //GetDate
                        GetDate(contact, item);
                    }
                    totalContactListWithoutGrouping.Add(item);


                    try
                    {
                        if (item.DisplayName != null && !string.IsNullOrEmpty(item.DisplayName))
                        {
                            var firstLetter = item.DisplayName.Substring(0, 1).ToUpper();
                            var indexs = Array.IndexOf(alphate, firstLetter);
                            totalContactList[indexs].Add(item);
                        }
                        else
                        {
                            totalContactList[26].Add(item);
                        }
                        //var vcvc = from s in totalContactList where s.Count > 0 select s.ToList();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            else
            {
                Console.WriteLine(stop);
            }
        }
    }

}
