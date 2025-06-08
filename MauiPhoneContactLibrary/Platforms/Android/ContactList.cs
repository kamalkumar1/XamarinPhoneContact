using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Database;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using AndroidX.AppCompat.App;
using MauiPhoneContactLibrary.Helper;
using System.Linq;
using MauiPhoneContactLibrary.Platforms;
using MauiPhoneContactLibrary.Platforms.Android;

namespace MauiPhoneContactLibrary.Platforms
{
    public class ContactList : AppCompatActivity, IContact, ICallBackInterface
    {
        private static Activity? m_activity;
        public event EventHandler? CustomPermissionStatus;
        readonly string[] permissionscontact = { Manifest.Permission.ReadContacts };
        static int RequestPermissionCode ;
        List<ContactItem> totalContactListWithoutGrouping;
        static string[] alphate = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "#" };
        List<ContactGroup> totalContactList = new List<ContactGroup>
        {
            new ContactGroup(alphate[0], alphate[0]){},new ContactGroup(alphate[1], alphate[1]){},
            new ContactGroup(alphate[2], alphate[2]){},new ContactGroup(alphate[3], alphate[3]){},
            new ContactGroup(alphate[4], alphate[4]){},new ContactGroup(alphate[5], alphate[5]){},
            new ContactGroup(alphate[6], alphate[6]){},new ContactGroup(alphate[7], alphate[7]){},
            new ContactGroup(alphate[8], alphate[8]){},new ContactGroup(alphate[9], alphate[9]){},
            new ContactGroup(alphate[10], alphate[10]){},new ContactGroup(alphate[11], alphate[11]){},
            new ContactGroup(alphate[12], alphate[12]){},new ContactGroup(alphate[13], alphate[13]){},
            new ContactGroup(alphate[14], alphate[14]){},new ContactGroup(alphate[15], alphate[15]){},
            new ContactGroup(alphate[16], alphate[16]){},new ContactGroup(alphate[17], alphate[17]){},
            new ContactGroup(alphate[18], alphate[18]){},new ContactGroup(alphate[19], alphate[19]){},
            new ContactGroup(alphate[20], alphate[20]){},new ContactGroup(alphate[21], alphate[21]){},
            new ContactGroup(alphate[22], alphate[22]){},new ContactGroup(alphate[23], alphate[23]){},
            new ContactGroup(alphate[24], alphate[24]){},new ContactGroup(alphate[25], alphate[25]){},
            new ContactGroup(alphate[26], alphate[26]){}

        };
        public static void Init(Activity activity)
        {
            m_activity = activity;
        }
        public void CheckPermission()
        {
            Console.WriteLine("55");
            var check = GetcontactPermission();
            if (!check)
            {
                Console.WriteLine("59");
                SetContactPermission();
                CustomPermissionStatus?.Invoke(ContactEnum.Denied, EventArgs.Empty);
               

            }
            else
            {
                CustomPermissionStatus?.Invoke(ContactEnum.Granted, EventArgs.Empty);
            }
        }
        public void SetContactPermission()
        {
            try
            {
                Console.WriteLine("71");
                MainActivity.Instance.callBackInterface = this;
 
                if (m_activity == null)
                {
                    throw new InvalidOperationException("Activity reference (m_activity) is null.");
                }
                ActivityCompat.RequestPermissions(m_activity, new string[] { Manifest.Permission.ReadContacts }, 1107);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Actvityexception:"+ex);

            }
          
        }
        private bool GetcontactPermission()
        {
            Console.WriteLine("77");
            if ((int)Build.VERSION.SdkInt < 23)
            {
                return true;
            }
            Console.WriteLine("83");
            var globalVariable = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
            var permissionCheck = ContextCompat.CheckSelfPermission(globalVariable, Manifest.Permission.ReadContacts);
            return permissionCheck == Permission.Granted;
        }
        void GetCompany(ContactItem item, ContentResolver contentResolver, string id)
        {
            string whereName = ContactsContract.Data.InterfaceConsts.Mimetype + " = ? AND " + ContactsContract.CommonDataKinds.StructuredName.InterfaceConsts.ContactId + " = ?";
            string[] whereNameParams = { ContactsContract.CommonDataKinds.StructuredName.ContentItemType, id };
            ICursor struc1 = contentResolver.Query(ContactsContract.Data.ContentUri, null, whereName, whereNameParams, null);

            if (struc1.MoveToNext())
            {
                Company company = new Company();
                string companyname = struc1.GetString(
                    struc1.GetColumnIndex(ContactsContract.CommonDataKinds.Organization.Company));
                string title = struc1.GetString(
                   struc1.GetColumnIndex(ContactsContract.CommonDataKinds.Organization.Title));
                company.CompanyName = companyname ?? "";
                company.Role = title ?? "";
                item.GetCompany = company;
            }
            struc1.Close();
        }
        void GetName(ContactItem item, ContentResolver contentResolver, string id)
        {
            string whereName = ContactsContract.Data.InterfaceConsts.Mimetype + " = ? AND " + ContactsContract.CommonDataKinds.StructuredName.InterfaceConsts.ContactId + " = ?";
            string[] whereNameParams = { ContactsContract.CommonDataKinds.StructuredName.ContentItemType, id };
            var dataContentUri = ContactsContract.Data.ContentUri ?? throw new InvalidOperationException("Data Content URI is null");
            ICursor struc = contentResolver.Query(dataContentUri, null, whereName, whereNameParams, ContactsContract.CommonDataKinds.StructuredName.GivenName) ?? throw new InvalidOperationException("Query returned null cursor");

            while (struc != null && struc.MoveToNext())
            {
                Name name = new Name();
                string middleName = struc.GetString(
                      struc.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredName.MiddleName)) ?? "";
                name.MiddleName = middleName ?? "";
                string suffix = struc.GetString(
                     struc.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredName.Suffix)) ?? "";
                name.Suffix = suffix ?? "";
                string prefix = struc.GetString(
                    struc.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredName.Prefix)) ?? "";
                name.Prefix = prefix ?? "";
                string givename = struc.GetString(
                       struc.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredName.GivenName)) ?? "";
                name.FirstName = givename ?? "";
                string familyname = struc.GetString(
                    struc.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredName.FamilyName)) ?? "";
                name.LastName = familyname ?? "";
                item.GetNames = name;
            }
            struc?.Close();

        }
        void GetPhoneNumber(ContactItem item, ContentResolver contentResolver, string id, ICursor myCursor)
        {
            var hasPhonenumber = Convert.ToInt32(myCursor.GetString(myCursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.HasPhoneNumber)));
            if (hasPhonenumber > 0)
            {

                var phoneContentUri = ContactsContract.CommonDataKinds.Phone.ContentUri ?? throw new InvalidOperationException("Phone Content URI is null");
                ICursor phoneCursor = contentResolver.Query(phoneContentUri,
                    null, ContactsContract.CommonDataKinds.Phone.InterfaceConsts.ContactId + " = ?", new string[] { id }, null);
                List<Phone> phoneitem = new List<Phone>();
                while (phoneCursor != null && phoneCursor.MoveToNext())
                {
                    Phone phone = new Phone();
                    string phonenumber = phoneCursor.GetString(phoneCursor.GetColumnIndex(ContactsContract.CommonDataKinds.Phone.Number)) ?? string.Empty;
                    PhoneDataKind pkind = (PhoneDataKind)phoneCursor.GetInt(phoneCursor.GetColumnIndex(ContactsContract.CommonDataKinds.CommonColumns.Type));
                    var type = pkind.ToString();
                    phone.PhoneNumber = phonenumber ?? "";
                    phone.Type = type ?? "";
                    phoneitem.Add(phone);
                }
                item.GetPhones = phoneitem;
                phoneCursor.Close();
            }
        }
        void GetEmail(ContactItem item, ContentResolver contentResolver, string id)
        {
            ICursor emailCursor = contentResolver.Query(ContactsContract.CommonDataKinds.Email.ContentUri,
                     null, ContactsContract.CommonDataKinds.Phone.InterfaceConsts.ContactId + " = ?", new string[] { id }, null);
            List<Emailids> emailitem = new List<Emailids>();
            while (emailCursor.MoveToNext())
            {
                Emailids email = new Emailids();
                string emailid = emailCursor.GetString(emailCursor.GetColumnIndex(ContactsContract.CommonDataKinds.Email.InterfaceConsts.Data));
                PhoneDataKind pkind = (PhoneDataKind)emailCursor.GetInt(emailCursor.GetColumnIndex(ContactsContract.CommonDataKinds.CommonColumns.Type));
                var type = pkind.ToString();
                email.Emailid = emailid ?? "";
                email.Type = type ?? "";
                emailitem.Add(email);
            }
            item.GetEmails = emailitem;
            emailCursor.Close();
        }
        void GetAddress(ContactItem item, ContentResolver contentResolver, string id)
        {
            ICursor addrCur = contentResolver.Query(ContactsContract.CommonDataKinds.StructuredPostal.ContentUri,
                  null, ContactsContract.CommonDataKinds.Phone.InterfaceConsts.ContactId + " = ?", new string[] { id }, null);

            if (addrCur != null)
            {
                if (addrCur.Count > 0)
                {
                    List<Address> addresslist = new List<Address>();
                    addrCur.MoveToFirst();
                    do
                    {
                        Address address = new Address();
                        string street = addrCur.GetString(
                                 addrCur.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredPostal.Street));
                        string city = addrCur.GetString(
                                     addrCur.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredPostal.City));
                        string state = addrCur.GetString(
                                     addrCur.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredPostal.Region));
                        string postalCode = addrCur.GetString(
                                     addrCur.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredPostal.Postcode));
                        string country = addrCur.GetString(
                                  addrCur.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredPostal.Country));
                        AddressDataKind pkind = (AddressDataKind)addrCur.GetInt(addrCur.GetColumnIndex(ContactsContract.CommonDataKinds.CommonColumns.Type));
                        address.Type = pkind.ToString() ?? "";
                        address.FullAddress = street + "," + city + "," + state + "," + "," + postalCode + "," + country;
                        addresslist.Add(address);

                    } while (addrCur.MoveToNext());
                    item.GetAddress = addresslist;
                }
            }
            addrCur.Close();
        }
        void GetGetUrls(ContactItem item, ContentResolver contentResolver, string id)
        {
            string whereName = ContactsContract.Data.InterfaceConsts.Mimetype + " = ? AND " + ContactsContract.CommonDataKinds.StructuredName.InterfaceConsts.ContactId + " = ?";
            string[] whereweb = { ContactsContract.CommonDataKinds.Website.ContentItemType, id };
            ICursor web = contentResolver.Query(ContactsContract.Data.ContentUri, null, whereName, whereweb, null);
            List<Url> urllist = new List<Url>();
            if (web != null)
            {
                if (web.Count > 0)
                {
                    web.MoveToFirst();
                    do
                    {
                        Url url = new Url();
                        string weburl = web.GetString(
                           web.GetColumnIndex(ContactsContract.CommonDataKinds.Website.Url));
                        url.URL = weburl ?? "";
                        urllist.Add(url);
                    } while (web.MoveToNext());
                }
            }
            item.GetUrls = urllist;

        }
        void GetBirthDay(ContactItem item, ContentResolver contentResolver, string id)
        {
            //date
            string whereevent = ContactsContract.Data.InterfaceConsts.Mimetype + " = ? AND " + ContactsContract.CommonDataKinds.Event.InterfaceConsts.ContactId + " = ?";
            string[] whereeventp = { ContactsContract.CommonDataKinds.Event.ContentItemType, id };
            ICursor events = contentResolver.Query(ContactsContract.Data.ContentUri, null, whereevent, whereeventp, null);

            if (events != null)
            {
                if (events.Count > 0)
                {
                    List<DateList> dates = new List<DateList>();
                    events.MoveToFirst();
                    do
                    {
                        DateList dateList = new DateList();
                        string date = events.GetString(
                          events.GetColumnIndex(ContactsContract.CommonDataKinds.Event.StartDate));
                        EventDataKind pkind1 = (EventDataKind)events.GetInt(events.GetColumnIndex(ContactsContract.CommonDataKinds.CommonColumns.Type));
                        var type = pkind1.ToString();
                        if (type.Equals("Birthday"))
                        {
                            item.Birthday = date;
                        }
                        dateList.Date = date ?? "";
                        dateList.type = type ?? "";
                        dates.Add(dateList);
                    } while (events.MoveToNext());
                    item.GetDateList = dates;
                }
            }

        }
        public Dictionary<string, object> GetAllContact()
        {
            string[] PROJECTION =  {

                  ContactsContract.Contacts.InterfaceConsts.Id,
            ContactsContract.Contacts.InterfaceConsts.DisplayName,
            ContactsContract.CommonDataKinds.StructuredName.GivenName };

            totalContactListWithoutGrouping = new List<ContactItem>();
            var globalVariable = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
            var contentResolver = globalVariable.ContentResolver;
            if (contentResolver == null)
            {
                throw new InvalidOperationException("ContentResolver is null");
            }

            var contactsUri = ContactsContract.Contacts.ContentUri;
            if (contactsUri == null)
            {
                throw new InvalidOperationException("Contacts URI is null");
            }
            ICursor myCursor = contentResolver.Query(contactsUri, null, null, null, "upper(" + ContactsContract.CommonDataKinds.Phone.InterfaceConsts.DisplayName + ") ASC") ?? throw new InvalidOperationException("Query returned null cursor");
            if (myCursor.Count > 0)
            {

                while (myCursor.MoveToNext())
                {
                    ContactItem item = new ContactItem();

                    string id = myCursor.GetString(myCursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.Id)) ?? string.Empty;
                    string displayname = myCursor.GetString(myCursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.DisplayName)) ?? string.Empty;
                    item.ContactID = id;
                    item.DisplayName = displayname;


                    //GetName
                    GetName(item, contentResolver, id);
                    //phone
                    GetPhoneNumber(item, contentResolver, id, myCursor);
                    if(kkContactControl.ShowEmail)
                    {
                       //email
                        GetEmail(item, contentResolver, id);
                    }
                    if(kkContactControl.ShowAddress)
                    {
                        //address
                        GetAddress(item, contentResolver, id);
                    }
                    if(kkContactControl.ShowCompany)
                    {
                        //company
                        GetCompany(item, contentResolver, id);
                    }
                    if(kkContactControl.ShowUrl)
                    {
                        //url
                        GetGetUrls(item, contentResolver, id);
                    }
                    if(kkContactControl.ShowBithday)
                    {
                        //date
                        GetBirthDay(item, contentResolver, id);
                    }
                   
                    // //Address
                    // GetAddress(item, contentResolver, id);
                    // //GetCompany
                    // GetCompany(item, contentResolver, id);
                    // //GetUrls
                    // GetGetUrls(item, contentResolver, id);
                    // //GetDate
                    // GetBirthDay(item, contentResolver, id);


                    totalContactListWithoutGrouping.Add(item);
                    try
                    {
                        if (item.DisplayName != null && !string.IsNullOrEmpty(item.DisplayName))
                        {
                            var firstLetter = item.DisplayName.Substring(0, 1).ToUpper();
                            var indexs = Array.IndexOf(alphate, firstLetter);
                            totalContactList[indexs].Add(item);
                        }
                        else
                        {
                            totalContactList[26].Add(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                }
            }

            var dict = new Dictionary<string, object>
            {
                { "Group", totalContactList },
                { "List", totalContactListWithoutGrouping }
            };
            return dict;

        }

        public void MoveToSetting()
        {
            //throw new NotImplementedException();
        }

        public void RequestPermissionsResults(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Console.WriteLine("384");
            switch (requestCode)
            {

                case 1107:
                    {
                        if (grantResults[0] == Permission.Granted)
                        {
                            CustomPermissionStatus?.Invoke(ContactEnum.Granted, EventArgs.Empty);
                            Console.WriteLine("Granted");

                        }
                        else
                        {
                            CustomPermissionStatus?.Invoke(ContactEnum.Denied, EventArgs.Empty);
                            Console.WriteLine("denied");

                        }
                    }
                    break;
            }
        }

        public void CheckPermission(object currentPage)
        {
            //throw new NotImplementedException();
        }
    }
}
