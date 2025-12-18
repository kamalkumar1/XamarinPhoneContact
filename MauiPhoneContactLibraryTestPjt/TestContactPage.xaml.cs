
using MauiPhoneContactLibrary.Helper;
using MauiPhoneContactLibrary.View;
using MauiPhoneContactLibrary.ViewModel;
using System.Threading.Tasks;

namespace MauiPhoneContactLibraryTestPjt;

public partial class TestContactPage : ContentPage
{
    KKSingleContactViewModel singleContactViewModel;
	public TestContactPage(KKSingleContactViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
        singleContactViewModel = viewModel;
        var _contactView = new KKSingleContactView(viewModel);
        viewModel.getSingleSelectedContact += OnGetSelectedContactItem;
        //page we need to add contact view
        contacgrid.Children.Add(_contactView);
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
       await singleContactViewModel.LoadContactsAsync();
    }
    public void OnGetSelectedContactItem(ContactItem contactItem)
    {
        // Handle the selected contact item
    }
}