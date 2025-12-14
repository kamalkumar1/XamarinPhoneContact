
using XamarinPhoneContact.Interface;
using XamarinPhoneContact.Interface.LocalDB;
using XamarinPhoneContact.Helper;
using System.Text.RegularExpressions;

namespace XamarinPhoneContact.Model;

public class KKContactBaseControl : IKKControlSetup
{
  private readonly IKKContactControlDbOperation _kKContactControlDbOperation;
  public KKContactBaseControl(IKKContactControlDbOperation kKContactControlDbOperation)
  {
    _kKContactControlDbOperation = kKContactControlDbOperation;
  }
  public async Task Initialize()
  {
    try
    {
      if (_kKContactControlDbOperation.GetSQLiteAsyncConnection() != null)
      {
        await _kKContactControlDbOperation.CreateAllContactTable();
        await _kKContactControlDbOperation.CloseSQLiteAsyncConnection();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"KKContactBaseControl Initialize Exception: {ex.Message}");
    }
  }

}
