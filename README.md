#PhoneContact.Maui.KK Developed using MAUI.

## Overview
PhoneContact.Maui.KK is a **MAUI** application that enables users to access and manage phone contacts efficiently. The app provides a clean UI for viewing, searching, and interacting with contacts stored on an Android or iOS device.

## Read Documentaion how to add the libary
https://medium.com/@kamalelango15/fetch-mobile-contact-in-maui-for-both-iphone-and-android-eefc296985c0

## Features
1. **ContactSearchBar** - Search functionality
2. **ContactCollectionView** - Flat contact list(Ungrouped contact list)
3. **GroupedContactCollectionView** - Grouped contact list
4. **GroupHeaderView** - Group section headers
5. **ContactItemView** - Individual contact item display
6. **AnimatedSelectionTickMark** - when Individual contact item get selected
6. **DarkMode** - It will support both dark and light mode feature
7. **Pagination** - Get contact via pagination
   
## CUSTOMISABLE INFORMATON
BY USING THE BELOW PROPERTIES YOU CAN CUSTOMISE THE CONTACT CONTROL.
# Contact Configuration Guide
### iOS Permissions
Add the following permissions to your `Info.plist` file to access contacts on iOS:

```xml
<key>NSContactsUsageDescription</key>
<string>This app needs access to contacts to display and manage your contact list.</string>
```

**Location:** `Platforms/iOS/Info.plist`

### Android Permissions

Add the following permissions to your `AndroidManifest.xml` file to access contacts on Android:

```xml
<uses-permission android:name="android.permission.READ_CONTACTS" />
<uses-permission android:name="android.permission.WRITE_CONTACTS" />
```

**Location:** `Platforms/Android/AndroidManifest.xml`

**Example AndroidManifest.xml:**

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
    <application android:allowBackup="true" android:icon="@mipmap/appicon" android:roundIcon="@mipmap/appicon_round" android:supportsRtl="true"></application>
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.READ_CONTACTS" />
    <uses-permission android:name="android.permission.WRITE_CONTACTS" />
</manifest>
```
## Integration Guide


### Using Contact Views in Your Pages

The library provides two main contact view controls that can be integrated into your XAML pages.
Supports single and multi‑selection via properties.
Choose the appropriate control depending on whether you want grouping or not:
Grouped Contact List → KKGroupContactView with KKGroupContactViewModel
Ungrouped Contact List → KKGroupContactView with KKSingleContactViewModel
You can also add your own selection image configuration file to customize the visuals.
Can be implemented via both ViewModel and code‑behind.
Use the configuration singleton to customize attributes:
```csharp
var config = ContactConfig.Instance;
```
Through ContactConfig.Instance, you can configure:
Font styles
Images (including selection icons)
Background colors
Placeholder text

```csharp
var config = ContactConfig.Instance;

//Setup you ruf estimation target user contact count
 //By doing this you avoid the buffer memory allocation in the list.
config.ExpectedTotalPhoneContact =2000
// Customize properties
config.SearchBarPlaceholder = "Find contacts...";
config.ContactNameFontSize = 18;
config.GroupHeaderBackgroundColor = Colors.Blue;
//use this property define my contact be taken while scrolling the contact list 
public int PageSize = 20;
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
//Contact cell
public Color ContactCellBackgroundColor = Colors.White;
```

### Initialize the Contact Control

Before using any contact views, you must initialize the contact control in your `MauiProgram.cs`:

```csharp
using XamarinPhoneContact.Helper;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .SetKKContactControl(); // Add this line

        return builder.Build();
    }
}
```

**Important:** The `SetKKContactControl()` method must be called during app initialization to register all necessary handlers and services for the contact control library.


#### 1. KKSingleContactView (Ungrouped Contact List) - integrated ui via xaml

Add the namespace and use the control in your XAML:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:contact="clr-namespace:XamarinPhoneContact.Controls;assembly=XamarinPhoneContact"
             x:Class="YourApp.ContactsPage"
             Title="Contacts">
    
    <Grid x:Name ="contentGrid">
        <contact:KKSingleContactView />
    </Grid>
    
</ContentPage>
```

#### 2. KKGroupContactView (Grouped Contact List)- ntegrated ui via xaml

Add the namespace and use the control in your XAML:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:contact="clr-namespace:XamarinPhoneContact.Controls;assembly=XamarinPhoneContact"
             x:Class="YourApp.GroupedContactsPage"
             Title="Grouped Contacts">
    
    <Grid x:Name ="contentGrid">
        <contact:KKGroupContactView />
    </Grid>
    
