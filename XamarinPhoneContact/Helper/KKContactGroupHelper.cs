using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace XamarinPhoneContact.Helper;

public enum KKContactResulType
{
  FirstSynCompleted,
  SyncTokenFailure,
  UpdateAsyncCompleted,
  UknownFailure,
  NoChangesFoundToUpdate
}
public static class KKContactGroupHelper
{
  public readonly static string[] Alphate = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "#" };

  static ObservableCollection<ContactGroup>? groupcontactItems;
  public static ObservableCollection<ContactGroup> CreateDefaultGroups()
  {
    try
    {
      if (groupcontactItems == null || groupcontactItems.Count == 0)
      {
        groupcontactItems = [];
        foreach (var a in Alphate)
        {
          groupcontactItems.Add(new ContactGroup(a, a));
        }
        return groupcontactItems;
      }
      return groupcontactItems;
    }
    catch (Exception ex)
    {
      Debug.WriteLine("CreateDefaultGroups:" + ex.Message);
      return null;
    }

  }

  public static int GetGroupIndex(string? displayName)
  {
    if (string.IsNullOrEmpty(displayName)) return Alphate.Length - 1; // '#'
    var first = displayName.Substring(0, 1).ToUpper();
    var idx = Array.IndexOf(Alphate, first);
    return (idx >= 0 && idx < Alphate.Length) ? idx : Alphate.Length - 1;
  }

}
