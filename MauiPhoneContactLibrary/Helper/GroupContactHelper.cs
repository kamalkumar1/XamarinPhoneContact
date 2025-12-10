using System;
using System.Collections.Generic;

namespace MauiPhoneContactLibrary.Helper
{
  public static class GroupContactHelper
  {
    public readonly static string[] Alphate = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "#" };

    public static List<ContactGroup> CreateDefaultGroups()
    {
      var groups = new List<ContactGroup>(Alphate.Length);
      foreach (var a in Alphate)
      {
        groups.Add(new ContactGroup(a, a));
      }
      return groups;
    }

    public static int GetGroupIndex(string? displayName)
    {
      if (string.IsNullOrEmpty(displayName)) return Alphate.Length - 1; // '#'
      var first = displayName.Substring(0, 1).ToUpper();
      var idx = Array.IndexOf(Alphate, first);
      return (idx >= 0 && idx < Alphate.Length) ? idx : Alphate.Length - 1;
    }
  }
}