</ContentPage>
```
#### 3. Request permission before you load contact.

 * When opening the app, the default library will check for contact permissions to sync contacts.
   If permission is not granted, a popup will be shown to alert the user.
 * When loading contacts from the ViewModel, validate the permission using           IKKContactPermissionRequest.
* Create an instance of IKKContactPermissionRequest via the ViewModel constructor, as is usually done in .NET MAUI. 

```csharp
    public interface IKKContactPermissionRequest
    {
      /// <summary>
      /// This method request contact authorization status. IF permission granted it will return true else false  and request permission and take user to the settings page
      /// </summary>
      /// <returns></returns>
      public Task<bool> GetContactAuthorizationStatus();
    }
```

#### 4. Inegrated ui via Code-Behind and load the data from code

You can also create and configure views programmatically:

```csharp
using XamarinPhoneContact.Controls;

public partial class SampleContentPage : ContentPage
{
   private KKGroupContactView _groupContactView;
   private KKSingleContactView _groupContactView;


//	public SampleContentPage(KKSingleContactViewModel viewModel)
	public SampleContentPage(KKGroupContactViewModel viewModel)
	{
        InitializeComponent();

          // Configure before adding view
        var config = ContactConfig.Instance;
        config.SearchBarPlaceholder = "Search your contacts...";
        config.ContactNameFontSize = 18;
        config.GroupHeaderBackgroundColor = Colors.LightBlue;
        
        // Option 1: Ungrouped contacts
         SetupSingleContactView(viewModel);
        // Option 2: Setupgrouped contacts
         Option 2: Grouped contacts
           SetupGroupContactView(viewModel)
       
    }
    /// <summary>
    /// This method setups the single contact view with out section based on the uiview 
    /// </summary>
    /// <param name="viewModel"></param>
	void SetupSingleContactView(KKSingleContactViewModel viewModel)
	{
		_viewModel = viewModel;
		BindingContext = _viewModel;
		
		//page we need to add contact view
    //Avoid this line if UI lareday ingeated in xaml page
    _contactView = new KKSingleContactView(_viewModel);
		contentGrid.Children.Add(_contactView);
	}
  /// <summary>
	/// This method setups the group contact view with  section based on the uiview
	/// </summary>
	/// <param name="viewModel"></param>
	void SetupGroupContactView(KKGroupContactViewModel viewModel)
	{
		// Create and cache the ContentView
		_groupViewModel = viewModel;
		BindingContext = _groupViewModel;

    //page we need to add contact view
    //Avoid this line if UI lareday ingeated in xaml page
    		_groupContactView = new KKGroupContactView(_groupViewModel);
		contentGrid.Children.Add(_groupContactView);
		
	}
}
```
#### 5. Using in Code-Behind to load the contact

```csharp
   protected override async void OnAppearing()
	{
		base.OnAppearing();
		if (_groupViewModel != null)
		{
			await _groupViewModel.LoadGroupContactsAsync();
		}
		if (_viewModel != null)
		{
			await _viewModel.CalulateAndGetTotalPageCount();
			await _viewModel.LoadContactsAsync();
		}
	}
  ```

#### 6. Using in Code-Behind Get Selected Contact

```csharp
var selectedContacts = _viewModel.GetSelectedContacts();
```
#### 7. Using in Code-Behind reset the contact view

```csharp
    protected override void OnDisappearing()
	{
		base.OnDisappearing();
    	_groupViewModel?.RestViewModel();
		 // Optional: Clean up if needed
		 contentGrid.Children.Clear();
		 BindingContext = null;
	}
