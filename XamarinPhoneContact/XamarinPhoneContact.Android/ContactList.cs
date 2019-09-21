using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Database;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Xamarin.Forms;
using XamarinPhoneContact.Droid;
using System.Linq;
//using Context = Android.Content.Context;

[assembly: Dependency(typeof(ContactList))]
namespace XamarinPhoneContact.Droid
{
    public class ContactList : Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IContact, ICallBackInterface
    {
       private static Activity m_activity;
        // private static int RequestPermissionCode = 1;
        public event EventHandler CustomPermissionStatus;
        readonly string[] permissionscontact = { Manifest.Permission.ReadContacts };
        static int RequestPermissionCode;
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
            //MessagingCenter.Subscribe<MainPage, Dictionary>(this, "data", (s, a) => {
            //   // Events.Add($"Received message at {a.ToString()}");



            //});
            var check = GetcontactPermission();
            if (!check)
            {
                CustomPermissionStatus?.Invoke(ContactEnum.Denied, EventArgs.Empty);
                SetContactPermission();
                
            }
            else
            {
                CustomPermissionStatus?.Invoke(ContactEnum.Granted, EventArgs.Empty);
            }
           


        }
        public void SetContactPermission()
        {
            //  var activity = MainActivity.Instance;
            /// activity.callBackInterface = this;
            PhoneContactPermissionsResults.Instance.callBackInterface = this;

            ActivityCompat.RequestPermissions(m_activity, new string[] { Manifest.Permission.ReadContacts }, 1107);

        }
        private bool GetcontactPermission()
        {
            if ((int)Build.VERSION.SdkInt < 23)
            {
                return true;
            }
            var permissionCheck = ContextCompat.CheckSelfPermission(GlobalApplication.CurrentContext, Manifest.Permission.ReadContacts);
            return permissionCheck == Permission.Granted;
        }
        public Dictionary<string,object> GetAllContact()
        {
            string[] PROJECTION =  {

                  ContactsContract.Contacts.InterfaceConsts.Id,
            ContactsContract.Contacts.InterfaceConsts.DisplayName,
            ContactsContract.CommonDataKinds.StructuredName.GivenName };
          
            totalContactListWithoutGrouping = new List<ContactItem>();
            var globalVariable = GlobalApplication.CurrentContext;
            var contentResolver = globalVariable.ContentResolver;
            ICursor myCursor = contentResolver.Query(ContactsContract.Contacts.ContentUri, null, null, null, "upper(" + ContactsContract.CommonDataKinds.Phone.InterfaceConsts.DisplayName + ") ASC");
            //ICursor myCursor = contentResolver.Query(ContactsContract.Contacts.ContentUri, null, null, null,  null);
            if (myCursor.Count > 0)
            {

                while (myCursor.MoveToNext())
                {
                    ContactItem item = new ContactItem();

                    string id = myCursor.GetString(myCursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.Id)) ?? "";
                    string displayname = myCursor.GetString(myCursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.DisplayName)) ?? "";
                    item.ContactID = id;
                    item.DisplayName = displayname;

                    //Name
                    string whereName = ContactsContract.Data.InterfaceConsts.Mimetype + " = ? AND " + ContactsContract.CommonDataKinds.StructuredName.InterfaceConsts.ContactId + " = ?";
                    string[] whereNameParams = { ContactsContract.CommonDataKinds.StructuredName.ContentItemType, id };
                    ICursor struc = contentResolver.Query(ContactsContract.Data.ContentUri, null, whereName, whereNameParams, ContactsContract.CommonDataKinds.StructuredName.GivenName);

                    while (struc.MoveToNext())
                    {
                        Name name = new Name();
                        string middleName = struc.GetString(
                              struc.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredName.MiddleName));
                        name.MiddleName = middleName ?? "";
                        string suffix = struc.GetString(
                             struc.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredName.Suffix));
                        name.Suffix = suffix ?? "";
                        string prefix = struc.GetString(
                            struc.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredName.Prefix));
                        name.Prefix = prefix ?? "";
                        string givename = struc.GetString(
                               struc.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredName.GivenName));
                        name.FirstName = givename ?? "";
                        string familyname = struc.GetString(
                            struc.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredName.FamilyName));
                        name.LastName = familyname ?? "";
                        item.GetNames = name;
                        //Console.WriteLine("Name:"+givename);

                    }
                    struc.Close();

                    //phone
                    var hasPhonenumber = Convert.ToInt32(myCursor.GetString(myCursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.HasPhoneNumber)));
                    if (hasPhonenumber > 0)
                    {
                        
                        ICursor phoneCursor = contentResolver.Query(ContactsContract.CommonDataKinds.Phone.ContentUri,
                            null, ContactsContract.CommonDataKinds.Phone.InterfaceConsts.ContactId + " = ?", new string[] { id }, null);
                        List<Phone> phoneitem = new List<Phone>();
                        while (phoneCursor.MoveToNext())
                        {
                            Phone phone = new Phone();
                           // string phoneid = phoneCursor.GetString(phoneCursor.GetColumnIndex(ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Id));
                            string phonenumber = phoneCursor.GetString(phoneCursor.GetColumnIndex(ContactsContract.CommonDataKinds.Phone.Number));
                            // string phonetyoe = phoneCursor.GetString(phoneCursor.GetColumnIndex(ContactsContract.CommonDataKinds.Email.InterfaceConsts.Type + " = ?"));
                            PhoneDataKind pkind = (PhoneDataKind)phoneCursor.GetInt(phoneCursor.GetColumnIndex(ContactsContract.CommonDataKinds.CommonColumns.Type));
                            var type = pkind.ToString();
                           // phone.Phoneid = phoneid ?? "";
                            phone.PhoneNumber = phonenumber ?? "";
                            phone.Type = type ?? "";
                            phoneitem.Add(phone);

                        }
                        item.GetPhones = phoneitem;
                        phoneCursor.Close();
                    }

                    //email
                   ICursor emailCursor = contentResolver.Query(ContactsContract.CommonDataKinds.Email.ContentUri,
                      null, ContactsContract.CommonDataKinds.Phone.InterfaceConsts.ContactId + " = ?", new string[] { id }, null);
                    List<Email> emailitem = new List<Email>();
                    while (emailCursor.MoveToNext())
                    {
                        Email email = new Email();
                      // string ids = emailCursor.GetString(emailCursor.GetColumnIndex(ContactsContract.CommonDataKinds.Email.InterfaceConsts.Id));
                        string emailid = emailCursor.GetString(emailCursor.GetColumnIndex(ContactsContract.CommonDataKinds.Email.InterfaceConsts.Data));
                        // string phonetyoe = phoneCursor.GetString(phoneCursor.GetColumnIndex(ContactsContract.CommonDataKinds.Email.InterfaceConsts.Type + " = ?"));
                        PhoneDataKind pkind = (PhoneDataKind)emailCursor.GetInt(emailCursor.GetColumnIndex(ContactsContract.CommonDataKinds.CommonColumns.Type));
                        var type = pkind.ToString();
                        email.Emailid = emailid ?? "";
                        //email.id = ids ?? "";
                        email.Type = type ?? "";
                        emailitem.Add(email);
                    }
                    item.GetEmails = emailitem;
                    emailCursor.Close();
                    //Address
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
                                // Console.WriteLine(poBox + ":" + street + ":" + city + ":" + state + ":" + postalCode + ":" + country+ ":"+type);
                                AddressDataKind pkind = (AddressDataKind)addrCur.GetInt(addrCur.GetColumnIndex(ContactsContract.CommonDataKinds.CommonColumns.Type));
                                address.Type = pkind.ToString() ?? "";
                                address.FullAddress = street + "," + city + "," + state + "," + "," + postalCode + "," + country;
                                addresslist.Add(address);

                            } while (addrCur.MoveToNext());
                            item.GetAddress = addresslist;
                        }
                    }
                    addrCur.Close();
                    string[] whereNameParams1 = { ContactsContract.CommonDataKinds.Organization.ContentItemType, id };
                    ICursor struc1 = contentResolver.Query(ContactsContract.Data.ContentUri, null, whereName, whereNameParams1, null);

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
                        // Console.WriteLine("company:" + company);
                    }
                    struc1.Close();
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
                                //WebsiteDataKind pkind = (WebsiteDataKind)emailCursor.GetInt(emailCursor.GetColumnIndex(ContactsContract.CommonDataKinds.CommonColumns.Type));
                                url.URL = weburl ?? "";
                                urllist.Add(url);
                            } while (web.MoveToNext());
                        }
                    }
                    item.GetUrls = urllist;
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
                                // Console.WriteLine("ani:" + type);
                                //  Console.WriteLine("date:" + bitrh);
                            } while (events.MoveToNext());
                            item.GetDateList = dates;
                        }

                    }

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
            switch (requestCode)
            {

                case 1107:
                    {
                        if (grantResults[0] == Permission.Granted)
                        {
                            CustomPermissionStatus?.Invoke(ContactEnum.Granted, EventArgs.Empty);

                        }
                        else
                        {
                            CustomPermissionStatus?.Invoke(ContactEnum.Denied, EventArgs.Empty);

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
