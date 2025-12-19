using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using MauiPhoneContactLibrary.Helper;
using MauiPhoneContactLibrary.Model;
using MauiPhoneContactLibrary.ViewModel;
using Microsoft.Maui.Controls;

namespace MauiPhoneContactLibrary.View;


[XamlCompilation(XamlCompilationOptions.Compile)]

public partial class KKGroupContactView : ContentView
{
    KKGroupContactViewModel? KKGroupContactViewModel;
    private bool _isProcessingSelection = false;

    public KKGroupContactView()
    {
        InitializeComponent();
        ConfigureAlphabetCollectionViewTemplate();
    }

    private void ConfigureAlphabetCollectionViewTemplate()
    {
        // Helper method to enumerate visual children of a view

        // Use the correct CollectionView for the alphabet list (should be the right-side CollectionView, not GroupContactCollectionView)
        var alphabetCollectionView = this.FindByName<CollectionView>("AlphabetCollectionView");

        if (alphabetCollectionView != null)
        {
            alphabetCollectionView.ItemTemplate = new DataTemplate(() =>
            {
                var config = ContactConfig.Instance;
                var span = new Span
                {
                    FontAttributes = FontAttributes.Bold
                };
                span.SetBinding(Span.TextProperty, ".");
                span.SetBinding(Span.FontSizeProperty, new Binding(nameof(ContactConfig.AlphabetFontSize), source: config));

                var formattedString = new FormattedString();
                formattedString.Spans.Add(span);

                var label = new Label
                {
                    FormattedText = formattedString,
                    BackgroundColor = config.AlphabetBackgroundColor,
                    Padding = config.AlphabetPadding,
                    HorizontalTextAlignment = config.AlphabetHorizontalTextAlignment,
                    VerticalTextAlignment = config.AlphabetVerticalTextAlignment
                };

                var tapGesture = new TapGestureRecognizer();
                tapGesture.SetBinding(TapGestureRecognizer.CommandProperty, new Binding("ScrollToLetterCommand", source: BindingContext));
                tapGesture.SetBinding(TapGestureRecognizer.CommandParameterProperty, ".");
                label.GestureRecognizers.Add(tapGesture);

                return label;
            });
        }
    }

    public KKGroupContactView(KKGroupContactViewModel vm) : this()
    {
        KKGroupContactViewModel = vm;
        BindingContext = vm;

        // Subscribe to CollectionView scrolled event for load more
        GroupContactCollectionView.Scrolled += OnCollectionViewScrolled;

        // Subscribe to alphabet scroll requests
        KKGroupContactViewModel.ScrollToLetterRequested += OnScrollToLetterRequested;

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

        if (KKGroupContactViewModel != null)
        {
            KKGroupContactViewModel.ScrollToLetterRequested -= OnScrollToLetterRequested;
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
                    KKGroupContactViewModel?.UpdateSingleSelectedContact(selectedItem);
                }
                else
                {
                    KKGroupContactViewModel?.UpdateMultipleSelectedContacts(selectedItem);
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

    private async void OnScrollToLetterRequested(string letter)
    {
        try
        {
            GroupContactCollectionView.DisableInteractionWhenLoading = true;
            if (KKGroupContactViewModel?.ContactGroups == null || GroupContactCollectionView == null)
                return;
            // Find the target group
            var group = KKGroupContactViewModel?.ContactGroups.FirstOrDefault(g =>
                string.Equals(g.ShortTitle, letter, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrEmpty(g.Title) && string.Equals(g.Title.Substring(0, 1), letter, StringComparison.OrdinalIgnoreCase)) ||
                string.Equals(g.Title, letter, StringComparison.OrdinalIgnoreCase));

            // If no items yet, keep loading until the group has items or no new data comes in
            if (group == null || group.Count == 0)
            {
                while (true)
                {
                    var beforeTotal = KKGroupContactViewModel!.ContactGroups.Sum(g => g.Count);
                    await KKGroupContactViewModel.LoadMoreCommand.ExecuteAsync(null);
                    await Task.Delay(100);

                    // Re-evaluate the group after loading
                    group = KKGroupContactViewModel.ContactGroups.FirstOrDefault(g =>
                        string.Equals(g.ShortTitle, letter, StringComparison.OrdinalIgnoreCase) ||
                        (!string.IsNullOrEmpty(g.Title) && string.Equals(g.Title.Substring(0, 1), letter, StringComparison.OrdinalIgnoreCase)) ||
                        string.Equals(g.Title, letter, StringComparison.OrdinalIgnoreCase));

                    // Break if we found items
                    if (group != null && group.Count > 0)
                        break;

                    // Stop trying if no new items were loaded
                    var afterTotal = KKGroupContactViewModel.ContactGroups.Sum(g => g.Count);
                    if (afterTotal <= beforeTotal)
                        break;
                }
            }

            if (group is null || group.Count == 0)
                return;

            var firstItem = group[0];

            // Scroll so that the section header is at the top
            GroupContactCollectionView.ScrollTo(
                item: firstItem,
                group: group,
                position: ScrollToPosition.Start,
                animate: false);

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"OnScrollToLetterRequested error: {ex.Message}");
        }
        finally
        {
            GroupContactCollectionView.DisableInteractionWhenLoading = false;
        }
    }
}
