#PhoneContact.Maui.KK Developed using MAUI.

## Overview
PhoneContact.Maui.KK is a **MAUI** application that enables users to access and manage phone contacts efficiently. The app provides a clean UI for viewing, searching, and interacting with contacts stored on an Android or iOS device.

## Read Documentaion how to add the libary
https://medium.com/@kamalelango15/fetch-mobile-contact-in-maui-for-both-iphone-and-android-eefc296985c0
## Features
- Retrieve and display phone contacts
- Search and filter contacts
- Select multiple contacts
- Display contact details such as name, phone number, email, address, company, and birthday
- Customizable Close button
- Request runtime permissions for accessing contacts
- Cross-platform support (**Android & iOS**)
- Smooth UI experience with **MAUI**
## CUSTOMISABLE INFORMATON
BY USING THE BELOW PROPERTIES YOU CAN CUSTOMISE THE CONTACT CONTROL.

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

## Prerequisites
Before running the project, ensure you have:
- **MAUI**** installed
- **Visual Studio** with MAUI development enabled
- **Android SDK** (for Android development)
- **iOS Simulator or device** (for iOS development)
## DOCUEMNTAION 
## BY USING BELOW GIVEN LINK YOU CAN SEE THE DOCUMENTATION FOR THIS PROJECT.
## Add the below given code to show the Custom contact control
 kkContactControl.EnableMultiSelectionTickMark = true;
 IContact contact = new ContactList(); // Assuming Contact implements IContact
 MobileContact mobile = new MobileContact(contact);
 mobile.getSelectedContact += Mobile_GetSelectedContactItem;
 await Navigation.PushModalAsync(mobile);
# Add the below given code to get the selected contact detail
private void Mobile_GetSelectedContactItem(ContactItem contactItem){}
## Installation
1. Clone the repository:
   ```sh
   git clone https://github.com/kamalkumar1/XamarinPhoneContact
