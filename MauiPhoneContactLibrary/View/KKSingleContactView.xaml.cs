using System.Diagnostics;
using MauiPhoneContactLibrary.Helper;
using MauiPhoneContactLibrary.Model;
using MauiPhoneContactLibrary.ViewModel;

namespace MauiPhoneContactLibrary.View;

public partial class KKSingleContactView : ContentView
{
	KKSingleContactViewModel? kKSingleContactViewModel;
	private bool _isProcessingSelection = false;

	public KKSingleContactView()
	{
		InitializeComponent();
		this.Loaded += OnLoaded;
		this.Unloaded += OnUnloaded;
	}
	// Constructor with DI - chains to parameterless constructor
	public KKSingleContactView(KKSingleContactViewModel vm) : this()
	{
		kKSingleContactViewModel = vm;
		BindingContext = kKSingleContactViewModel;
	}


	private async void OnLoaded(object? sender, EventArgs e)
	{
		// if (kKSingleContactViewModel != null)
		// {
		// 	await kKSingleContactViewModel.CalulateAndGetTotalPageCount();
		// 	await kKSingleContactViewModel.LoadContactsAsync();
		// }
	}

	private void OnUnloaded(object? sender, EventArgs e)
	{
		if (kKSingleContactViewModel != null)
		{
			kKSingleContactViewModel.RestViewModel();
		}
		BindingContext = null;

		// Unsubscribe to prevent memory leaks
		Loaded -= OnLoaded;
		Unloaded -= OnUnloaded;
	}

	private async void ContactList_SelectionChangedEvent(object sender, SelectionChangedEventArgs e)
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
					kKSingleContactViewModel.UpdateSingleSelectedContact(selectedItem);
				}
				else
				{
					kKSingleContactViewModel.UpdateMultipleSelectedContacts(selectedItem);
				}
			}
			else
			{
				Debug.WriteLine("No valid contact item selected.");
			}

			// Small delay to allow UI to update before clearing selection
			await Task.Delay(50);
			// Clear selection to allow re-selecting the same item
			if (singlecontactList != null)
				singlecontactList.SelectedItem = null;
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