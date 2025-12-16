using System;
using SQLite;

namespace XamarinPhoneContact.Interface.LocalDB;

public interface IKKContactControlDbOperation
{
  public SQLiteAsyncConnection GetSQLiteAsyncConnection();
  public Task<bool> CloseSQLiteAsyncConnection();
  public Task<bool> CreateAllContactTable();
  public Task<bool> DropAllContactTable();
  public Task<bool> DeleteAllDataFromDbTable();
}