```
#### 8. Using in Code-Behind reset the contact view
* by usingvar config = ContactConfig.Instance; use can define the  drak and light time
 ``` /// <summary>
  /// Example: Minimal light theme
  /// </summary>
  public static void ApplyLightTheme()
  {
    var config = ContactConfig.Instance;
    config.SearchBarBackgroundColor = Colors.White;
    config.SearchBarTextColor = Colors.Black;
    config.SearchBarPlaceholderColor = Colors.Gray;
    config.GroupHeaderBackgroundColor = Colors.LightGray;
    config.GroupHeaderTextColor = Colors.Black;
    config.ContactNameTextColor = Colors.Black;
    config.ContactPhoneTextColor = Colors.Gray;
    config.SelectedItemBackgroundColor = Color.FromRgba("#E8F4F8");
    config.NormalItemBackgroundColor = Colors.Transparent;
    config.ContactCellBackgroundColor = Colors.White;
    config.SeparateColor = Colors.LightGray;
    config.AlphabetBackgroundColor = Colors.White;
    config.AlphabetTextColor = Colors.DarkBlue;
    config.CheckmarkIcon = "checkmark";
  }

  /// <summary>
  /// Example: Dark theme
  /// </summary>
  public static void ApplyDarkTheme()
  {
    var config = ContactConfig.Instance;

    config.SearchBarBackgroundColor = Color.FromRgba("#2C2C2E");
    config.SearchBarTextColor = Colors.White;
    config.SearchBarPlaceholderColor = Colors.Gray;
    config.GroupHeaderBackgroundColor = Color.FromRgba("#1C1C1E");
    config.GroupHeaderTextColor = Colors.White;
    config.ContactNameTextColor = Colors.WhiteSmoke;
    config.ContactPhoneTextColor = Color.FromRgba("#AEAEB2");
    config.SelectedItemBackgroundColor = Color.FromRgba("#3A3A3C");
    config.NormalItemBackgroundColor = Color.FromRgba("#2C2C2E");
    config.ContactCellBackgroundColor = Color.FromRgba("#2C2C2E");
    config.SeparateColor = Color.FromRgba("#3A3A3C");
    config.AlphabetBackgroundColor = Color.FromRgba("#2C2C2E");
    config.AlphabetTextColor = Colors.WhiteSmoke;
    config.CheckmarkIcon = "checkmark";
    config.ContactCellBackgroundColor = Color.FromRgba("#2C2C2E");
  }

```
## Available Configuration Properties

### SearchBar Configuration
- `SearchBarPlaceholder` - Placeholder text (default: "Search contacts...")
- `SearchBarBackgroundColor` - Background color (default: White)
- `SearchBarTextColor` - Text color (default: Black)
- `SearchBarPlaceholderColor` - Placeholder color (default: Gray)
- `SearchBarIconColor` - Search icon color (default: AliceBlue)
- `SearchBarFontSize` - Font size (default: 14)
- `SearchBarFontFamily` - Font family (default: "Arial")
- `SearchBarFontAttributes` - Font attributes (default: Bold)

### CollectionView Configuration
- `CollectionViewItemSpacing` - Space between items (default: 5)
- `RemainingItemsThreshold` - Items from bottom to trigger load more (default: 5)
- `ShowVerticalScrollBar` - Show/hide scroll bar (default: false)
- `CollectionSelectionMode` - Selection mode (default: Single)

### Group Header Configuration
- `GroupHeaderFontSize` - Font size (default: 16)
- `GroupHeaderFontAttributes` - Font attributes (default: Bold)
- `GroupHeaderPadding` - Padding (default: 10,5)
- `GroupHeaderBackgroundColor` - Background color (default: LightGray)
- `GroupHeaderTextColor` - Text color (default: Black)

### Contact Item Configuration
- `ContactItemHeight` - Item height (default: 80)
- `ContactNameFontSize` - Name font size (default: 16)
- `ContactNameFontAttributes` - Name font attributes (default: Bold)
- `ContactNamePadding` - Name padding (default: 10)
- `ContactNameTextColor` - Name text color (default: Black)
- `ContactPhoneFontSize` - Phone font size (default: 14)
- `ContactPhoneTextColor` - Phone text color (default: Black)
- `ContactPhonePadding` - Phone padding (default: 10,10,0,10)

### Selection Checkmark Configuration
- `CheckmarkSize` - Checkmark size (default: 20)
- `CheckmarkIcon` - Icon name (default: "checkmark")
- `CheckmarkMargin` - Margin (default: 5,5,20,0)
- `ShowCheckmarkAnimation` - Enable/disable animation (default: true)

### Selection Background Configuration
- `SelectedItemBackgroundColor` - Selected item background (default: Transparent)
- `NormalItemBackgroundColor` - Normal item background (default: Transparent)

## Secuirty 
List Secuirty of below given secuirty check  and encryption followed while saving the Data of the Localdb.
-Key Genration to encrypte the data.
  * Per-device uniqueness: deviceid 
  * Strong KDF is used Derives the key via Rfc2898DeriveBytes.    
  * Pbkdf2 with SHA-256 and 100,000 iterations, slowing offline brute-force attempts.
  * No hardcoded secrets: All entropy is generated at runtime; nothing sensitive is embedded in code.

## Notes

- Changes to configuration should be made before views are created
- Configuration is singleton, changes affect all instances
- This library supports iOS and Android platforms only
- The `SetKKContactControl()` method must be called in `MauiProgram.cs` before the app starts
- **iOS:** `NSContactsUsageDescription` is required in `Info.plist`
- **Android:** `READ_CONTACTS` permission is required in `AndroidManifest.xml`
- Colors can b
