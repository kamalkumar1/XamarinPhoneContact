using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using XamarinPhoneContact.Helper;
using XamarinPhoneContact.Model;
using XamarinPhoneContact.ViewModel;

namespace XamarinPhoneContact.View;

public class Namesd
{
    public string? name;
    public string? display;
}
public delegate void GetSelectedContactItem(KKSqlTableForContact contactItem);
[XamlCompilation(XamlCompilationOptions.Compile)]

public partial class KKGroupContactView : ContentView
{
    IContact? _contact;
    IEnumerable<ContactGroup> totalContactItems = new List<ContactGroup>(1000);
    public GetSelectedContactItem? getSelectedContact;
    public Thickness ContactViewCellMargin = new Thickness(20, 20, 20, 20);
    KKGroupContactViewModel? KKGroupContactViewModel;

    public KKGroupContactView()
    {
        InitializeComponent();
    }

    public KKGroupContactView(KKGroupContactViewModel vm) : this()
    {
        KKGroupContactViewModel = vm;
        BindingContext = vm;

        // Subscribe to CollectionView scrolled event for load more
        GroupContactCollectionView.Scrolled += OnCollectionViewScrolled;

        // Subscribe to lifecycle events
        // this.Loaded += OnLoaded;
        // this.Unloaded += OnUnloaded;
    }

    private async void OnCollectionViewScrolled(object? sender, ItemsViewScrolledEventArgs e)
    {
        if (KKGroupContactViewModel._totalPagecount <= KKGroupContactViewModel._currentPageSize)
        {
            Debug.WriteLine("All pages loaded, no more data to load.");
            return;
        }
        var collectionView = sender as CollectionView;
        if (collectionView == null) return;

        // Get total item count across all groups
        int totalItems = 0;
        if (collectionView.ItemsSource is IEnumerable<ContactGroup> groups)
        {
            totalItems = groups.Sum(g => g.Count);
        }

        // Calculate if we're near the end
        var threshold = 2; // Load more when 5 items from bottom
        var lastVisibleIndex = e.FirstVisibleItemIndex + e.CenterItemIndex;

        Debug.WriteLine($"Scrolled - LastVisible: {lastVisibleIndex}, TotalItems: {totalItems}");

        if (totalItems > 0 && lastVisibleIndex >= totalItems - threshold)
        {
            Debug.WriteLine("Threshold reached, triggering LoadMore");
            if (KKGroupContactViewModel != null)
            {
                await KKGroupContactViewModel.LoadMoreCommand.ExecuteAsync(null);
            }
        }
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
        // Unsubscribe from events
        if (GroupContactCollectionView != null)
        {
            GroupContactCollectionView.Scrolled -= OnCollectionViewScrolled;
        }

        KKGroupContactViewModel?.RestViewModel();
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
        //  contactList.SelectedItem = null;
    }
}
