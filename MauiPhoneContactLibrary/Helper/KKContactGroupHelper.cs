using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

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

  // public static ObservableCollection<ContactGroup> CreateDefaultGroups()
  // {
  //   try
  //   {
  //     if (groupcontactItems == null || groupcontactItems.Count == 0)
  //     {
  //       groupcontactItems = [];
  //       foreach (var a in Alphate)
  //       {
  //         groupcontactItems.Add(new ContactGroup(a, a));
  //       }
  //       return groupcontactItems;
  //     }
  //     return groupcontactItems;
  //   }
  //   catch (Exception ex)
  //   {
  //     Debug.WriteLine("CreateDefaultGroups:" + ex.Message);
  //     return null;
  //   }
  // }

  /// <summary>
  /// Creates contact groups with sections only when they have items
  /// </summary>
  public static ObservableCollection<ContactGroup> CreateGroupsWithSections(List<ContactItem> contacts)
  {
    try
    {
      var groupedContacts = new ObservableCollection<ContactGroup>();

      if (contacts == null || contacts.Count == 0)
        return groupedContacts;

      // Group contacts by their first letter
      var contactsByLetter = contacts
        .GroupBy(c =>
        {
          if (string.IsNullOrEmpty(c.DisplayName))
            return "#";
          var first = c.DisplayName.Substring(0, 1).ToUpper();
          var idx = Array.IndexOf(Alphate, first);
          return (idx >= 0 && idx < Alphate.Length - 1) ? first : "#";
        })
        .OrderBy(g =>
        {
          var idx = Array.IndexOf(Alphate, g.Key);
          return idx >= 0 ? idx : Alphate.Length - 1;
        });

      // Only create groups for letters that have contacts
      foreach (var group in contactsByLetter)
      {
        var contactGroup = new ContactGroup(group.Key, group.Key);
        foreach (var contact in group.OrderBy(c => c.DisplayName))
        {
          contactGroup.Add(contact);
        }
        groupedContacts.Add(contactGroup);
      }

      return groupedContacts;
    }
    catch (Exception ex)
    {
      Debug.WriteLine("CreateGroupsWithSections:" + ex.Message);
      return new ObservableCollection<ContactGroup>();
    }
  }

  /// <summary>
  /// Adds contact to existing grouped collection, creating section if needed
  /// </summary>
  public static void AddContactToGroupedCollection(ObservableCollection<ContactGroup> groupedContacts, ContactItem contact)
  {
    try
    {
      if (string.IsNullOrEmpty(contact.DisplayName)) return;

      var first = contact.DisplayName.Substring(0, 1).ToUpper();
      var idx = Array.IndexOf(Alphate, first);
      var sectionKey = (idx >= 0 && idx < Alphate.Length - 1) ? first : "#";

      // Find existing group or create new one
      var existingGroup = groupedContacts.FirstOrDefault(g => g.ShortTitle == sectionKey);

      if (existingGroup != null)
      {
        // Add to existing group in sorted position
        var insertIndex = existingGroup
          .TakeWhile(c => string.Compare(c.DisplayName, contact.DisplayName, StringComparison.OrdinalIgnoreCase) < 0)
          .Count();
        existingGroup.Insert(insertIndex, contact);
      }
      else
      {
        // Create new group and insert in correct alphabetical position
        var newGroup = new ContactGroup(sectionKey, sectionKey);
        newGroup.Add(contact);

        var groupInsertIndex = groupedContacts
          .TakeWhile(g =>
          {
            var gIdx = Array.IndexOf(Alphate, g.ShortTitle);
            var sIdx = Array.IndexOf(Alphate, sectionKey);
            return gIdx < sIdx;
          })
          .Count();

        groupedContacts.Insert(groupInsertIndex, newGroup);
      }
    }
    catch (Exception ex)
    {
      Debug.WriteLine("AddContactToGroupedCollection:" + ex.Message);
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
