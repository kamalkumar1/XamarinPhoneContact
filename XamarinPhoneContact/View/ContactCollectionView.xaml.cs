using System.Collections.ObjectModel;
using System.Windows.Input;
using XamarinPhoneContact.Helper;

namespace XamarinPhoneContact.View;

public partial class ContactCollectionView : CollectionView
{
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
