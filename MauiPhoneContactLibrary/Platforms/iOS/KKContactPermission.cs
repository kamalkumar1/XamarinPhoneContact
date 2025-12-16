using System;
namespace MauiPhoneContactLibrary.Platforms.iOS;

public class KKContactPermission : IKKContactPermission
{
  public event EventHandler CustomPermissionStatus;

  public void CheckPermission()
  {

  }
}
