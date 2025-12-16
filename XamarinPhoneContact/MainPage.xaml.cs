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
                await Shell.Current.GoToAsync(nameof(SampleContentPage));
                // Option 1: Direct navigation
                // await Navigation.PushAsync(new SampleContentPage());

                // Option 2: Modal navigation
                // await Navigation.PushModalAsync(new SampleContentPage());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
        private void Mobile_GetSelectedContactItem(KKSqlTableForContact contactItem)
        {


        }
    }
}
