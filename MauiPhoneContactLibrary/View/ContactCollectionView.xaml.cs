
using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiPhoneContactLibrary.Helper;

namespace MauiPhoneContactLibrary.View;

public partial class ContactCollectionView : CollectionView
{
  public static readonly BindableProperty DisableInteractionWhenLoadingProperty =
    BindableProperty.Create(nameof(DisableInteractionWhenLoading), typeof(bool), typeof(ContactCollectionView), false, propertyChanged: OnDisableInteractionWhenLoadingChanged);
  public bool DisableInteractionWhenLoading
  {
    get => (bool)GetValue(DisableInteractionWhenLoadingProperty);
    set => SetValue(DisableInteractionWhenLoadingProperty, value);
  }
  private static void OnDisableInteractionWhenLoadingChanged(BindableObject bindable, object oldValue, object newValue)
  {
    var control = (ContactCollectionView)bindable;
    bool isDisabled = (bool)newValue;
    control.InputTransparent = isDisabled;
    control.IsEnabled = !isDisabled;
  }
  public static readonly BindableProperty ContactItemsProperty =
      BindableProperty.Create(nameof(ContactItems), typeof(ObservableCollection<ContactItem>), typeof(ContactCollectionView), null,
          propertyChanged: OnContactItemsChanged);

  public static readonly BindableProperty LoadMoreCommandProperty =
      BindableProperty.Create(nameof(LoadMoreCommand), typeof(ICommand), typeof(ContactCollectionView), null,
          propertyChanged: OnLoadMoreCommandChanged);

  public ObservableCollection<ContactItem> ContactItems
  {
    get => (ObservableCollection<ContactItem>)GetValue(ContactItemsProperty);
    set => SetValue(ContactItemsProperty, value);
  }

  public ICommand LoadMoreCommand
  {
    get => (ICommand)GetValue(LoadMoreCommandProperty);
    set => SetValue(LoadMoreCommandProperty, value);
  }

  public ContactCollectionView()
  {
    InitializeComponent();
    ApplyConfiguration();
  }

  private void ApplyConfiguration()
  {
    var config = Helper.ContactConfig.Instance;

    this.RemainingItemsThreshold = config.RemainingItemsThreshold;
    this.VerticalScrollBarVisibility = config.ShowVerticalScrollBar ? ScrollBarVisibility.Always : ScrollBarVisibility.Never;
    this.SelectionMode = SelectionMode.Single; //config.CollectionSelectionMode;
    BackgroundColor = config.SeparateColor;

    // Apply ItemsLayout configuration
    if (this.ItemsLayout is LinearItemsLayout layout)
    {
      layout.ItemSpacing = config.CollectionViewItemSpacing;
    }
  }

  private static void OnContactItemsChanged(BindableObject bindable, object oldValue, object newValue)
  {
    var control = (ContactCollectionView)bindable;

    MainThread.BeginInvokeOnMainThread(() =>
    {
      control.ItemsSource = newValue as ObservableCollection<ContactItem>;
    });
  }

  private static void OnLoadMoreCommandChanged(BindableObject bindable, object oldValue, object newValue)
  {
    var control = (ContactCollectionView)bindable;
    control.RemainingItemsThresholdReachedCommand = newValue as ICommand;
  }
}
