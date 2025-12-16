
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using Contacts;
using Foundation;
using MauiPhoneContactLibrary.Helper;
using MauiPhoneContactLibrary.Model;
using MauiPhoneContactLibrary.Model.SecureKeyGenrator;

namespace MauiPhoneContactLibrary.Platforms.iOS;


public struct CNContactHelper
{

  public NSString[] GetCNcontactKey()
  {
    NSString[] allKeys = new NSString[]
    {
            CNContactKey.Birthday, CNContactKey.ThumbnailImageData,
            CNContactKey.SocialProfiles,CNContactKey.Relations,
            CNContactKey.PreviousFamilyName,CNContactKey.PostalAddresses,
            CNContactKey.PhoneticOrganizationName,CNContactKey.PhoneticMiddleName,
            CNContactKey.PhoneticGivenName,CNContactKey.PhoneticFamilyName,
            CNContactKey.PhoneNumbers,CNContactKey.OrganizationName,
            CNContactKey.NonGregorianBirthday,CNContactKey.Nickname,
            CNContactKey.NameSuffix,CNContactKey.NamePrefix,
            CNContactKey.MiddleName,CNContactKey.JobTitle,
            CNContactKey.InstantMessageAddresses,CNContactKey.ImageDataAvailable,
            CNContactKey.ImageData,CNContactKey.Identifier,
            CNContactKey.GivenName,CNContactKey.FamilyName,
            CNContactKey.EmailAddresses,CNContactKey.DepartmentName,
            CNContactKey.Dates,CNContactKey.Type,
            CNContactKey.UrlAddresses

    };
    return allKeys;

  }
  public void GetBirthDay(CNContact contact, KKSqlTableForContact item)
  {
    if (contact.Birthday != null)
    {
      var month = contact.Birthday.Month.ToString();
      var day = contact.Birthday.Day.ToString();
      var year = contact.Birthday.Year.ToString();
      item.Birthday = day + "/" + month + "/" + year;
    }
  }
  public void GetDisplayName(CNContact contact, KKSqlTableForContact item)
  {
    if (contact.GivenName.Length > 0 || contact.FamilyName.Length > 0)
    {
      //Displayname
      item.DisplayName = contact.GivenName + " " + contact.FamilyName;
    }
    else
    {
      item.DisplayName = "";
    }
  }
  public void GetName(CNContact contact, KKSqlTableForContact item)
  {
    try
    {
      Name name = new Name();
      name.FirstName = contact.GivenName;
      name.LastName = contact.FamilyName;
      name.Prefix = contact.NamePrefix;
      name.Suffix = contact.NameSuffix;
      name.MiddleName = contact.MiddleName;
      item.NameList = JsonSerializer.Serialize(name);
    }
    catch (Exception ex)
    {
      Debug.WriteLine(ex);
      item.NameList = "Errorocured";
    }
  }
  public void GetPhoneNumber(CNContact contact, KKSqlTableForContact item)
  {
    List<Phone> phoneslist = new List<Phone>();
    try
    {
      foreach (var number in contact.PhoneNumbers)
      {
        string phonenumber = number.Value.ValueForKey(new NSString("digits")).ToString() ?? "";
        string phonenumberType = CNLabeledValue<NSString>.LocalizeLabel(new NSString(number.Label ?? "")) ?? "";
        Phone phone = new Phone
        {
          Type = phonenumberType,
          PhoneNumber = phonenumber
        };
        phoneslist.Add(phone);
        Debug.WriteLine(phonenumber);

      }
      item.Phoneslist = JsonSerializer.Serialize(phoneslist);
    }
    catch (Exception ex)
    {
      Debug.WriteLine(ex);
      item.Phoneslist = "Errorocured";
    }
    finally
    {
      phoneslist.Clear();
    }

  }
  public void GetEmails(CNContact contact, KKSqlTableForContact item)
  {
    try
    {
      List<Emailids> emailslist = new List<Emailids>();
      foreach (var obj in contact.EmailAddresses)
      {
        var type = Regex.Replace(obj.Label ?? "", @"[^0-9a-zA-Z]+", "");
        var email = Regex.Replace(obj.Value ?? "", @"[^0-9a-zA-Z@]+", "");
        Emailids email1 = new Emailids();
        email1.Emailid = email;
        email1.Type = type;
        emailslist.Add(email1);
      }
      item.Emaillist = JsonSerializer.Serialize(emailslist); ;
    }
    catch (Exception ex)
    {
      Debug.WriteLine(ex);
    }

  }
  public void GetAddress(CNContact contact, KKSqlTableForContact item)
  {
    try
    {
      List<Address> addresseslist = [];
      foreach (var obj in contact.PostalAddresses)
      {
        Address address = new Address();
        var type = Regex.Replace(obj.Label ?? "", @"[^0-9a-zA-Z]+", "");
        var street = obj.Value.Street;
        var city = obj.Value.City;
        var state = obj.Value.State;
        var postalcode = obj.Value.PostalCode;
        var country = obj.Value.Country;
        address.Type = type;
        address.FullAddress = street + "," + city + "," + "," + state + "," + postalcode + "," + country;
        addresseslist.Add(address);
      }
      item.Addresslist = JsonSerializer.Serialize(addresseslist);
    }
    catch (Exception ex)
    {
      Debug.WriteLine(ex);
    }
  }
  public void GetCompany(CNContact contact, KKSqlTableForContact item)
  {
    try
    {
      Company company = new Company
      {
        CompanyName = contact.OrganizationName ?? "",
        Role = contact.JobTitle
      };
      item.Companylist = JsonSerializer.Serialize(company);
    }
    catch (Exception ex)
    {
      Debug.WriteLine(ex);
    }
  }
  public void GetUrls(CNContact contact, KKSqlTableForContact item)
  {
    try
    {
      List<Url> url = new List<Url>();
      foreach (var obj in contact.UrlAddresses)
      {
        Url url1 = new Url();
        url1.URL = Regex.Replace(obj.Value ?? "", @"[^0-9a-zA-Z@]+", "");
        url.Add(url1);
      }
      item.Urlslist = JsonSerializer.Serialize(url);
    }
    catch (Exception ex)
    {
      Debug.WriteLine(ex);
    }
  }
  public void GetDate(CNContact contact, KKSqlTableForContact item)
  {
    //Date
    try
    {
      List<DateList> dateLists = new List<DateList>();
      foreach (var obj in contact.Dates)
      {
        var type = Regex.Replace(obj.Label ?? "", @"[^0-9a-zA-Z]+", "");
        var datestring = obj.Value;
        DateList list = new DateList();
        var month = contact.Birthday?.Month.ToString() ?? "";
        var day = contact.Birthday?.Day.ToString() ?? "";
        var year = contact.Birthday?.Year.ToString() ?? "";
        list.Date = day + "/" + month + "/" + year;
        list.type = type;
        dateLists.Add(list);
      }
      item.Datelist = JsonSerializer.Serialize(dateLists);
    }
    catch (Exception ex)
    {
      Debug.WriteLine(ex);

    }
  }
  public async Task<KKSqlTableForContact> ProcessSingleContact(CNContact contact)
  {
    var item = new KKSqlTableForContact { ContactID = contact.Identifier ?? "" };
    var helper = this;

    var taskdisplay = Task.Run(() => helper.GetDisplayName(contact, item));
    var taskname = Task.Run(() => helper.GetName(contact, item));
    var taskphone = Task.Run(() => helper.GetPhoneNumber(contact, item));
    var taskbithday = Task.Run(() => { if (kkContactControl.ShowBithday) helper.GetBirthDay(contact, item); });
    var taskemail = Task.Run(() =>
    {
      if (kkContactControl.ShowEmail) helper.GetEmails(contact, item);
    });
    var taskaddress = Task.Run(() =>
    {
      if (kkContactControl.ShowAddress) helper.GetAddress(contact, item);
    });
    var taskcompany = Task.Run(() =>
    {
      if (kkContactControl.ShowCompany) helper.GetCompany(contact, item);
    });
    var taskshowurl = Task.Run(() =>
    {
      if (kkContactControl.ShowUrl) helper.GetUrls(contact, item);
    });
    var taskshowdate = Task.Run(() =>
    {
      if (kkContactControl.GetDate) helper.GetDate(contact, item);
    });

    await Task.WhenAll(taskdisplay, taskname, taskphone, taskbithday, taskemail, taskaddress, taskcompany, taskshowurl, taskshowdate);
    return item;
  }
  public void SaveHistoryToken(NSData token)
  {
    try
    {
      if (token != null)
      {
        //Remove the old and update with new encrypted token
        NSUserDefaults.StandardUserDefaults.RemoveObject("ContactHistoryToken");
        var secureKey = KKSecureKeyGenerator.GetOrCreateSecureKey();
        var encryptedToken = KKEncryptionHelperiOS.EncryptData(token, secureKey);

        if (encryptedToken != null)
        {
          NSUserDefaults.StandardUserDefaults.SetValueForKey(encryptedToken, new NSString("ContactHistoryToken"));
          NSUserDefaults.StandardUserDefaults.Synchronize();
          Debug.WriteLine("🔒 History token encrypted and saved successfully");
        }
        else
        {
          Debug.WriteLine("❌ Failed to encrypt history token");
        }
      }
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"❌ Error saving history token: {ex.Message}");
    }
  }
  /// <summary>
  /// Load history token from preferences
  /// </summary>
  public NSData? LoadHistoryToken()
  {
    try
    {
      var encryptedToken = NSUserDefaults.StandardUserDefaults.ValueForKey(new NSString("ContactHistoryToken")) as NSData;

      if (encryptedToken != null)
      {
        var secureKey = KKSecureKeyGenerator.GetOrCreateSecureKey();
        var decryptedToken = KKEncryptionHelperiOS.DecryptData(encryptedToken, secureKey);

        if (decryptedToken != null)
        {
          Debug.WriteLine("🔓 Loaded and decrypted existing history token for incremental sync");
          return decryptedToken;
        }
        else
        {
          Debug.WriteLine("⚠️ Failed to decrypt history token - will perform full sync");
          return null;
        }
      }
      else
      {
        Debug.WriteLine("No history token found - first sync will be full");
        return null;
      }
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Error loading history token: {ex.Message}");
      return null;
    }
  }

}
