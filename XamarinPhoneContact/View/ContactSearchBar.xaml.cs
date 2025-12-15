using System.Windows.Input;

namespace XamarinPhoneContact.View;

public partial class ContactSearchBar : SearchBar
{
  public static readonly BindableProperty SearchTextValueProperty =
      BindableProperty.Create(
          nameof(SearchTextValue),
          typeof(string),
          typeof(ContactSearchBar),
          string.Empty,
          BindingMode.TwoWay,
          propertyChanged: OnSearchTextValueChanged);

  public static readonly BindableProperty SearchCommandValueProperty =
      BindableProperty.Create(
          nameof(SearchCommandValue),
          typeof(ICommand),
          typeof(ContactSearchBar),
          null);

  public string SearchTextValue
  {
    get => (string)GetValue(SearchTextValueProperty);
    set => SetValue(SearchTextValueProperty, value);
  }

  public ICommand SearchCommandValue
  {
    get => (ICommand)GetValue(SearchCommandValueProperty);
    set => SetValue(SearchCommandValueProperty, value);
  }

  public ContactSearchBar()
  {
    InitializeComponent();

    // Listen to internal Text changes and propagate to SearchTextValue
    this.TextChanged += OnInternalTextChanged;
    this.SearchCommand = SearchCommandValue;
  }

  private void OnInternalTextChanged(object sender, TextChangedEventArgs e)
  {
    // Update the bindable property when user types
    if (SearchTextValue != e.NewTextValue)
    {
      SearchTextValue = e.NewTextValue;
    }
  }

  private static void OnSearchTextValueChanged(BindableObject bindable, object oldValue, object newValue)
  {
    var control = (ContactSearchBar)bindable;
    var newText = newValue as string ?? string.Empty;

    // Update internal Text if different
    if (control.Text != newText)
    {
      control.Text = newText;
    }
  }
}
