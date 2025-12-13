using System;

namespace XamarinPhoneContact.Helper
{
    public static class kkContactControl
    {


        public static bool EnableSearchBar = true;
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

        public int PageSize = 10;


    }



}
