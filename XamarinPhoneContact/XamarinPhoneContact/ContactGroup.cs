﻿using System;
using System.Collections.Generic;

namespace XamarinPhoneContact
{
    public class ContactGroup:List<ContactItem>
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
