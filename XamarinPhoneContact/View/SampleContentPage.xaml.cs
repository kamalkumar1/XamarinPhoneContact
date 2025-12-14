using XamarinPhoneContact.ViewModel;

namespace XamarinPhoneContact.View;

public partial class SampleContentPage : ContentPage
{
	private readonly KKSingleContactView _contactView;
	KKSingleContactViewModel kKSingleContactViewModel;


	public SampleContentPage(KKSingleContactViewModel viewModel)
	{
		InitializeComponent();

		// Create and cache the ContentView
		kKSingleContactViewModel = viewModel;
		_contactView = new KKSingleContactView(viewModel);
		contentGrid.Children.Add(_contactView);
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();

		// Optional: Clean up if needed
		contentGrid.Children.Clear();
		kKSingleContactViewModel?.RestViewModel();
		kKSingleContactViewModel = null;
		BindingContext = null;
	}
}