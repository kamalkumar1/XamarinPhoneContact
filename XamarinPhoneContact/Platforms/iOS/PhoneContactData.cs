using System;
using Foundation;
using Contacts;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using XamarinPhoneContact.Helper;
using System.Linq;

namespace XamarinPhoneContact.iOS
{
    public class PhoneContactData
    {
        //  string[] chars = { "_", "$", "!", "<", ">" };
        List<ContactItem> totalContactListWithoutGrouping;
        static string[] alphate = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "#" };
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
