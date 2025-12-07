using System;
using Contacts;
using Foundation;

namespace MauiPhoneContactLibrary.Platforms.iOS;

public class KKCNContact:CNContact
{

        public KKCNContact()
        {
           
        }
  public override NSDictionary GetDictionaryOfValuesFromKeys(NSString[] keys)
  {
    return base.GetDictionaryOfValuesFromKeys(keys);
  }
  
 

}
