using System;

namespace MauiPhoneContactLibrary.Helper;

public struct TotalContactCollection
{
  public TotalContactCollection()
  {
  }
  public static string[] alphate = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "#" };
  public List<ContactGroup> totalContactList = new List<ContactGroup>
        {
            new(alphate[0], alphate[0]){},new(alphate[1], alphate[1]){},
            new(alphate[2], alphate[2]){},new ContactGroup(alphate[3], alphate[3]){},
            new(alphate[4], alphate[4]){},new ContactGroup(alphate[5], alphate[5]){},
            new(alphate[6], alphate[6]){},new ContactGroup(alphate[7], alphate[7]){},
            new(alphate[8], alphate[8]){},new(alphate[9], alphate[9]){},
            new(alphate[10], alphate[10]){},new ContactGroup(alphate[11], alphate[11]){},
            new(alphate[12], alphate[12]){},new ContactGroup(alphate[13], alphate[13]){},
            new(alphate[14], alphate[14]){},new ContactGroup(alphate[15], alphate[15]){},
            new(alphate[16], alphate[16]){},new ContactGroup(alphate[17], alphate[17]){},
            new(alphate[18], alphate[18]){},new ContactGroup(alphate[19], alphate[19]){},
            new(alphate[20], alphate[20]){},new ContactGroup(alphate[21], alphate[21]){},
            new(alphate[22], alphate[22]){},new ContactGroup(alphate[23], alphate[23]){},
            new(alphate[24], alphate[24]){},new ContactGroup(alphate[25], alphate[25]){},
            new(alphate[26], alphate[26]){}

        };


}
