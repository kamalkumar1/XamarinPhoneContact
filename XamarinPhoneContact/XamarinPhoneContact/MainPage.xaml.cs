using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinPhoneContact
{

    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        IContact contact;
        public MainPage()
        {
            InitializeComponent();
            contact = DependencyService.Get<IContact>();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            CheckContactAccessPermission();
        }

        private void CheckContactAccessPermission()
        {
            if (contact.CheckPermission() != ContactEnum.Restricted && contact.CheckPermission() != ContactEnum.Denied)
            {
                if (contact.CheckPermission() != ContactEnum.PermissionRequired)
                {
                    LoadContact();
                }
                else
                {
                    Device.InvokeOnMainThreadAsync(() =>
                    {
                        DisplayAlert("Alert", "You have no Access to cconact", "OK");
                    });
                }
            }
            else
            {
                Device.InvokeOnMainThreadAsync(() =>
                {
                    ShowAlert();
                });
            }
        }
        public async void ShowAlert()
        {
            bool status = await DisplayAlert("Message", "PermissionRequired To Access Contaact", "Setting", "Cancel");
            if (!status.Equals("Cancel"))
            {
                contact.MoveToSetting();
            }
        }
        void LoadContact()
        {
            contact.GetAllContact();
        }
    }
}
