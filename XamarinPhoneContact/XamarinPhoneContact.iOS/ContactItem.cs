using System;
using System.Collections.Generic;
using Foundation;
namespace XamarinPhoneContact.iOS
{
    public class ContactItem
    {
        public string Birthday { get; set; }
        public List<Dictionary<string,string>> Dates { get; set; }
        public string DepartmentName { get; set; }
        public string EmailAddresses { get; set; }
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
        public string Identifier { get; set; }
        public string ImageData { get; set; }
        public string ImageDataAvailable { get; set; }
        public string InstantMessageAddresses { get; set; }
        public string JobTitle { get; set; }
        public string MiddleName { get; set; }
        public string NamePrefix { get; set; }
        public string NameSuffix { get; set; }
        public string Nickname { get; set; }
        public string NonGregorianBirthday { get; set; }
        public string Note { get; set; }
        public string OrganizationName { get; set; }
        public string PhoneNumbers { get; set; }
        public string PhoneticFamilyName { get; set; }
        public string PhoneticGivenName { get; set; }
        public string PhoneticMiddleName { get; set; }
        public string PhoneticOrganizationName { get; set; }
        public string PostalAddresses { get; set; }
        public string PreviousFamilyName { get; set; }
        public string Relations { get; set; }
        public string SocialProfiles { get; set; }
        public string ThumbnailImageData { get; set; }
        public string Type { get; set; }
        public string UrlAddresses { get; set; }
    }
}
