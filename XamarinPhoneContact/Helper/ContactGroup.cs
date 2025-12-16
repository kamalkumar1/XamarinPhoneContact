using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XamarinPhoneContact.Model;

namespace XamarinPhoneContact.Helper
{
    public class ContactGroup : ObservableCollection<ContactItem>
    {
        public string Title { get; set; }
        public string ShortTitle { get; set; }
        public ContactGroup(string title, string shortTitle)
        {
            Title = title;
            ShortTitle = shortTitle;
        }
    }
}
