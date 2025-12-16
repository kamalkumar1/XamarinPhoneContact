using System.Collections.ObjectModel;
using System.Windows.Input;
using XamarinPhoneContact.Helper;

namespace XamarinPhoneContact.View;

public partial class GroupedContactCollectionView : CollectionView
{
  public static readonly BindableProperty ContactGroupsProperty =
      BindableProperty.Create(nameof(ContactGroups), typeof(ObservableCollection<ContactGroup>), typeof(GroupedContactCollectionView), null,
          propertyChanged: OnContactGroupsChanged);

  public static readonly BindableProperty LoadMoreCommandProperty =
      BindableProperty.Create(nameof(LoadMoreCommand), typeof(ICommand), typeof(GroupedContactCollectionView), null,
          propertyChanged: OnLoadMoreCommandChanged);

  public ObservableCollection<ContactGroup> ContactGroups
  {
    get => (ObservableCollection<ContactGroup>)GetValue(ContactGroupsProperty);
    set => SetValue(ContactGroupsProperty, value);
  }

  public ICommand LoadMoreCommand
  {
    get => (ICommand)GetValue(LoadMoreCommandProperty);
    set => SetValue(LoadMoreCommandProperty, value);
  }

  public GroupedContactCollectionView()
  {
    InitializeComponent();
    ApplyConfiguration();
  }

  private void ApplyConfiguration()
  {
    var config = Helper.ContactConfig.Instance;

    this.RemainingItemsThreshold = config.RemainingItemsThreshold;
    this.VerticalScrollBarVisibility = config.ShowVerticalScrollBar ? ScrollBarVisibility.Always : ScrollBarVisibility.Never;
    this.SelectionMode = SelectionMode.Single;
    BackgroundColor = config.SeparateColor;

    // Apply ItemsLayout configuration
    if (this.ItemsLayout is LinearItemsLayout layout)
    {
      layout.ItemSpacing = config.CollectionViewItemSpacing;
    }
  }

  private static void OnContactGroupsChanged(BindableObject bindable, object oldValue, object newValue)
  {
    var control = (GroupedContactCollectionView)bindable;

    MainThread.BeginInvokeOnMainThread(() =>
    {
      control.ItemsSource = newValue as ObservableCollection<ContactGroup>;
    });
  }

  private static void OnLoadMoreCommandChanged(BindableObject bindable, object oldValue, object newValue)
  {
    var control = (GroupedContactCollectionView)bindable;
    control.RemainingItemsThresholdReachedCommand = newValue as ICommand;
  }
}
