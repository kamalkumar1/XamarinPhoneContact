using System;

namespace MauiPhoneContactLibrary.Helper
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

        public int PageSize = 20;

        //Expected Total Contact Count to fetch from Phone Contact
        public int ExpectedTotalPhoneContact = 1000;

        // SearchBar Configuration
        public string SearchBarPlaceholder = "Search contacts...";
        public Color SearchBarBackgroundColor = Colors.White;
        public Color SearchBarTextColor = Colors.Black;
        public Color SearchBarPlaceholderColor = Colors.Gray;
        public Color SearchBarIconColor = Colors.Gray;
        public double SearchBarFontSize = 14;
        public string SearchBarFontFamily = "Arial";
        public FontAttributes SearchBarFontAttributes = FontAttributes.Bold;

        // CollectionView Configuration
        public int CollectionViewItemSpacing = 5;
        public int RemainingItemsThreshold = 5;
        public bool ShowVerticalScrollBar = false;
        public SelectionMode CollectionSelectionMode = SelectionMode.Multiple;
        public Color SeparateColor = Colors.LightGray;

        // Group Header Configuration
        public double GroupHeaderFontSize = 16;
        public FontAttributes GroupHeaderFontAttributes = FontAttributes.Bold;
        public Thickness GroupHeaderPadding = new Thickness(10, 5);
        public Color GroupHeaderBackgroundColor = Colors.LightGray;
        public Color GroupHeaderTextColor = Colors.Black;

        // Contact Item Configuration
        public double ContactItemHeight = 80;
        public double ContactNameFontSize = 16;
        public FontAttributes ContactNameFontAttributes = FontAttributes.Bold;
        public double ContactNamePadding = 10;
        public Color ContactNameTextColor = Colors.Black;

        public double ContactPhoneFontSize = 14;
        public Color ContactPhoneTextColor = Colors.Black;
        public Thickness ContactPhonePadding = new Thickness(10, 10, 0, 10);

        // Selection Checkmark Configuration
        public double CheckmarkSize = 20;
        public string CheckmarkIcon = "checkmark";
        public Thickness CheckmarkMargin = new Thickness(5, 5, 20, 0);
        public bool ShowCheckmarkAnimation = true;

        // Selected Item Background Configuration
        public Color SelectedItemBackgroundColor = Colors.Transparent;
        public Color NormalItemBackgroundColor = Colors.Transparent;

        public Color ContactCellBackgroundColor = Colors.White;

    }



}
