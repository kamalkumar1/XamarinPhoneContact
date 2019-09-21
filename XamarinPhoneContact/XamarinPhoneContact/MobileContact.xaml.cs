using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace XamarinPhoneContact
{
    public class Namesd
    {
        public string name;
        public string display;
    }
    public delegate void GetSelectedContactItem(ContactItem contactItem);
    [DesignTimeVisible(true)]
    public partial class MobileContact : ContentPage
    {
        readonly IContact contact;
        IEnumerable<ContactGroup> totalContactItems;
        IEnumerable<ContactItem> totalContactItemsWithoutGrouping;
        public GetSelectedContactItem getSelectedContact;
        public MobileContact()
        {
            InitializeComponent();
            if (Device.Android == Device.RuntimePlatform)
            {
                searchText.BackgroundColor = Color.White;
            }

            searchText.IsSpellCheckEnabled = false;
            dismisbutton.BackgroundColor = Color.Transparent;
            dismisbutton.IsVisible = ContactConfig.Instance.Dismisbutton;
            searchText.IsVisible = ContactConfig.Instance.EnableSearchBar;

            if (ContactConfig.Instance.EnableMultiSelectionTickMark)
            {
                clearButton.IsVisible = true;

            }
            else
            {
                clearButton.IsVisible = false;

            }
            contactList.SeparatorColor = ContactConfig.Instance.ListSepratorColor;
            contact = DependencyService.Get<IContact>();
            contact.CustomPermissionStatus += Contact_CustomPermissionStatus;
            SetCloseButton();

        }
        public void SetCloseButton()
        {
            if (!string.IsNullOrEmpty(ContactConfig.Instance.CloseButtonImageName))
            {
                dismisbutton.IsVisible = true;
                dismisbuttonText.IsVisible = false;
                dismisbutton.Source = ContactConfig.Instance.CloseButtonImageName;
            }
            if (!string.IsNullOrEmpty(ContactConfig.Instance.CloseButtonTitle))
            {
                dismisbutton.IsVisible = false;
                dismisbuttonText.IsVisible = true;
                dismisbuttonText.Text = ContactConfig.Instance.CloseButtonTitle;

            }
            if (string.IsNullOrEmpty(ContactConfig.Instance.CloseButtonTitle) && string.IsNullOrEmpty(ContactConfig.Instance.CloseButtonImageName))
            {
                dismisbuttonText.Text = "Close";
                dismisbuttonText.IsVisible = true;
                dismisbutton.IsVisible = true;
                dismisbutton.Source = "";



            }

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            CheckContactAccessPermission();
        }

        private void CheckContactAccessPermission()
        {
            contact.CheckPermission();
        }
        void Handle_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchList(e.NewTextValue);
        }
        void Handle_SearchButtonPressed(object sender, EventArgs e)
        {
            SearchList(searchText.Text);
        }
        private void Contact_CustomPermissionStatus(object sender, EventArgs e)
        {
            var permission = (ContactEnum)sender;
            if (permission == ContactEnum.Granted)
            {
                LoadContact();
            }
        }
        void SearchList(string searchBarText)
        {
            if (string.IsNullOrEmpty(searchBarText))
            {
                contactList.ItemsSource = new List<ContactItem>();
                contactList.IsGroupingEnabled = true;
                contactList.ItemsSource = totalContactItems;
            }
            else
            {
                contactList.IsGroupingEnabled = false;
                var cvcv = totalContactItemsWithoutGrouping.Where(c => c.DisplayName.Contains(searchBarText) || c.GetNames.FirstName.Contains(searchBarText) || c.GetNames.LastName.Contains(searchBarText));
                contactList.ItemsSource = cvcv;

            }

        }
        async void LoadContact()
        {
            bottomLayout.IsVisible = true;
            await Task.Run(async () =>
            {
                var keyValuePairs = contact.GetAllContact();
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    bottomLayout.IsVisible = false;
                    // Code to run on the main thread

                    totalContactItems = (IEnumerable<ContactGroup>)keyValuePairs["Group"];
                    totalContactItemsWithoutGrouping = (IEnumerable<ContactItem>)keyValuePairs["List"];
                    contactList.ItemsSource = totalContactItems;
                });
            });
         
        }
        public void Handle_clear(object sender, EventArgs e)
        {
            var selectedlist = (from s in totalContactItemsWithoutGrouping
                                where s.Itemselcted == true
                                select s).ToList();
            foreach (var item in selectedlist)
            {
                var index = totalContactItemsWithoutGrouping.ToList().IndexOf(item);
                item.Itemselcted = false;
                totalContactItemsWithoutGrouping.ToList()[index] = item;

            }
            if (string.IsNullOrEmpty(searchText.Text))
            {
                contactList.ItemsSource = totalContactItems;
            }
            else
            {
                contactList.ItemsSource = totalContactItemsWithoutGrouping;
            }

        }
        public void HandleListSelected(object sender, SelectedItemChangedEventArgs eventArgs)
        {
            //var objes = (ContactItem)eventArgs.SelectedItem;
            //objes.Itemselcted = objes.Itemselcted == true ? false:true;
            //GetSelectedContactItem?.Invoke(sender, EventArgs.Empty);
            contactList.SelectedItem = null;
        }
        public void HandleItemTapped(object sender, ItemTappedEventArgs e)
        {

            if (!ContactConfig.Instance.EnableMultiSelectionTickMark)
            {
                var item = e.Item as ContactItem;
                getSelectedContact?.Invoke(item);
            }
            else
            {
                var objes = e.Item as ContactItem;
                objes.Itemselcted = objes.Itemselcted == true ? false : true;
                getSelectedContact?.Invoke(objes);
                var inex = totalContactItemsWithoutGrouping.ToList().IndexOf(objes);
                totalContactItemsWithoutGrouping.ToList()[inex] = objes;
            }



        }
        public void Dismiss_Selected(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
