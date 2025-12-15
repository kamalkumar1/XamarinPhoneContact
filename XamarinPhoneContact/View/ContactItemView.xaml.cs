namespace XamarinPhoneContact.View;

public partial class ContactItemView : ContentView
{
  public ContactItemView()
  {
    InitializeComponent();
    ApplyConfiguration();
  }

  private void ApplyConfiguration()
  {
    var config = Helper.ContactConfig.Instance;

    // Apply background colors from config
    this.BackgroundColor = config.NormalItemBackgroundColor;

    // Apply configuration to the main container
    if (this.Content is Grid grid && grid.Children.Count > 0)
    {
      // Apply visual states for selection
      ApplyVisualStates(grid, config);

      // Find and configure the StackLayout containing contact info
      var stackLayout = grid.Children.FirstOrDefault(c => c is StackLayout) as StackLayout;
      if (stackLayout != null)
      {
        stackLayout.HeightRequest = config.ContactItemHeight;

        // Configure name label
        if (stackLayout.Children.Count > 0 && stackLayout.Children[0] is Label nameLabel)
        {
          nameLabel.FontSize = config.ContactNameFontSize;
          nameLabel.FontAttributes = config.ContactNameFontAttributes;
          nameLabel.TextColor = config.ContactNameTextColor;
        }

        // Configure phone label
        if (stackLayout.Children.Count > 1 && stackLayout.Children[1] is Label phoneLabel)
        {
          phoneLabel.FontSize = config.ContactPhoneFontSize;
          phoneLabel.Padding = config.ContactPhonePadding;
          phoneLabel.TextColor = config.ContactPhoneTextColor;
        }
      }

      // Find and configure the checkmark image
      var checkmarkImage = grid.Children.FirstOrDefault(c => c is Image && ((Image)c).StyleId == "TickImage") as Image;
      if (checkmarkImage == null)
      {
        checkmarkImage = grid.Children.FirstOrDefault(c => c is Image) as Image;
      }

      if (checkmarkImage != null)
      {
        checkmarkImage.HeightRequest = config.CheckmarkSize;
        checkmarkImage.WidthRequest = config.CheckmarkSize;
        checkmarkImage.Margin = config.CheckmarkMargin;
        checkmarkImage.Source = config.CheckmarkIcon;
      }
    }
  }

  private void ApplyVisualStates(Grid grid, Helper.ContactConfig config)
  {
    var visualStateGroups = new VisualStateGroupList();
    var commonStates = new VisualStateGroup { Name = "CommonStates" };

    // Normal state
    var normalState = new VisualState { Name = "Normal" };
    normalState.Setters.Add(new Setter
    {
      Property = BackgroundColorProperty,
      Value = config.NormalItemBackgroundColor
    });

    // Selected state
    var selectedState = new VisualState { Name = "Selected" };
    selectedState.Setters.Add(new Setter
    {
      Property = BackgroundColorProperty,
      Value = config.SelectedItemBackgroundColor
    });

    commonStates.States.Add(normalState);
    commonStates.States.Add(selectedState);
    visualStateGroups.Add(commonStates);

    VisualStateManager.SetVisualStateGroups(this, visualStateGroups);
    VisualStateManager.SetVisualStateGroups(grid, visualStateGroups);
  }
}
