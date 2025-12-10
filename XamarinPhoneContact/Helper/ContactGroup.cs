using System;
using System.Collections.Generic;
using XamarinPhoneContact.Model;

namespace XamarinPhoneContact.Helper
{
    public class ContactGroup : List<KKSqlTableForContact>
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
