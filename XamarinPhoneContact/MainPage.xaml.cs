
using System.Diagnostics;
using XamarinPhoneContact.Helper;
using XamarinPhoneContact.Interface;
using XamarinPhoneContact.Model;
#if ANDROID
using XamarinPhoneContact.Platforms.Android;
#elif IOS       
using XamarinPhoneContact.Platforms.iOS;
#endif
using XamarinPhoneContact.View;
namespace XamarinPhoneContact
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();

        }
        async void Show_Clicked(object sender, System.EventArgs e)
        {
            try
            {
                kkContactControl.EnableMultiSelectionTickMark = true;
                kkContactControl.CloseButtonImageName = "deletebutton.png";
                IKKPhoneContactData phoneContactData = MauiServiceProvider.GetService<IKKPhoneContactData>();
                IContact contact = new ContactList(phoneContactData); // Assuming Contact implements IContact
                MobileContact mobile = new MobileContact(contact);
                mobile.getSelectedContact += Mobile_GetSelectedContactItem;
                await Navigation.PushModalAsync(mobile);


            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

            }
        }
        private void Mobile_GetSelectedContactItem(ContactItem contactItem)
        {


        }
    }
}
