using System.Diagnostics;
using XamarinPhoneContact.ViewModel;

namespace XamarinPhoneContact.View;

public partial class SampleContentPage : ContentPage
{
	private KKSingleContactViewModel _viewModel;
	private readonly KKSingleContactView _contactView;


	public SampleContentPage(KKSingleContactViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;

		// Create and cache the ContentView
		_contactView = new KKSingleContactView(viewModel);
		contentGrid.Children.Add(_contactView);
	}


	private void OnGetSelectedContacts()
	{
		// Get all selected contacts
		var selectedContacts = _viewModel.GetSelectedContacts();

		// Or observe the SelectedContacts collection
		foreach (var contact in selectedContacts)
		{
			Debug.WriteLine($"Selected: {contact.DisplayName}");
		}
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		OnGetSelectedContacts();
		// Optional: Clean up if needed
		contentGrid.Children.Clear();
		_viewModel?.RestViewModel();
		BindingContext = null;
	}
}