using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;
using MauiPhoneContactLibrary.Model;

namespace MauiPhoneContactLibrary.Helper
{

    public struct Name
    {
        public string? Prefix { get; set; }
        public string? Suffix { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
    }
    public struct Emailids
    {
        // public string id { get; set; }
        public string? Emailid { get; set; }
        public string? Type { get; set; }
    }
    public struct Url
    {
        public string URL { get; set; }
    }
    public struct Phone
    {
        // public string Phoneid { get; set;}
        public string PhoneNumber { get; set; }
        public string Type { get; set; }
    }
    public struct Company
    {
        public string CompanyName { get; set; }
        public string Role { get; set; }
    }
    public struct Address
    {
        public string Type { get; set; }
        public string FullAddress { get; set; }

    }
    public struct DateList
    {
        public string Date { get; set; }
        public string type { get; set; }
    }
    public class ContactItem : INotifyPropertyChanged
    {
        public int Id { get; set; }
        /// <summary>
        /// Unique id of contact
        /// </summary>
        public string? ContactID { get; set; }
        /// <summary>
        /// user Birthday date
        /// </summary>
        public string? Birthday { get; set; }
        public string? DisplayName { get; set; }
        public Name? GetNames { get; set; }
        public List<Emailids>? GetEmails { get; set; }
        public List<Url>? GetUrls { get; set; }
        public List<Phone>? GetPhones { get; set; }
        public Company? GetCompany { get; set; }
        public List<Address>? GetAddress { get; set; }
        public List<DateList>? GetDateList { get; set; }
        private bool _itemselcted;
        public bool Itemselcted
        {
            get { return _itemselcted; }
            set
            {
                if (_itemselcted == value)
                    return;
                _itemselcted = value;
                OnPropertyChanged();
            }
        }
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler? PropertyChanged;

    }
}
