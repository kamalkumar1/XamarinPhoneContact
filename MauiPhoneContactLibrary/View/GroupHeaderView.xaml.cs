namespace MauiPhoneContactLibrary.View;

public partial class GroupHeaderView : Label
{
  public static readonly BindableProperty GroupTitleProperty =
      BindableProperty.Create(
          nameof(GroupTitle),
          typeof(string),
          typeof(GroupHeaderView),
          string.Empty,
          propertyChanged: OnGroupTitleChanged);

  public string GroupTitle
  {
    get => (string)GetValue(GroupTitleProperty);
    set => SetValue(GroupTitleProperty, value);
  }

  public GroupHeaderView()
  {
    InitializeComponent();
    ApplyConfiguration();
  }

  private void ApplyConfiguration()
  {
    var config = Helper.ContactConfig.Instance;

    this.FontSize = config.GroupHeaderFontSize;
    this.FontAttributes = config.GroupHeaderFontAttributes;
    this.Padding = config.GroupHeaderPadding;
    this.BackgroundColor = config.GroupHeaderBackgroundColor;
    this.TextColor = config.GroupHeaderTextColor;
  }

  private static void OnGroupTitleChanged(BindableObject bindable, object oldValue, object newValue)
  {
    var control = (GroupHeaderView)bindable;
    control.Text = newValue as string ?? string.Empty;
  }
}
