using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using SQLite;
using SQLitePCL;
using XamarinPhoneContact.Interface;
using XamarinPhoneContact.Interface.LocalDB;
using XamarinPhoneContact.Model.SecureKeyGenrator;

namespace XamarinPhoneContact.Model;

public class SQlLiteSetup : ISqlLiteSetup
{
  readonly string DatabaseFilename = "KKContactControlSQLite.db3";
  public string SetupdbPath()
  {
    try
    {
      return Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }
    catch (Exception ex)
    {
      throw new Exception("KKDbcontrol", ex);
    }
  }
  public SQLiteConnectionString GetSQLiteConnectionString(string databasePath)
  {
    var secureKey = KKSecureKeyGenerator.GetOrCreateSecureKey();

#if ANDROID
    return new SQLiteConnectionString(databasePath, true, secureKey);
#elif IOS
    return new SQLiteConnectionString(databasePath, true, key: secureKey);
#endif
  }
  public SQLiteAsyncConnection CreateConnection(SQLiteConnectionString getConnectionString)
  {
    var connection = new SQLiteAsyncConnection(getConnectionString.DatabasePath, false);
    return connection;
  }
  private SQLiteOpenFlags GetSecureFlags()
  {

#if IOS
    SQLiteOpenFlags Flags = // open the database in read/write mode
                                SQLiteOpenFlags.ReadWrite |
                                // create the database if it doesn't exist
                                SQLiteOpenFlags.Create |
                                // enable multi-threaded database access
                                SQLiteOpenFlags.SharedCache |
                                //set Encryption
                                SQLiteOpenFlags.ProtectionCompleteUntilFirstUserAuthentication; // Includes Data Protection
    return Flags;
#elif ANDROID
    SQLiteOpenFlags androidFlags = SQLiteOpenFlags.ReadWrite |
    SQLiteOpenFlags.Create |
    SQLiteOpenFlags.SharedCache |
    SQLiteOpenFlags.FullMutex;  // Android optimized
    return androidFlags;
#endif
  }

}
