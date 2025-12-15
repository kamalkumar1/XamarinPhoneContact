# Contact Configuration Guide

All contact-related UI properties can be customized through the `ContactConfig` class. This allows you to change the appearance and behavior of contact lists without modifying XAML files.

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
2. **ContactCollectionView** - Flat contact list
3. **GroupedContactCollectionView** - Grouped contact list
4. **GroupHeaderView** - Group section headers
5. **ContactItemView** - Individual contact item display

## Notes

- Changes to configuration should be made before views are created
- Configuration is singleton, changes affect all instances
- Some properties may not be supported on all platforms
- Colors can be set using `Colors` class or custom `Color` instances
