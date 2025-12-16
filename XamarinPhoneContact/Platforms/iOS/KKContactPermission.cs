using System;
namespace XamarinPhoneContact.Platforms.iOS;

public class KKContactPermission : IKKContactPermission
{
  public event EventHandler CustomPermissionStatus;

  public void CheckPermission()
  {

  }
}
