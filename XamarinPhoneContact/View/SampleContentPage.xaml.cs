using System.Diagnostics;
using XamarinPhoneContact.ViewModel;
using XamarinPhoneContact.Helper;
using XamarinPhoneContact.Model;
using XamarinPhoneContact.Interface;

namespace XamarinPhoneContact.View;

public partial class SampleContentPage : ContentPage
{
	private KKSingleContactViewModel _viewModel;
	private KKSingleContactView _contactView;

	private KKGroupContactViewModel _groupViewModel;
	private KKGroupContactView _groupContactView;
	private IKKContactPermissionRequest _kKContactPermissionRequest;




	public SampleContentPage(KKSingleContactViewModel viewModel, IKKContactPermissionRequest kKContactPermissionRequest)
	//public SampleContentPage(KKGroupContactViewModel viewModel)
	{
		InitializeComponent();
		_kKContactPermissionRequest = kKContactPermissionRequest;



		SetupSingleContactView(viewModel);
		//SetupGroupContactView(viewModel);
		// Create and cache the ContentView

		//For mutiple Selection GetSelectedContacts
		//GetSelectedContacts method will retunn all selected contacts when mutiple selection enabled
		//	_groupViewModel.GetSelectedContacts();

	}
	void OnGetSelectedContactItem(ContactItem contactItem)
	{
		Debug.WriteLine($"Single Selected Contact: {contactItem.DisplayName}");
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
		//Get the selected contact item event or on both group and single contact view model For Signle Selection
		_viewModel.getSingleSelectedContact += OnGetSelectedContactItem;
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
		//Get the selected contact item event or on both group and single contact view model For Signle Selection
		_groupViewModel.getSingleSelectedContact += OnGetSelectedContactItem;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		var permissionStatus = await _kKContactPermissionRequest.GetContactAuthorizationStatus();
		if (permissionStatus)
		{

			Debug.WriteLine("Contact permission not granted.");
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
		_groupViewModel.getSingleSelectedContact -= OnGetSelectedContactItem;
		// Optional: Clean up if needed
		contentGrid.Children.Clear();
		BindingContext = null;
	}
}