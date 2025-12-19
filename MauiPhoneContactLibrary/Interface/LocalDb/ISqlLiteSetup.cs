using System;
using SQLite;

namespace MauiPhoneContactLibrary.Interface.LocalDB;

public interface ISqlLiteSetup
{
  public string SetupdbPath();
  public SQLiteConnectionString GetSQLiteConnectionString(string databasePath);
  public SQLiteAsyncConnection CreateConnection(SQLiteConnectionString getConnectionString);

}
