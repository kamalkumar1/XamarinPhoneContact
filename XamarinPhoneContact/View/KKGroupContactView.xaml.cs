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

public partial class KKGroupContactView : ContentView
{
    IContact _contact;
    IEnumerable<ContactGroup> totalContactItems = new List<ContactGroup>(1000);
    public GetSelectedContactItem getSelectedContact;
    public Thickness ContactViewCellMargin = new Thickness(20, 20, 20, 20);
    KKGroupContactViewModel KKGroupContactViewModel;
    public KKGroupContactView()
    {
        InitializeComponent();
    }
    public KKGroupContactView(KKGroupContactViewModel vm) : this()
    {
        KKGroupContactViewModel = vm;
        BindingContext = vm;
        //_contact = contact;
        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            searchText.BackgroundColor = Colors.White;
        }

        searchText.IsSpellCheckEnabled = false;
        searchText.IsVisible = kkContactControl.EnableSearchBar;

        // Subscribe to lifecycle events
        this.Loaded += OnLoaded;
        this.Unloaded += OnUnloaded;
    }

    private async void OnLoaded(object sender, EventArgs e)
    {
        await KKGroupContactViewModel.CalulateAndGetTotalPageCount();
        KKGroupContactViewModel.CheckPermission();
    }

    private void OnUnloaded(object sender, EventArgs e)
    {
        KKGroupContactViewModel.RestViewModel();
        BindingContext = null;

        // Unsubscribe to prevent memory leaks
        this.Loaded -= OnLoaded;
        this.Unloaded -= OnUnloaded;
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

