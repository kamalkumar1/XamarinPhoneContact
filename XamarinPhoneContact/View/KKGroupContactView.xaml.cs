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

    KKGroupContactViewModel? _KKGroupContactViewModel;
    private bool _isProcessingSelection = false;

    public KKGroupContactView()
    {
        InitializeComponent();
    }

    public KKGroupContactView(KKGroupContactViewModel vm) : this()
    {
        _KKGroupContactViewModel = vm;
        BindingContext = vm;

        // Subscribe to CollectionView scrolled event for load more
        GroupContactCollectionView.Scrolled += OnCollectionViewScrolled;

        // Subscribe to lifecycle events
        // this.Loaded += OnLoaded;
        // this.Unloaded += OnUnloaded;
    }

    private async void OnCollectionViewScrolled(object? sender, ItemsViewScrolledEventArgs e)
    {
        if (_KKGroupContactViewModel._totalPagecount <= _KKGroupContactViewModel._currentPageSize)
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
            if (_KKGroupContactViewModel != null)
            {
                await _KKGroupContactViewModel.LoadMoreCommand.ExecuteAsync(null);
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

        _KKGroupContactViewModel?.RestViewModel();
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

            var selectedItem = e.CurrentSelection[0] as ContactItem;
            if (selectedItem != null)
            {
                selectedItem.Itemselcted = !selectedItem.Itemselcted;

                // Update ViewModel with selected contact
                if (_KKGroupContactViewModel != null)
                {
                    _KKGroupContactViewModel.UpdateSelectedContact(selectedItem);
                }
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
