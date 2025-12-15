# Contact Configuration Quick Start

## Basic Setup

Add this to your `MauiProgram.cs` **before** creating any contact views:

```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        
        // Configure contact views BEFORE using them
        ConfigureContactViews();
        
        // ... rest of your setup
    }
    
    private static void ConfigureContactViews()
    {
        var config = ContactConfig.Instance;
        
        // Customize as needed
        config.SearchBarPlaceholder = "Search...";
        config.ContactNameFontSize = 18;
        config.GroupHeaderBackgroundColor = Colors.Blue;
        // ... more customizations
    }
}
```

## Quick Theme Examples

### Light Theme
```csharp
ContactConfig.Instance.SearchBarBackgroundColor = Colors.White;
ContactConfig.Instance.GroupHeaderBackgroundColor = Colors.LightGray;
ContactConfig.Instance.ContactNameTextColor = Colors.Black;
```

### Dark Theme
```csharp
ContactConfig.Instance.SearchBarBackgroundColor = Color.FromRgba("#2C2C2E");
ContactConfig.Instance.GroupHeaderBackgroundColor = Color.FromRgba("#1C1C1E");
ContactConfig.Instance.ContactNameTextColor = Colors.White;
```

### Compact Layout
```csharp
ContactConfig.Instance.ContactItemHeight = 60;
ContactConfig.Instance.ContactNameFontSize = 14;
ContactConfig.Instance.CollectionViewItemSpacing = 2;
```

## All Configurable Properties

| Property | Default | Description |
|----------|---------|-------------|
| **SearchBar** | | |
| `SearchBarPlaceholder` | "Search contacts..." | Search box placeholder text |
| `SearchBarFontSize` | 14 | Search text size |
| `SearchBarBackgroundColor` | White | Search bar background |
| **CollectionView** | | |
| `CollectionViewItemSpacing` | 5 | Space between items |
| `RemainingItemsThreshold` | 5 | Trigger load more threshold |
| `PageSize` | 20 | Items per page |
| **Group Header** | | |
| `GroupHeaderFontSize` | 16 | Header text size |
| `GroupHeaderBackgroundColor` | LightGray | Header background |
| `GroupHeaderPadding` | 10,5 | Header padding |
| **Contact Item** | | |
| `ContactItemHeight` | 80 | Item container height |
| `ContactNameFontSize` | 16 | Name text size |
| `ContactPhoneFontSize` | 14 | Phone text size |
| `CheckmarkSize` | 20 | Selection checkmark size |

See [CONTACT_CONFIGURATION.md](CONTACT_CONFIGURATION.md) for the complete list.

## Where to Apply Configuration

1. **MauiProgram.cs** - Best for app-wide settings
2. **App.xaml.cs** - Alternative location for global settings
3. **Before View Creation** - Settings must be applied before views are instantiated

## Components Using Configuration

- `ContactSearchBar` - Search functionality
- `ContactCollectionView` - Single contact list
- `GroupedContactCollectionView` - Grouped contact list
- `GroupHeaderView` - Group headers
- `ContactItemView` - Individual items

## Note

Configuration is **singleton-based** - changes affect all contact view instances. Apply configuration once at app startup for consistent styling across all views.
