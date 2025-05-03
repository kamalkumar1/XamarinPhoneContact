using System;

namespace XamarinPhoneContact.Helper
{
    public static  class kkContactControl
    {
       
         /// <summary>
        /// The color of the list separator.
        /// </summary>
        public  static  Color ListSepratorColor = Color.FromArgb("#D3D3D3");
        /// <summary>
        /// The Close button background color.
        /// </summary>
        public static Color CloseButtonBackgroundColor = Color.FromArgb("#D3D3D3");
        /// <summary>
        /// Close button text color.
        /// </summary>
        public static Color CloseButtonTextColor = Color.FromArgb("#FFFFFF");
        /// <summary>
        /// Close Button Image icon name
        /// </summary>
        public static string? CloseButtonImageName;
        /// <summary>
        /// Close Button Title
        /// </summary>
        public static string CloseButtonTitle = "Close";
        /// <summary>
        /// Enable the search bar.
        /// </summary>
        public static bool EnableSearchBar = true;
        /// <summary>
        /// Enable the CloseButton.
        /// </summary>
        public static bool Dismisbutton = true;
        /// <summary>
        /// Enable MutliSelectionTickMark.
        /// </summary>
        public static bool EnableMultiSelectionTickMark;
        // private void GetButonImageName
        /// <summary>
        /// Get Birthday detail while select the contact.
        /// </summary>
        public static bool ShowBithday = false;
        /// <summary>
        /// Get Email detail while select the contact.
        /// </summary>
        public static bool ShowEmail = false;
        /// <summary>
        /// Get Address detail while select the contact.
        /// </summary>
        public static bool ShowAddress = false;
        /// <summary>
        /// Get Company detail while select the contact.
        /// </summary>
        public static bool ShowCompany = false;
        /// <summary>
        /// Get Url detail while select the contact.
        /// </summary>
        public static bool ShowUrl = false;
        /// <summary>
        /// Get Date like birhtday detail while select the contact.
        /// </summary>
         public static bool GetDate = false;

        /// <summary>
        /// While loading the contact below text will be shown
        /// </summary>
         public static string Loadingtext = "Fetching your contact...";

    }
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
       
        public int ContactPermission = 1107;

    }



}
