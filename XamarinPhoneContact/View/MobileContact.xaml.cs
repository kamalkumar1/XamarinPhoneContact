using System.ComponentModel;
using System.Diagnostics;
using XamarinPhoneContact.Helper;
using XamarinPhoneContact.Model;
using XamarinPhoneContact.ViewModel;

namespace XamarinPhoneContact.View;

public class Namesd
{
    public string name;
    public string display;
}
public delegate void GetSelectedContactItem(KKSqlTableForContact contactItem);
[XamlCompilation(XamlCompilationOptions.Compile)]

public partial class MobileContact : ContentPage
{
    IContact _contact;
    IEnumerable<ContactGroup> totalContactItems = new List<ContactGroup>(1000);
    public GetSelectedContactItem getSelectedContact;
    public Thickness ContactViewCellMargin = new Thickness(20, 20, 20, 20);
    KKContactViewModel kKContactViewModel;
    public MobileContact(KKContactViewModel vm)
    {
        InitializeComponent();
        kKContactViewModel = vm;
        // var kkcontactviewmodel = new KKContactViewModel();
        BindingContext = vm;
        //_contact = contact;
        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            //  searchText.BackgroundColor = Colors.White;
        }

        searchText.IsSpellCheckEnabled = false;
        // dismisbutton.BackgroundColor = Colors.Transparent;
        // dismisbutton.IsVisible = kkContactControl.Dismisbutton;
        searchText.IsVisible = kkContactControl.EnableSearchBar;
        // LblLoadingText.Text = kkContactControl.Loadingtext;

        // _contact.CustomPermissionStatus += Contact_CustomPermissionStatus;
        SetCloseButton();

    }
    public void SetCloseButton()
    {

        // if (!string.IsNullOrEmpty(kkContactControl.CloseButtonImageName))
        // {
        //     dismisbutton.IsVisible = true;
        //     dismisbuttonText.IsVisible = false;
        //     dismisbutton.Source = kkContactControl.CloseButtonImageName;
        // }
        // else if (!string.IsNullOrEmpty(kkContactControl.CloseButtonTitle))
        // {
        //     dismisbutton.IsVisible = false;
        //     dismisbuttonText.IsVisible = true;
        //     dismisbuttonText.Text = kkContactControl.CloseButtonTitle;

        // }

    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        kKContactViewModel.CalulateAndGetTotalPageCount();
        kKContactViewModel.CheckPermission();
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        kKContactViewModel.RestViewModel();
        BindingContext = null;
    }

    private void CheckContactAccessPermission()
    {
        //_contact.CheckPermission();
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
            //LoadContact();
        }
    }
    void SearchList(string searchBarText)
    {
        try
        {
            if (string.IsNullOrEmpty(searchBarText))
            {
                contactList.ItemsSource = new List<KKSqlTableForContact>();
                contactList.IsGrouped = true;
                contactList.ItemsSource = totalContactItems;
            }
            else
            {
                contactList.IsGrouped = false;
                // var filteredContacts = totalContactItemsWithoutGrouping.Where(c => c.DisplayName != null && c.DisplayName.IndexOf(searchBarText, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                // if (filteredContacts != null)
                // {
                //     contactList.ItemsSource = filteredContacts;
                // }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }
    public void HandleListSelected(object sender, SelectedItemChangedEventArgs eventArgs)
    {
        //  contactList.SelectedItem = null;
    }
    public void HandleItemTapped(object sender, ItemTappedEventArgs e)
    {
        // if (!kkContactControl.EnableMultiSelectionTickMark)
        // {
        //     var item = e.Item as KKSqlTableForContact;
        //     getSelectedContact?.Invoke(item);
        // }
        // else
        // {
        //     var objes = e.Item as KKSqlTableForContact;
        //     objes.Itemselcted = objes.Itemselcted == true ? false : true;
        //     getSelectedContact?.Invoke(objes);
        //     var inex = totalContactItemsWithoutGrouping.ToList().IndexOf(objes);
        //     totalContactItemsWithoutGrouping.ToList()[inex] = objes;
        // }
    }
    public void Dismiss_Selected(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }

    private async void contactList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection == null || e.CurrentSelection.Count == 0)
            return;

        var selectedItem = e.CurrentSelection[0] as ContactItem;
        if (selectedItem == null)
            return;

        selectedItem.Itemselcted = !selectedItem.Itemselcted;


        // Clear selection to allow re-selecting the same item
        contactList.SelectedItem = null;
    }
}

