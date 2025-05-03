using System.ComponentModel;
using System.Diagnostics;
using XamarinPhoneContact.Helper;
namespace XamarinPhoneContact.View;

public class Namesd
{
    public string name;
    public string display;
}
public delegate void GetSelectedContactItem(ContactItem contactItem);
[XamlCompilation(XamlCompilationOptions.Compile)]

public partial class MobileContact : ContentPage
{
    IContact _contact;
    IEnumerable<ContactGroup> totalContactItems;
    IEnumerable<ContactItem> totalContactItemsWithoutGrouping;
    public GetSelectedContactItem getSelectedContact;
    public Thickness ContactViewCellMargin = new Thickness(20, 20, 20, 20);

    public MobileContact(IContact contact)
    {
        InitializeComponent();
        _contact = contact;
        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            searchText.BackgroundColor = Colors.White;
        }

        searchText.IsSpellCheckEnabled = false;
        dismisbutton.BackgroundColor = Colors.Transparent;
        dismisbutton.IsVisible = kkContactControl.Dismisbutton;
        searchText.IsVisible = kkContactControl.EnableSearchBar;
        contactList.SeparatorColor = kkContactControl.ListSepratorColor;
        LblLoadingText.Text= kkContactControl.Loadingtext;

        _contact.CustomPermissionStatus += Contact_CustomPermissionStatus;
        SetCloseButton();

    }
    public void SetCloseButton()
    {
        if (!string.IsNullOrEmpty(kkContactControl.CloseButtonImageName))
        {
            dismisbutton.IsVisible = true;
            dismisbuttonText.IsVisible = false;
            dismisbutton.Source = kkContactControl.CloseButtonImageName;
        }
        else if (!string.IsNullOrEmpty(kkContactControl.CloseButtonTitle))
        {
            dismisbutton.IsVisible = false;
            dismisbuttonText.IsVisible = true;
            dismisbuttonText.Text = kkContactControl.CloseButtonTitle;

        }

    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        CheckContactAccessPermission();
    }

    private void CheckContactAccessPermission()
    {
        _contact.CheckPermission();
    }
    void Handle_TextChanged(object sender, TextChangedEventArgs e)
    {
        SearchList(e.NewTextValue);
    }
    void Handle_SearchButtonPressed(object sender, EventArgs e)
    {
        SearchList(searchText.Text);
    }
    private void Contact_CustomPermissionStatus(object sender, EventArgs e)
    {
        var permission = (ContactEnum)sender;
        if (permission == ContactEnum.Granted)
        {
            LoadContact();
        }
    }
    void SearchList(string searchBarText)
    {
        try
        {
            if (string.IsNullOrEmpty(searchBarText))
            {
                contactList.ItemsSource = new List<ContactItem>();
                contactList.IsGroupingEnabled = true;
                contactList.ItemsSource = totalContactItems;
            }
            else
            {
                 contactList.IsGroupingEnabled = false;
                var filteredContacts = totalContactItemsWithoutGrouping.Where(c => c.DisplayName != null && c.DisplayName.IndexOf(searchBarText, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                if(filteredContacts!=null)
                {
                    contactList.ItemsSource = filteredContacts;
                }
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }

    }
    async void LoadContact()
    {
        bottomLayout.IsVisible = true;
        await Task.Run(async () =>
        {
            var keyValuePairs = _contact.GetAllContact();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                bottomLayout.IsVisible = false;
                // Code to run on the main thread

                totalContactItems = (IEnumerable<ContactGroup>)keyValuePairs["Group"];
                totalContactItemsWithoutGrouping = (IEnumerable<ContactItem>)keyValuePairs["List"];
                contactList.ItemsSource = totalContactItems;

                // Set margin for ViewCell

            });
        });

    }
    public void Handle_clear(object sender, EventArgs e)
    {
        var selectedlist = (from s in totalContactItemsWithoutGrouping
                            where s.Itemselcted == true
                            select s).ToList();
        foreach (var item in selectedlist)
        {
            var index = totalContactItemsWithoutGrouping.ToList().IndexOf(item);
            item.Itemselcted = false;
            totalContactItemsWithoutGrouping.ToList()[index] = item;

        }
        if (string.IsNullOrEmpty(searchText.Text))
        {
            contactList.ItemsSource = totalContactItems;
        }
        else
        {
            contactList.ItemsSource = totalContactItemsWithoutGrouping;
        }

    }
    public void HandleListSelected(object sender, SelectedItemChangedEventArgs eventArgs)
    {
        //var objes = (ContactItem)eventArgs.SelectedItem;
        //objes.Itemselcted = objes.Itemselcted == true ? false:true;
        //GetSelectedContactItem?.Invoke(sender, EventArgs.Empty);
        contactList.SelectedItem = null;
    }
    public void HandleItemTapped(object sender, ItemTappedEventArgs e)
    {

        if (!kkContactControl.EnableMultiSelectionTickMark)
        {
            var item = e.Item as ContactItem;
            getSelectedContact?.Invoke(item);
        }
        else
        {
            var objes = e.Item as ContactItem;
            objes.Itemselcted = objes.Itemselcted == true ? false : true;
            getSelectedContact?.Invoke(objes);
            var inex = totalContactItemsWithoutGrouping.ToList().IndexOf(objes);
            totalContactItemsWithoutGrouping.ToList()[inex] = objes;
        }
    }
    public void Dismiss_Selected(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }
}

