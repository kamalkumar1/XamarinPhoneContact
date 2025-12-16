using System;
using System.Diagnostics;
using System.Text.Json;
using Android.Content;
using Android.Database;
using Android.Provider;
using MauiPhoneContactLibrary.Helper;
using MauiPhoneContactLibrary.Model;
using MauiPhoneContactLibrary.Model.SecureKeyGenrator;

namespace MauiPhoneContactLibrary.Platforms.Android;

public struct CNContactHelper
{
  public void GetCompany(KKSqlTableForContact item, ContentResolver contentResolver, string id)
  {
    ICursor? cursor = null;
    try
    {
      string whereName = ContactsContract.Data.InterfaceConsts.Mimetype + " = ? AND " + ContactsContract.CommonDataKinds.Organization.InterfaceConsts.ContactId + " = ?";
      string[] whereNameParams = { ContactsContract.CommonDataKinds.Organization.ContentItemType, id };
      cursor = contentResolver.Query(ContactsContract.Data.ContentUri, null, whereName, whereNameParams, null);

      if (cursor != null && cursor.MoveToNext())
      {
        var company = new Company
        {
          CompanyName = cursor.GetString(cursor.GetColumnIndex(ContactsContract.CommonDataKinds.Organization.Company)) ?? "",
          Role = cursor.GetString(cursor.GetColumnIndex(ContactsContract.CommonDataKinds.Organization.Title)) ?? ""
        };
        item.Companylist = JsonSerializer.Serialize(company);
      }
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error getting company for contact {id}: {ex.Message}");
    }
    finally
    {
      cursor?.Close();
    }
  }
  public void GetName(KKSqlTableForContact item, ContentResolver contentResolver, string id)
  {
    ICursor? cursor = null;
    try
    {
      string whereName = ContactsContract.Data.InterfaceConsts.Mimetype + " = ? AND " + ContactsContract.CommonDataKinds.StructuredName.InterfaceConsts.ContactId + " = ?";
      string[] whereNameParams = { ContactsContract.CommonDataKinds.StructuredName.ContentItemType, id };
      cursor = contentResolver.Query(ContactsContract.Data.ContentUri, null, whereName, whereNameParams, null);

      if (cursor != null && cursor.MoveToNext())
      {
        var name = new Name
        {
          MiddleName = cursor.GetString(cursor.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredName.MiddleName)) ?? "",
          Suffix = cursor.GetString(cursor.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredName.Suffix)) ?? "",
          Prefix = cursor.GetString(cursor.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredName.Prefix)) ?? "",
          FirstName = cursor.GetString(cursor.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredName.GivenName)) ?? "",
          LastName = cursor.GetString(cursor.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredName.FamilyName)) ?? ""
        };
        item.NameList = JsonSerializer.Serialize(name);
      }
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error getting name for contact {id}: {ex.Message}");
    }
    finally
    {
      cursor?.Close();
    }
  }
  public void GetPhoneNumber(KKSqlTableForContact item, ContentResolver contentResolver, string id, ICursor? myCursor)
  {
    ICursor? phoneCursor = null;
    try
    {
      if (myCursor != null)
      {
        var hasPhonenumber = Convert.ToInt32(myCursor.GetString(myCursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.HasPhoneNumber)));
        if (hasPhonenumber <= 0) return;
      }

      phoneCursor = contentResolver.Query(
        ContactsContract.CommonDataKinds.Phone.ContentUri,
        null,
        ContactsContract.CommonDataKinds.Phone.InterfaceConsts.ContactId + " = ?",
        new string[] { id },
        null);

      var phoneList = new List<Phone>();
      while (phoneCursor != null && phoneCursor.MoveToNext())
      {
        var phone = new Phone
        {
          PhoneNumber = phoneCursor.GetString(phoneCursor.GetColumnIndex(ContactsContract.CommonDataKinds.Phone.Number)) ?? "",
          Type = ((PhoneDataKind)phoneCursor.GetInt(phoneCursor.GetColumnIndex(ContactsContract.CommonDataKinds.CommonColumns.Type))).ToString()
        };
        phoneList.Add(phone);
      }

      if (phoneList.Any())
        item.Phoneslist = JsonSerializer.Serialize(phoneList);
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error getting phone numbers for contact {id}: {ex.Message}");
    }
    finally
    {
      phoneCursor?.Close();
    }
  }
  public void GetEmail(KKSqlTableForContact item, ContentResolver contentResolver, string id)
  {
    ICursor? cursor = null;
    try
    {
      cursor = contentResolver.Query(
        ContactsContract.CommonDataKinds.Email.ContentUri,
        null,
        ContactsContract.CommonDataKinds.Email.InterfaceConsts.ContactId + " = ?",
        new string[] { id },
        null);

      var emailList = new List<Emailids>();
      while (cursor != null && cursor.MoveToNext())
      {
        var email = new Emailids
        {
          Emailid = cursor.GetString(cursor.GetColumnIndex(ContactsContract.CommonDataKinds.Email.InterfaceConsts.Data)) ?? "",
          Type = ((PhoneDataKind)cursor.GetInt(cursor.GetColumnIndex(ContactsContract.CommonDataKinds.CommonColumns.Type))).ToString()
        };
        emailList.Add(email);
      }

      if (emailList.Any())
        item.Emaillist = JsonSerializer.Serialize(emailList);
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error getting emails for contact {id}: {ex.Message}");
    }
    finally
    {
      cursor?.Close();
    }
  }

  public void GetAddress(KKSqlTableForContact item, ContentResolver contentResolver, string id)
  {
    ICursor? cursor = null;
    try
    {
      cursor = contentResolver.Query(
        ContactsContract.CommonDataKinds.StructuredPostal.ContentUri,
        null,
        ContactsContract.CommonDataKinds.StructuredPostal.InterfaceConsts.ContactId + " = ?",
        new string[] { id },
        null);

      if (cursor == null || cursor.Count == 0) return;

      var addressList = new List<Address>();
      while (cursor.MoveToNext())
      {
        var street = cursor.GetString(cursor.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredPostal.Street)) ?? "";
        var city = cursor.GetString(cursor.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredPostal.City)) ?? "";
        var state = cursor.GetString(cursor.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredPostal.Region)) ?? "";
        var postalCode = cursor.GetString(cursor.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredPostal.Postcode)) ?? "";
        var country = cursor.GetString(cursor.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredPostal.Country)) ?? "";

        var address = new Address
        {
          Type = ((AddressDataKind)cursor.GetInt(cursor.GetColumnIndex(ContactsContract.CommonDataKinds.CommonColumns.Type))).ToString(),
          FullAddress = $"{street},{city},{state},{postalCode},{country}".Trim(',')
        };
        addressList.Add(address);
      }

      if (addressList.Any())
        item.Addresslist = JsonSerializer.Serialize(addressList);
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error getting addresses for contact {id}: {ex.Message}");
    }
    finally
    {
      cursor?.Close();
    }
  }
  public void GetGetUrls(KKSqlTableForContact item, ContentResolver contentResolver, string id)
  {
    ICursor? cursor = null;
    try
    {
      string whereName = ContactsContract.Data.InterfaceConsts.Mimetype + " = ? AND " + ContactsContract.CommonDataKinds.Website.InterfaceConsts.ContactId + " = ?";
      string[] whereParams = { ContactsContract.CommonDataKinds.Website.ContentItemType, id };
      cursor = contentResolver.Query(ContactsContract.Data.ContentUri, null, whereName, whereParams, null);

      if (cursor == null || cursor.Count == 0) return;

      var urlList = new List<Url>();
      while (cursor.MoveToNext())
      {
        var url = new Url
        {
          URL = cursor.GetString(cursor.GetColumnIndex(ContactsContract.CommonDataKinds.Website.Url)) ?? ""
        };
        urlList.Add(url);
      }

      if (urlList.Any())
        item.Urlslist = JsonSerializer.Serialize(urlList);
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error getting URLs for contact {id}: {ex.Message}");
    }
    finally
    {
      cursor?.Close();
    }
  }
  public void GetBirthDay(KKSqlTableForContact item, ContentResolver contentResolver, string id)
  {
    ICursor? cursor = null;
    try
    {
      string whereEvent = ContactsContract.Data.InterfaceConsts.Mimetype + " = ? AND " + ContactsContract.CommonDataKinds.Event.InterfaceConsts.ContactId + " = ?";
      string[] whereParams = { ContactsContract.CommonDataKinds.Event.ContentItemType, id };
      cursor = contentResolver.Query(ContactsContract.Data.ContentUri, null, whereEvent, whereParams, null);

      if (cursor == null || cursor.Count == 0) return;

      var dateList = new List<DateList>();
      while (cursor.MoveToNext())
      {
        var date = cursor.GetString(cursor.GetColumnIndex(ContactsContract.CommonDataKinds.Event.StartDate)) ?? "";
        var type = ((EventDataKind)cursor.GetInt(cursor.GetColumnIndex(ContactsContract.CommonDataKinds.CommonColumns.Type))).ToString();

        if (type.Equals("Birthday", StringComparison.OrdinalIgnoreCase))
        {
          item.Birthday = date;
        }

        dateList.Add(new DateList { Date = date, type = type });
      }

      if (dateList.Any())
        item.Datelist = JsonSerializer.Serialize(dateList);
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error getting birthday for contact {id}: {ex.Message}");
    }
    finally
    {
      cursor?.Close();
    }
  }
  public KKSqlTableForContact ProcessSingleContact(string id, string displayName, ContentResolver contentResolver)
  {
    try
    {
      var item = new KKSqlTableForContact
      {
        ContactID = id,
        DisplayName = displayName
      };

      GetName(item, contentResolver, id);
      GetPhoneNumber(item, contentResolver, id, null);

      if (kkContactControl.ShowEmail)
        GetEmail(item, contentResolver, id);
      if (kkContactControl.ShowAddress)
        GetAddress(item, contentResolver, id);

      if (kkContactControl.ShowCompany)
        GetCompany(item, contentResolver, id);

      if (kkContactControl.ShowUrl)
        GetGetUrls(item, contentResolver, id);

      if (kkContactControl.ShowBithday)
        GetBirthDay(item, contentResolver, id);

      return item;
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error processing contact {id}: {ex.Message}");
      return null;
    }
  }
  /// <summary>
  /// Save the encrypted sync timestamp to SharedPreferences
  /// </summary>
  public long SaveSyncTimestamp(long timestamp)
  {
    try
    {
      // Check if timestamp already exists and remove it before saving new one
      var globalVariable = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
      if (globalVariable == null) return 0;

      var prefs = globalVariable.GetSharedPreferences("ContactSync", FileCreationMode.Private);
      if (prefs == null) return 0;

      // Check if old timestamp exists and remove it
      if (prefs.Contains("ContactSyncTimestamp"))
      {
        var editor = prefs.Edit();
        editor.Remove("ContactSyncTimestamp");
        editor.Apply();
        Debug.WriteLine("🗑️ Removed old sync timestamp");
      }
      var secureKey = KKSecureKeyGenerator.GetOrCreateSecureKey();
      var encryptedTimestamp = KKEncryptionHelperAndroid.EncryptString(timestamp.ToString(), secureKey);

      if (!string.IsNullOrEmpty(encryptedTimestamp))
      {
        var editor = prefs.Edit();
        if (editor == null) return 0;

        editor.PutString("ContactSyncTimestamp", encryptedTimestamp);
        editor.Apply();
        Debug.WriteLine($"🔒 Encrypted and saved sync timestamp: {timestamp}");
        return timestamp;

      }
      else
      {
        Debug.WriteLine("❌ Failed to encrypt sync timestamp");
        return 0;
      }
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"❌ Error saving sync timestamp: {ex.Message}");
      return 0;
    }
  }
  /// <summary>
  /// Load and decrypt the sync timestamp from SharedPreferences
  /// </summary>
  public long LoadSyncTimestamp()
  {
    try
    {
      var globalVariable = Platform.CurrentActivity;
      if (globalVariable == null) return 0;

      var prefs = globalVariable.GetSharedPreferences("ContactSync", FileCreationMode.Private);
      if (prefs == null) return 0;

      var encryptedTimestamp = prefs.GetString("ContactSyncTimestamp", null);
      if (string.IsNullOrEmpty(encryptedTimestamp))
      {
        Debug.WriteLine("⚠️ No sync timestamp found");
        return 0;
      }

      var secureKey = KKSecureKeyGenerator.GetOrCreateSecureKey();
      var decryptedTimestamp = KKEncryptionHelperAndroid.DecryptString(encryptedTimestamp, secureKey);

      if (long.TryParse(decryptedTimestamp, out long timestamp))
      {
        Debug.WriteLine($"🔓 Decrypted sync timestamp: {timestamp}");
        return timestamp;
      }
      else
      {
        Debug.WriteLine("❌ Failed to parse decrypted timestamp");
        return 0;
      }
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"❌ Error loading sync timestamp: {ex.Message}");
      return 0;
    }
  }

}
