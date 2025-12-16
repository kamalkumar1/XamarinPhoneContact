using XamarinPhoneContact.Helper;

namespace XamarinPhoneContact.Examples;

/// <summary>
/// Example of how to customize contact views using ContactConfig
/// Place this configuration in MauiProgram.cs or App.xaml.cs before creating any contact views
/// </summary>
public static class ContactThemeConfiguration
{
  public static void ConfigureContactViews()
  {
    var config = ContactConfig.Instance;

    // ============================================
    // SEARCH BAR CUSTOMIZATION
    // ============================================
    config.SearchBarPlaceholder = "Find your contacts...";
    config.SearchBarBackgroundColor = Colors.LightBlue;
    config.SearchBarTextColor = Colors.Black;
    config.SearchBarPlaceholderColor = Colors.Gray;
    config.SearchBarIconColor = Colors.DarkBlue;
    config.SearchBarFontSize = 16;
    config.SearchBarFontFamily = "OpenSans-Regular";
    config.SearchBarFontAttributes = FontAttributes.None;

    // ============================================
    // COLLECTION VIEW CUSTOMIZATION
    // ============================================
    config.CollectionViewItemSpacing = 8;
    config.RemainingItemsThreshold = 10; // Load more when 10 items from bottom
    config.ShowVerticalScrollBar = false;
    config.CollectionSelectionMode = SelectionMode.Single;

    // ============================================
    // GROUP HEADER CUSTOMIZATION (for grouped contacts)
    // ============================================
    config.GroupHeaderFontSize = 18;
    config.GroupHeaderFontAttributes = FontAttributes.Bold;
    config.GroupHeaderPadding = new Thickness(15, 8);
    config.GroupHeaderBackgroundColor = Colors.DarkSlateBlue;
    config.GroupHeaderTextColor = Colors.White;

    // ============================================
    // CONTACT ITEM CUSTOMIZATION
    // ============================================
    config.ContactItemHeight = 85;

    // Contact Name
    config.ContactNameFontSize = 18;
    config.ContactNameFontAttributes = FontAttributes.Bold;
    config.ContactNamePadding = 12;
    config.ContactNameTextColor = Colors.DarkBlue;

    // Contact Phone
    config.ContactPhoneFontSize = 15;
    config.ContactPhoneTextColor = Colors.Gray;
    config.ContactPhonePadding = new Thickness(12, 8, 0, 8);

    // ============================================
    // SELECTION CHECKMARK CUSTOMIZATION
    // ============================================
    config.CheckmarkSize = 24;
    config.CheckmarkIcon = "checkmark"; // Change to your custom icon
    config.CheckmarkMargin = new Thickness(8, 8, 24, 0);
    config.ShowCheckmarkAnimation = true;

    // ============================================
    // SELECTION BACKGROUND CUSTOMIZATION
    // ============================================
    config.SelectedItemBackgroundColor = Color.FromRgba("#E8F4F8");
    config.NormalItemBackgroundColor = Colors.Transparent;

    // ============================================
    // PAGINATION CUSTOMIZATION
    // ============================================
    config.PageSize = 50; // Number of contacts to load per page

    // ============================================
    // FEATURE FLAGS
    // ============================================
    kkContactControl.EnableSearchBar = true;
    kkContactControl.ShowBithday = true;
    kkContactControl.ShowEmail = true;
    kkContactControl.ShowAddress = false;
    kkContactControl.ShowCompany = true;
    kkContactControl.ShowUrl = false;
    kkContactControl.GetDate = true;
    kkContactControl.Loadingtext = "Loading contacts...";

    config.EnableTextChangedDelegate = true;
    config.EnableSearchButtonPressedDelegate = true;
  }

  /// <summary>
  /// Example: Minimal light theme
  /// </summary>
  public static void ApplyLightTheme()
  {
    var config = ContactConfig.Instance;

    config.SearchBarBackgroundColor = Colors.White;
    config.SearchBarTextColor = Colors.Black;
    config.GroupHeaderBackgroundColor = Colors.LightGray;
    config.GroupHeaderTextColor = Colors.Black;
    config.ContactNameTextColor = Colors.Black;
    config.ContactPhoneTextColor = Colors.Gray;
    config.SelectedItemBackgroundColor = Color.FromRgba("#F0F0F0");
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
  }

  /// <summary>
  /// Example: Compact view for showing more contacts
  /// </summary>
  public static void ApplyCompactView()
  {
    var config = ContactConfig.Instance;

    config.ContactItemHeight = 60;
    config.ContactNameFontSize = 14;
    config.ContactPhoneFontSize = 12;
    config.CollectionViewItemSpacing = 2;
    config.CheckmarkSize = 16;
    config.GroupHeaderFontSize = 14;
    config.GroupHeaderPadding = new Thickness(8, 4);
  }

  /// <summary>
  /// Example: Spacious view for better readability
  /// </summary>
  public static void ApplySpaciousView()
  {
    var config = ContactConfig.Instance;

    config.ContactItemHeight = 100;
    config.ContactNameFontSize = 20;
    config.ContactPhoneFontSize = 16;
    config.CollectionViewItemSpacing = 10;
    config.CheckmarkSize = 28;
    config.GroupHeaderFontSize = 20;
    config.GroupHeaderPadding = new Thickness(15, 10);
  }
}
