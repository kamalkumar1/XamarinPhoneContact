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
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        readonly IContact contact;
        public MainPage()
        {
            InitializeComponent();
           
        }
        async void Show_Clicked(object sender, System.EventArgs e)
        {

            MobileContact mobile = new MobileContact();
            mobile.getSelectedContact += Mobile_GetSelectedContactItem;
            await Navigation.PushModalAsync(mobile);
        }

        private void Mobile_GetSelectedContactItem(ContactItem contactItem)
        {
            
        }
    }
}
