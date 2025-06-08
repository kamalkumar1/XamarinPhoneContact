



using MauiPhoneContactLibrary;
using MauiPhoneContactLibrary.Helper;
using MauiPhoneContactLibrary.Platforms;
using MauiPhoneContactLibrary.View;
using System.Diagnostics;

namespace MauiPhoneContactLibraryTestPjt
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            try
            {
                kkContactControl.EnableMultiSelectionTickMark = true;
              //  kkContactControl.CloseButtonImageName = "deletebutton.png";
                IContact contact = new ContactList(); // Assuming Contact implements IContact
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
