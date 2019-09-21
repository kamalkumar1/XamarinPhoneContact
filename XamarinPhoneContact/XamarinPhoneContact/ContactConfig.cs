using System;
using System.Drawing;

namespace XamarinPhoneContact
{
    public sealed class ContactConfig
    {
        // Explicit static constructor to tell C# compiler  
        // not to mark type as beforefieldinit  
        static ContactConfig()
        {
        }
        private ContactConfig()
        {
        }
        public static ContactConfig Instance { get; } = new ContactConfig();
        public bool EnableTextChangedDelegate = true;
        public bool EnableSearchButtonPressedDelegate = true;
        public Color ListSepratorColor = Color.LightGray;
        public bool EnableSearchBar = true;
        public bool Dismisbutton = true;
        public bool EnableMultiSelectionTickMark;
        // private void GetButonImageName
        public string CloseButtonImageName;
        public string CloseButtonTitle;
        public int ContactPermission = 1107;

    }



}
