using System;
using System.Diagnostics;
using SQLite;
using XamarinPhoneContact.Helper;
using XamarinPhoneContact.Interface;
using XamarinPhoneContact.Interface.LocalDB;
using XamarinPhoneContact.Model.LocalSql.Sqltable;

namespace XamarinPhoneContact.Model.LocalSql;

public class KKContactControlDbOperation : IKKContactControlDbOperation
{
  private readonly ISqlLiteSetup _sqlLiteSetup;
  private Lazy<SQLiteAsyncConnection>? _sQLiteAsyncConnection;
  // Lazy-initialized singleton connection (thread-safe).
  // If concurrent operations require separate connections, consider implementing connection pooling.
  public KKContactControlDbOperation(ISqlLiteSetup sqlLiteSetup)
  {
    _sqlLiteSetup = sqlLiteSetup;
  }
  private Lazy<SQLiteAsyncConnection> CreateLazyConnection()
  {
    return new Lazy<SQLiteAsyncConnection>(() =>
    {
      var dbpath = _sqlLiteSetup.SetupdbPath();
      var getConnectionString = _sqlLiteSetup.GetSQLiteConnectionString(dbpath);
      return _sqlLiteSetup.CreateConnection(getConnectionString);
    });
  }
  public SQLiteAsyncConnection GetSQLiteAsyncConnection()
  {
    try
    {
      // Initialize Lazy on first access (thread-safe)
      _sQLiteAsyncConnection ??= CreateLazyConnection();
      return _sQLiteAsyncConnection.Value;
    }
    catch (Exception ex)
    {
      throw new Exception("Error in GetSQLiteAsyncConnection: " + ex.Message);
    }
  }
  public async Task<bool> CloseSQLiteAsyncConnection()
  {
    try
    {
      if (_sQLiteAsyncConnection?.IsValueCreated ?? false)
      {
        await _sQLiteAsyncConnection.Value.CloseAsync();
        _sQLiteAsyncConnection = null;
      }
      return true;
    }
    catch (Exception ex)
    {
      throw new Exception("Error in CloseSQLiteAsyncConnection: " + ex.Message);
    }
  }
  public async Task<bool> CreateAllContactTable()
  {
    try
    {
      var connection = GetSQLiteAsyncConnection();
      await connection.CreateTableAsync<KKSqlTableForContact>();
      await connection.CreateTableAsync<KKSqlTableUpdate>();
      Debug.WriteLine("KKContactControl Database and Tables Created Successfully");
      return true;
    }
    catch (Exception ex)
    {
      throw new Exception("Error in CreateAllContactTable: " + ex.Message);
    }
  }
  public async Task<bool> DropAllContactTable()
  {
    try
    {
      var connection = GetSQLiteAsyncConnection();
      await connection.DropTableAsync<KKSqlTableForContact>();
      await connection.DropTableAsync<KKSqlTableUpdate>();
      await connection.CloseAsync();
      Debug.WriteLine("KKContactControl Database and Tables Dropped Successfully");
      return true;
    }
    catch (Exception ex)
    {
      throw new Exception("Error in DropAllContactTable: " + ex.Message);
    }
  }
  public async Task<bool> DeleteAllDataFromDbTable()
  {
    try
    {
      var connection = GetSQLiteAsyncConnection();
      await connection.DeleteAllAsync<KKSqlTableForContact>();
      await connection.DeleteAllAsync<KKSqlTableUpdate>();
      await connection.CloseAsync();
      Debug.WriteLine("KKContactControl Tables deleted Successfully from Db");
      return true;
    }
    catch (Exception ex)
    {
      throw new Exception("Error in DeleteAllDataFromDbTable: " + ex.Message);
    }
  }
}
