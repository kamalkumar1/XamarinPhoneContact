using XamarinPhoneContact.Helper;
using XamarinPhoneContact.Model;
using XamarinPhoneContact.ViewModel;

namespace XamarinPhoneContact.View;

public partial class KKSingleContactView : ContentView
{
	KKSingleContactViewModel kKSingleContactViewModel;
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


	private async void OnLoaded(object sender, EventArgs e)
	{
		if (kKSingleContactViewModel != null)
		{
			await kKSingleContactViewModel.CalulateAndGetTotalPageCount();
			kKSingleContactViewModel.CheckPermission();
		}
	}

	private void OnUnloaded(object sender, EventArgs e)
	{
		if (kKSingleContactViewModel != null)
		{
			kKSingleContactViewModel.RestViewModel();
		}
		BindingContext = null;

		// Unsubscribe to prevent memory leaks
		this.Loaded -= OnLoaded;
		this.Unloaded -= OnUnloaded;
	}

	private void contactList_SelectionChanged(object sender, SelectionChangedEventArgs e)
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