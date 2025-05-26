
using System.ComponentModel;
using System.Diagnostics;
using XamarinPhoneContact.Helper;
using XamarinPhoneContact.Platforms;
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
                kkContactControl.CloseButtonImageName ="deletebutton.png";
                 IContact contact = new ContactList(); // Assuming Contact implements IContact
                 MobileContact mobile = new MobileContact(contact);
                 mobile.getSelectedContact += Mobile_GetSelectedContactItem;
                 await Navigation.PushModalAsync(mobile);
                

            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);

            }
        }
        private void Mobile_GetSelectedContactItem(ContactItem contactItem)
        {
            
            
        }
    }
}
