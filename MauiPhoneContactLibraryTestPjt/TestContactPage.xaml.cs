using MauiPhoneContactLibrary.Helper;
using MauiPhoneContactLibrary.Interface;
using MauiPhoneContactLibrary.Model;
using MauiPhoneContactLibrary.View;
using MauiPhoneContactLibrary.ViewModel;
using System.Threading.Tasks;

namespace MauiPhoneContactLibraryTestPjt;

public partial class TestContactPage : ContentPage
{
    //KKGroupContactViewModel is Grouped listview model
    KKGroupContactViewModel groupContactViewModel;

    //IKKContactPermissionRequest use this interface to request contact permission
    //if you can implement default permission request of the maui use.
    private IKKContactPermissionRequest _kKContactPermissionRequest;

    public TestContactPage(KKGroupContactViewModel viewModel, IKKContactPermissionRequest kKContactPermissionRequest)
    {
        InitializeComponent();
        //set BindingContext
        BindingContext = viewModel;
        //keep reference of viewmodel
        groupContactViewModel = viewModel;
        //keep reference of contact permission request interface
        _kKContactPermissionRequest = kKContactPermissionRequest;
        //create KKSingleContactView view with ungrouped listview
        var _contactView = new KKGroupContactView(viewModel);
        //subscribe to get selected contact event when single selectiom mode is enabled
        viewModel.getSingleSelectedContact += OnGetSelectedContactItem;
        //Use this method to get selected contacts from viewmodel whe multiple selection mode is enabled
        viewModel.GetSelectedContacts();
        //page we need to add contact view
        contacgrid.Children.Add(_contactView);
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var permissionStatus = await _kKContactPermissionRequest.GetContactAuthorizationStatus();
        if (permissionStatus)
        {
            await groupContactViewModel.LoadGroupContactsAsync();
        }
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        //unsubscribe event
        groupContactViewModel.getSingleSelectedContact -= OnGetSelectedContactItem;
        //reset viewmodel
        groupContactViewModel.RestViewModel();
        //clear reference
        groupContactViewModel = null;
        //clear bindingcontext
        BindingContext = null;
        //clear permission request reference
        _kKContactPermissionRequest = null;
    }
    public void OnGetSelectedContactItem(ContactItem contactItem)
    {
        // Handle the selected contact item for ungrouped listview
    }

    // public partial class TestContactPage : ContentPage
    // {
    //     //KKSingleContactViewModel is Ungrouped listview model
    //     KKSingleContactViewModel singleContactViewModel;

    //     //IKKContactPermissionRequest use this interface to request contact permission
    //     //if you can implement default permission request of the maui use.
    //     private IKKContactPermissionRequest _kKContactPermissionRequest;

    //     public TestContactPage(KKSingleContactViewModel viewModel, IKKContactPermissionRequest kKContactPermissionRequest)
    //     {
    //         InitializeComponent();
    //         //set BindingContext
    //         BindingContext = viewModel;
    //         //keep reference of viewmodel
    //         singleContactViewModel = viewModel;
    //         //keep reference of contact permission request interface
    //         _kKContactPermissionRequest = kKContactPermissionRequest;
    //         //create KKSingleContactView view with ungrouped listview
    //         var _contactView = new KKSingleContactView(viewModel);
    //         //subscribe to get selected contact event when single selectiom mode is enabled
    //         viewModel.getSingleSelectedContact += OnGetSelectedContactItem;
    //         //Use this method to get selected contacts from viewmodel whe multiple selection mode is enabled
    //         viewModel.GetSelectedContacts();
    //         //page we need to add contact view
    //         contacgrid.Children.Add(_contactView);
    //     }
    //     protected override async void OnAppearing()
    //     {
    //         base.OnAppearing();
    //         var permissionStatus = await KKMauiServiceProvider.GetService<IKKContactPermissionRequest>().GetContactAuthorizationStatus();
    //         if (permissionStatus)
    //         {
    //             await singleContactViewModel.LoadContactsAsync();
    //         }
    //     }
    //     protected override void OnDisappearing()
    //     {
    //         base.OnDisappearing();
    //         //unsubscribe event
    //         singleContactViewModel.getSingleSelectedContact -= OnGetSelectedContactItem;
    //         //reset viewmodel
    //         singleContactViewModel.RestViewModel();
    //         //clear reference
    //         singleContactViewModel = null;
    //         //clear bindingcontext
    //         BindingContext = null;
    //         //clear permission request reference
    //         _kKContactPermissionRequest = null;
    //     }
    //     public void OnGetSelectedContactItem(ContactItem contactItem)
    //     {
    //         // Handle the selected contact item for ungrouped listview
    //     }
}