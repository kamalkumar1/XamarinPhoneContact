using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using XamarinPhoneContact.Helper;
using XamarinPhoneContact.Model;
using XamarinPhoneContact.ViewModel;

namespace XamarinPhoneContact.View;


[XamlCompilation(XamlCompilationOptions.Compile)]

public partial class KKGroupContactView : ContentView
{
    KKGroupContactViewModel? KKGroupContactViewModel;
    private bool _isProcessingSelection = false;

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
        var collectionView = sender as CollectionView;
        if (collectionView == null) return;

        // Get total item count across all groups
        int totalItems = 0;
        if (collectionView.ItemsSource is IEnumerable<ContactGroup> groups)
        {
            totalItems = groups.Sum(g => g.Count);
        }

        // Calculate if we're near the end
        var threshold = 5; // Load more when 5 items from bottom
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
        // Prevent duplicate processing
        if (_isProcessingSelection)
            return;

        try
        {
            // Only process if there's a new selection (not deselection)
            if (e.CurrentSelection == null || e.CurrentSelection.Count == 0)
                return;

            _isProcessingSelection = true;
            var selectedItem2 = e.CurrentSelection.FirstOrDefault();
            var selectedItem = e.CurrentSelection[0] as ContactItem;
            if (selectedItem != null)
            {
                var config = ContactConfig.Instance;
                if (config.CollectionSelectionMode == SelectionMode.Single)
                {
                    KKGroupContactViewModel.UpdateSingleSelectedContact(selectedItem);
                }
                else
                {
                    KKGroupContactViewModel.UpdateMultipleSelectedContacts(selectedItem);
                }
            }
            else
            {
                Debug.WriteLine("No valid contact item selected.");
            }

            // Small delay to allow UI to update before clearing selection
            await Task.Delay(50);
            // Clear selection to allow re-selecting the same item
            if (GroupContactCollectionView != null)
                GroupContactCollectionView.SelectedItem = null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in contactList_SelectionChanged: {ex.Message}");
        }
        finally
        {
            _isProcessingSelection = false;
        }
    }
}
