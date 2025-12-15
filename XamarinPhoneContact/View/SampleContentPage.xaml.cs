using System.Diagnostics;
using XamarinPhoneContact.ViewModel;

namespace XamarinPhoneContact.View;

public partial class SampleContentPage : ContentPage
{
	private KKSingleContactViewModel _viewModel;
	private KKSingleContactView _contactView;

	private KKGroupContactViewModel _groupViewModel;
	private KKGroupContactView _groupContactView;


	//public SampleContentPage(KKSingleContactViewModel viewModel)
	public SampleContentPage(KKGroupContactViewModel viewModel)
	{
		InitializeComponent();

		//SetupSingleContactView(viewModel);
		SetupGroupContactView(viewModel);
		// Create and cache the ContentView

	}
	/// <summary>
	/// This method setups the single contact view with out section based on the uiview 
	/// </summary>
	/// <param name="viewModel"></param>
	void SetupSingleContactView(KKSingleContactViewModel viewModel)
	{
		// Create and cache the ContentView
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_contactView = new KKSingleContactView(_viewModel);
		//page we need to add contact view
		contentGrid.Children.Add(_contactView);
	}
	/// <summary>
	/// This method setups the group contact view with  section based on the uiview
	/// </summary>
	/// <param name="viewModel"></param>
	void SetupGroupContactView(KKGroupContactViewModel viewModel)
	{
		// Create and cache the ContentView
		_groupViewModel = viewModel;
		_groupContactView = new KKGroupContactView(_groupViewModel);
		BindingContext = _groupViewModel;
		//page we need to add contact view
		contentGrid.Children.Add(_groupContactView);
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		if (_groupViewModel != null)
		{
			await _groupViewModel.LoadGroupContactsAsync();
		}
		if (_viewModel != null)
		{
			await _viewModel.CalulateAndGetTotalPageCount();
			await _viewModel.LoadContactsAsync();
		}
	}

	private void OnGetSelectedContacts()
	{
		if (_viewModel != null)
		{
			// Get all selected contacts
			var selectedContacts = _viewModel.GetSelectedContacts();

			// Or observe the SelectedContacts collection
			foreach (var contact in selectedContacts)
			{
				Debug.WriteLine($"Selected: {contact.DisplayName}");
			}
			_viewModel?.RestViewModel();
		}

		if (_groupViewModel != null)
		{
			// Get all selected contacts
			var selectedContacts = _groupViewModel.GetSelectedContacts();

			// Or observe the SelectedContacts collection
			foreach (var contact in selectedContacts)
			{
				Debug.WriteLine($"Selected: {contact.DisplayName}");
			}
			_groupViewModel?.RestViewModel();
		}

	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		OnGetSelectedContacts();
		// Optional: Clean up if needed
		contentGrid.Children.Clear();
		BindingContext = null;
	}
}