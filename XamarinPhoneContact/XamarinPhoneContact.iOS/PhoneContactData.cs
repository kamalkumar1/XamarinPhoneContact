using System;
using Foundation;
using Contacts;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace XamarinPhoneContact.iOS
{
	public class PhoneContactData
	{
        string[] chars = { "_", "$", "!", "<",">" };
        List<ContactItem> totalContactList;
		/// <summary>
		/// Gets all contact from phone.
		/// </summary>
		/// <returns>The all contact from phone.</returns>
		public List<ContactItem> GetAllContactFromPhone()
		{
			NSError error;
			var keyToFetchs = new NSString[]   { CNContactKey.Birthday,
												 CNContactKey.Dates,
												 CNContactKey.DepartmentName,
												 CNContactKey.EmailAddresses,
            CNContactKey.FamilyName,CNContactKey.GivenName,CNContactKey.Identifier,CNContactKey.ImageData,CNContactKey.ImageDataAvailable,
            CNContactKey.InstantMessageAddresses,CNContactKey.JobTitle,CNContactKey.MiddleName,CNContactKey.NamePrefix,CNContactKey.NameSuffix,
            CNContactKey.Nickname,CNContactKey.NonGregorianBirthday,CNContactKey.Note,CNContactKey.OrganizationName,
            CNContactKey.PhoneNumbers,CNContactKey.PhoneticFamilyName,CNContactKey.PhoneticGivenName,CNContactKey.PhoneticMiddleName,
            CNContactKey.PhoneticOrganizationName,CNContactKey.PostalAddresses,CNContactKey.PreviousFamilyName,
            CNContactKey.Relations,CNContactKey.SocialProfiles,CNContactKey.ThumbnailImageData,CNContactKey.Type,CNContactKey.UrlAddresses};
			CNContactStore store = new CNContactStore();
			CNContactFetchRequest request = new CNContactFetchRequest(keyToFetchs);
			totalContactList = new List<ContactItem>();
			store.EnumerateContacts(request, out error, HandleCNContactStoreListContactsHandler);
			return totalContactList;
		}


		void HandleCNContactStoreListContactsHandler(CNContact contact, ref bool stop)
		{
			if (stop == false)
			{
                foreach(var obj in contact.Dates)
                {
                    var ob = Regex.Replace(obj.Label, @"[^0-9a-zA-Z]+","");
                    var gf = obj.Value;
                    
                }

				if (contact.PhoneNumbers.Length > 0)
				{
					if (contact.PhoneNumbers.Length == 1)
					{

						var phonenumber = contact.PhoneNumbers[0].Value.ValueForKey(new NSString("digits")).ToString() ?? "";
						var phonenumberType = CNLabeledValue<NSString>.LocalizeLabel(new NSString(contact.PhoneNumbers[0].Label ?? "")) ?? "";
						//ContactItem contactItem = new ContactItem();
						//contactItem.Birthday = contact.Birthday;
						//contactItem.Dates = contact.Dates;
						//contactItem.FullName = contact.GivenName + " "+ contact.FamilyName;
						//contactItem.PhoneNumberType = phonenumberType;
						//contactItem.Phonenumber = phonenumber;
						//totalContactList.Add(contactItem);
						//Console.WriteLine(contact.GivenName + " " + contact.FamilyName);
					}
					else
					{
						foreach (var number in contact.PhoneNumbers)
						{
							var phonenumber = number.Value.ValueForKey(new NSString("digits")).ToString() ?? "";
							var phonenumberType = CNLabeledValue<NSString>.LocalizeLabel(new NSString(number.Label ?? "")) ?? "";
							//ContactItem contactItem = new ContactItem();
							//contactItem.FirstName = contact.GivenName;
							//contactItem.Lastname = contact.FamilyName;
							//contactItem.FullName = contact.GivenName + " " +contact.FamilyName;
							//contactItem.PhoneNumberType = phonenumberType;
							//contactItem.Phonenumber = phonenumber;
							//totalContactList.Add(contactItem);
							//Console.WriteLine(contact.GivenName + " " + contact.FamilyName);
						}
					}
				}
			}
			else
			{
				Console.WriteLine(stop);
			}
		}
	}

}
