# Contact Configuration Guide

All contact-related UI properties can be customized through the `ContactConfig` class. This allows you to change the appearance and behavior of contact lists without modifying XAML files.

## Setup

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

### Runtime Permissions

The library handles runtime permission requests automatically. However, you should inform users why your app needs contact access before the permission dialog appears.

## Integration Guide

### Using Contact Views in Your Pages

The library provides two main contact view controls that you can integrate into your XAML pages. 
It has single and multi selection feature in the property. 
This can be implemented via both ViewModel and code behid as Well:

#### 1. KKSingleContactView (Ungrouped Contact List)

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

#### 2. KKGroupContactView (Grouped Contact List)

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

#### 3. Using in Code-Behind

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
		_contactView = new KKSingleContactView(_viewModel);
		//page we need to add contact view
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
		_groupContactView = new KKGroupContactView(_groupViewModel);
		BindingContext = _groupViewModel;
		//page we need to add contact view
		contentGrid.Children.Add(_groupContactView);
	}
}
```
#### 4. Using in Code-Behind to load the contact

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
#### 4. Using in Code-Behind Get Selected Contact
```csharp
var selectedContacts = _viewModel.GetSelectedContacts();

```
#### 5. Using in Code-Behind reset the contact view
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
## How to Use

Access the configuration singleton instance and modify properties before using any contact views:

```csharp
var config = ContactConfig.Instance;

// Customize properties
config.SearchBarPlaceholder = "Find contacts...";
config.ContactNameFontSize = 18;
config.GroupHeaderBackgroundColor = Colors.Blue;
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

## Example Usage

```csharp
// In MauiProgram.cs or App.xaml.cs before creating views
var config = ContactConfig.Instance;

// Customize search bar
config.SearchBarPlaceholder = "Type to search...";
config.SearchBarBackgroundColor = Colors.LightBlue;
config.SearchBarFontSize = 16;

// Customize group headers
config.GroupHeaderBackgroundColor = Colors.DarkBlue;
config.GroupHeaderTextColor = Colors.White;
config.GroupHeaderFontSize = 18;

// Customize contact items
config.ContactNameFontSize = 18;
config.ContactNameTextColor = Colors.DarkBlue;
config.ContactPhoneFontSize = 15;
config.ContactPhoneTextColor = Colors.Gray;

// Customize selection
config.CheckmarkSize = 24;
config.CheckmarkIcon = "check_circle"; // Use your own icon
config.SelectedItemBackgroundColor = Colors.LightGray;

// Customize pagination
config.RemainingItemsThreshold = 10;
config.PageSize = 50;
```

## Components That Use Configuration

1. **ContactSearchBar** - Search functionality
2. **ContactCollectionView** - Flat contact list(Ungrouped Collectionview)
3. **GroupedContactCollectionView** - Grouped contact list
4. **GroupHeaderView** - Group section headers
5. **ContactItemView** - Individual contact item display
6. **AnimatedSelectionTickMark** - when Individual contact item get selected
6. **DarkMode** - It will support both dark and light mode feature

## Notes

- Changes to configuration should be made before views are created
- Configuration is singleton, changes affect all instances
- This library supports iOS and Android platforms only
- The `SetKKContactControl()` method must be called in `MauiProgram.cs` before the app starts
- **iOS:** `NSContactsUsageDescription` is required in `Info.plist`
- **Android:** `READ_CONTACTS` permission is required in `AndroidManifest.xml`
- Colors can be set using `Colors` class or custom `Color` instances
